﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Orbit.Core.Players;
using System.Windows.Media;
using Orbit.Core.Scene;
using Lidgren.Network;
using Orbit.Core;
using Orbit.Core.Scene.Entities;
using Orbit.Core.Scene.Entities.Implementations;
using Orbit.Core.Helpers;
using System.Net;
using System.Windows;
using Orbit.Core.Weapons;
using Orbit.Core.Server.Match;
using Orbit.Core.Server.Level;
using Orbit.Core.AI;

namespace Orbit.Core.Server
{
    public class GameManager : IGameState
    {
        private ServerMgr serverMgr;
        private List<Player> players;
        private SortedSet<string> tournamentPlayerIds;
        private List<ISceneObject> objects;
        private ITournamentMatchManager matchManager;
        private IGameLevel gameLevel;

        public int Level { get; set; }
        public bool IsRunning { get; set; }

        private bool matchCreated;

        public GameManager(ServerMgr serverMgr, List<Player> players)
        {
            this.serverMgr = serverMgr;
            this.players = players;
            if (serverMgr.TournamentSettings == null)
                serverMgr.TournamentSettings = new TournamentSettings(true);

            switch (serverMgr.TournamentSettings.MMType)
            {
                case GameMatchManagerType.WINS_THEN_SCORE:
                    matchManager = new WinsThenScoreMatchManager(players, serverMgr.GetRandomGenerator(), serverMgr.TournamentSettings.RoundCount);
                    break;
                case GameMatchManagerType.ONLY_SCORE:
                    matchManager = new ScoreMatchManager(players, serverMgr.GetRandomGenerator(), serverMgr.TournamentSettings.RoundCount);
                    break;
                case GameMatchManagerType.TEST_LEADER_SPECTATOR:
                    matchManager = new LeaderSpectatorMatchManager(players);
                    break;
                default:
                    throw new NotImplementedException("Unknown MatchManager required");
            }
            
            tournamentPlayerIds = new SortedSet<string>();
            if (serverMgr.GameType == Gametype.TOURNAMENT_GAME)
                players.ForEach(p => tournamentPlayerIds.Add(p.Data.HashId));

            matchCreated = false;
        }

        public void CreateNewMatch()
        {
            if (matchCreated)
                return;

            matchCreated = true;

            CreateNewLevel();

            // pri solo hre se vytvori jeden bot
            if (players.Count == 1)
            {
                Player bot = serverMgr.CreateAndAddPlayer(BotNameAccessor.GetBotName(SharedDef.DEFAULT_BOT), "NullBotHash", Colors.White);
                bot.Data.PlayerType = PlayerType.BOT;
                if (gameLevel.IsBotAllowed())
                    bot.Data.BotType = SharedDef.DEFAULT_BOT;
                bot.Data.StartReady = true;
                Color plrColor = players[0].Data.PlayerColor;
                bot.Data.PlayerColor = Color.FromRgb((byte)(0xFF - plrColor.R), (byte)(0xFF - plrColor.G), (byte)(0xFF - plrColor.B));
            }

            Tuple<Player, Player> selectedPlayers = matchManager.SelectPlayersForNewMatch();
            Player plr1 = selectedPlayers.Item1;
            Player plr2 = selectedPlayers.Item2;

            PlayerPosition firstPlayerPosition = serverMgr.GetRandomGenerator().Next(2) == 0 ? PlayerPosition.LEFT : PlayerPosition.RIGHT;
            if (plr1 != null)
                plr1.Data.PlayerPosition = firstPlayerPosition;
            if (plr2 != null)
                plr2.Data.PlayerPosition = firstPlayerPosition == PlayerPosition.RIGHT ? PlayerPosition.LEFT : PlayerPosition.RIGHT;
        }

        private void CreateNewLevel()
        {
            objects = new List<ISceneObject>();

            gameLevel = GameLevelManager.CreateNewGameLevel(serverMgr, serverMgr.TournamentSettings.Level, objects);
            gameLevel.CreateLevelObjects();
        }

        public void GameEnded(Player plr, GameEnd endType)
        {
            IsRunning = false;
            matchCreated = false;

            if (serverMgr.GameType == Gametype.TOURNAMENT_GAME)
                matchManager.OnMatchEnd(plr, endType);
        }

        public void ObjectDestroyed(long id)
        {
            ISceneObject obj = objects.Find(o => o.Id == id);
            if (obj == null)
                return;

            objects.Remove(obj);

            if (obj is Asteroid)
            {
                obj = ServerSceneObjectFactory.CreateNewAsteroidOnEdge(serverMgr, (obj as Asteroid).IsHeadingRight);
                GameLevelManager.SendNewObject(serverMgr, obj);
                objects.Add(obj);
            }
        }

        private void SendMatchData()
        {
            // poslani vsech hracu
            NetOutgoingMessage outmsg = serverMgr.CreateAllPlayersDataMessage();
            serverMgr.BroadcastMessage(outmsg);

            // poslani vsech asteroidu
            outmsg = serverMgr.CreateNetMessage();
            outmsg.Write((int)PacketType.ALL_ASTEROIDS);

            Int32 count = 0;
            objects.ForEach(x => { if (x is Asteroid) count++; });
            outmsg.Write(count);

            foreach (ISceneObject obj in objects)
                if (obj is Asteroid)
                    (obj as Asteroid).WriteObject(outmsg);

            serverMgr.BroadcastMessage(outmsg);

        }

        public bool RequestStartMatch(Player p)
        {
            if (IsRunning && !p.IsActivePlayer() && !SharedDef.ALLOW_SPECTATORS_IN_DUO_MATCH)
                return false;

            // do tournamentu se nemuzou pridat dalsi hraci, kteri nebyli v lobby pri startu
            if (serverMgr.GameType == Gametype.TOURNAMENT_GAME && !tournamentPlayerIds.Contains(p.Data.HashId))
                return false;

            p.Data.StartReady = true;

            if ((p.IsActivePlayer() && !IsRunning && players.Count(plr => plr.IsActivePlayer() && plr.Data.StartReady) == 2)
                || (!IsRunning && serverMgr.GameType == Gametype.TOURNAMENT_GAME))
            {
                if (serverMgr.GameType == Gametype.TOURNAMENT_GAME)
                {
                    NetOutgoingMessage tournamentMsg = serverMgr.CreateNetMessage();
                    tournamentMsg.Write((int)PacketType.TOURNAMENT_STARTING);
                    serverMgr.BroadcastMessage(tournamentMsg);
                }

                SendMatchData();

                NetOutgoingMessage startMsg = serverMgr.CreateNetMessage();
                startMsg.Write((int)PacketType.START_GAME_RESPONSE);
                serverMgr.BroadcastMessage(startMsg);

                gameLevel.OnStart();

                IsRunning = true;
                return true;
            }
            
            // TODO: nemuze se stat, ze by jeden spectator dal request vickrat? 
            // pak by bylo jeste potreba kontrolovat stav pred a po requestu
            if (!p.IsActivePlayer() && IsRunning)
            {
                if (serverMgr.GameType == Gametype.TOURNAMENT_GAME)
                {
                    NetOutgoingMessage tournamentMsg = serverMgr.CreateNetMessage();
                    tournamentMsg.Write((int)PacketType.TOURNAMENT_STARTING);
                    serverMgr.SendMessage(tournamentMsg, p);
                }

                NetOutgoingMessage outmsg = serverMgr.CreateAllPlayersDataMessage();
                serverMgr.SendMessage(outmsg, p);

                // na zacatku neposilame zadne asteroidy, protoze nevime jejich soucasnou pozici - posilaji se az nove
                // ale zaroven tato zprava iniciuje klienta, takze je nutna
                outmsg = serverMgr.CreateNetMessage();
                outmsg.Write((int)PacketType.ALL_ASTEROIDS);
                outmsg.Write(0);
                serverMgr.SendMessage(outmsg, p);

                NetOutgoingMessage startMsg = serverMgr.CreateNetMessage();
                startMsg.Write((int)PacketType.START_GAME_RESPONSE);
                serverMgr.SendMessage(startMsg, p);
            }

            return IsRunning;
        }

        public bool CheckTournamentFinished(bool announce = false)
        {
            Player winner = matchManager.GetWinner();
            if (winner == null)
                return false;

            if (announce)
            {
                NetOutgoingMessage tournamentFinished = serverMgr.CreateNetMessage();
                tournamentFinished.Write((int)PacketType.TOURNAMENT_FINISHED);
                tournamentFinished.Write(winner.GetId());
                //hraci kteri hrali posledni hru
                players.ForEach(p => { if (p.IsActivePlayer()) tournamentFinished.Write(p.GetId()); });
                serverMgr.BroadcastMessage(tournamentFinished);
            }

            return true;
        }

        public void Update(float tpf)
        {
            if (!IsRunning)
                return;

            gameLevel.Update(tpf);
        }

        public static int GetRequiredNumberOfMatches(int players, int rounds)
        {
            return FastMath.Factorial(players) * rounds / (2 * FastMath.Factorial(players - 2));
        }
    }

    public enum GameLevel
    {
        NORMAL1,
        TEST_EMPTY,
        TEST_BASE_COLLISIONS,
        TEST_POWERUPS,
        TEST_STATIC_OBJ
    }
}
