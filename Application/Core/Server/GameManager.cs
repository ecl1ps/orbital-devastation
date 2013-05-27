using System;
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
        private ITournamentMatchManager matchManager;
        private IGameLevel gameLevel;

        public bool IsRunning { get; set; }

        private bool matchCreated;

        private float totalTime = 0;

        public GameManager(ServerMgr serverMgr, List<Player> players)
        {
            this.serverMgr = serverMgr;
            this.players = players;
            if (serverMgr.TournamentSettings == null)
                serverMgr.TournamentSettings = new TournamentSettings(true);

            CreateMatchManager(serverMgr.TournamentSettings.MMType);
            
            tournamentPlayerIds = new SortedSet<string>();
            if (serverMgr.GameType == Gametype.TOURNAMENT_GAME)
                players.ForEach(p => tournamentPlayerIds.Add(p.Data.HashId));

            matchCreated = false;
        }

        private void CreateMatchManager(MatchManagerType type)
        {
            switch (type)
            {
                case MatchManagerType.ONLY_SCORE:
                    matchManager = new ScoreMatchManager(players, serverMgr.GetRandomGenerator(), serverMgr.TournamentSettings.RoundCount);
                    break;
                case MatchManagerType.SKIRMISH:
                    matchManager = new SkirmishMatchManager(players, serverMgr.GetRandomGenerator(), serverMgr.TournamentSettings.RoundCount);
                    break;
                case MatchManagerType.QUICK_GAME:
                    matchManager = new QuickGameMatchManager(players, serverMgr.GetRandomGenerator(), serverMgr.TournamentSettings.RoundCount);
                    break;

                // testovaci managery
                case MatchManagerType.TEST_LEADER_SPECTATOR:
                    matchManager = new LeaderSpectatorMatchManager(players);
                    break;
                default:
                    throw new NotImplementedException("Unknown MatchManager required");
            }
        }
        public void CreateNewMatch()
        {
            if (matchCreated)
                return;

            if (!matchManager.HasRightNumberOfPlayersForStart())
                return;

            matchCreated = true;

            CreateNewLevel();

            Tuple<Player, Player> selectedPlayers = matchManager.SelectPlayersForNewMatch();
            Player plr1 = selectedPlayers.Item1;
            Player plr2 = selectedPlayers.Item2;


            PlayerPosition firstPlayerPosition = PlayerPosition.INVALID;
            // pokud hrac zadnou pozici jeste nemel, tak se vybere nahodne jinak se mu priradi ta druha
            if (plr1 == null || plr1.Data.PlayerPosition == PlayerPosition.INVALID)
                firstPlayerPosition = serverMgr.GetRandomGenerator().Next(2) == 0 ? PlayerPosition.LEFT : PlayerPosition.RIGHT;
            else
                firstPlayerPosition = plr1.Data.PlayerPosition == PlayerPosition.RIGHT ? PlayerPosition.LEFT : PlayerPosition.RIGHT;

            if (plr1 != null)
                plr1.Data.PlayerPosition = firstPlayerPosition;
            if (plr2 != null)
                plr2.Data.PlayerPosition = firstPlayerPosition == PlayerPosition.RIGHT ? PlayerPosition.LEFT : PlayerPosition.RIGHT;

            foreach (Player p in players)
                GenerateMiningModulePositionAndColor(p);
        }

        private void GenerateMiningModulePositionAndColor(Player p)
        {
            if (p.IsActivePlayer())
                return;

            Player friend = players.Find(other => other.GetId() == p.Data.FriendlyPlayerId);
            p.Data.SpecialColor = friend != null ? friend.Data.PlayerColor : p.Data.PlayerColor;

            p.Data.MiningModuleStartPos = new Vector(
                serverMgr.GetRandomGenerator().Next(0, (int)SharedDef.VIEW_PORT_SIZE.Width - 30),
                serverMgr.GetRandomGenerator().Next((int)SharedDef.LOWER_ORBIT_AREA.Top + 60, (int)PlayerBaseLocation.GetBaseLocation(PlayerPosition.LEFT).Top - 100));
        }

        private void CreateNewLevel()
        {
            gameLevel = GameLevelManager.CreateNewGameLevel(serverMgr, serverMgr.TournamentSettings.Level);
            gameLevel.CreateBots(players, serverMgr.TournamentSettings.BotCount, serverMgr.TournamentSettings.BotType);
        }

        public void GameEnded(Player plr, GameEnd endType)
        {
            IsRunning = false;
            matchCreated = false;

            if (serverMgr.GameType == Gametype.TOURNAMENT_GAME)
                matchManager.OnMatchEnd(plr, endType, totalTime, serverMgr);
        }

        public void ObjectDestroyed(long id)
        {
            gameLevel.ObjectDestroyed(id);
        }

        private void SendMatchData()
        {
            // poslani vsech hracu
            NetOutgoingMessage outmsg = serverMgr.CreateAllPlayersDataMessage();
            serverMgr.BroadcastMessage(outmsg);

            // poslani vsech asteroidu
            outmsg = serverMgr.CreateNetMessage();
            outmsg.Write((int)PacketType.ALL_ASTEROIDS);

            List<ISceneObject> objects = gameLevel.GetLevelObjects();
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
            if (!matchManager.HasRightNumberOfPlayersForStart())
                return false;

            // zabrani pripojeni spectatora do quick game
            if (IsRunning && serverMgr.GameType != Gametype.TOURNAMENT_GAME && !p.IsActivePlayer() && !SharedDef.ALLOW_SPECTATORS_IN_DUO_MATCH)
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
                serverMgr.TournamentSettings.PlayedMatches++;
                return true;
            }
            
            // pro spectatora, ktery se reconnectuje do bezici hry
            if (!p.IsActivePlayer() && IsRunning)
            {
                if (serverMgr.GameType == Gametype.TOURNAMENT_GAME)
                {
                    NetOutgoingMessage tournamentMsg = serverMgr.CreateNetMessage();
                    tournamentMsg.Write((int)PacketType.TOURNAMENT_STARTING);
                    serverMgr.SendMessage(tournamentMsg, p);
                }

                // je potreba ostatnim hracum dat vedet, ze se spectator reconnectuje - aby mu vytvorili mining module
                NetOutgoingMessage plrReconnectedMsg = serverMgr.CreateNetMessage();
                plrReconnectedMsg.Write((int)PacketType.PLAYER_RECONNECTED);
                plrReconnectedMsg.Write(p.GetId());
                serverMgr.BroadcastMessage(plrReconnectedMsg, p);

                // reconnectujicimu hraci se posle hromadna zprava o ostatnich hracich
                NetOutgoingMessage outmsg = serverMgr.CreateAllPlayersDataMessage();
                serverMgr.SendMessage(outmsg, p);

                // na zacatku neposilame zadne asteroidy, protoze nevime jejich soucasnou pozici - posilaji se az nove
                // ale zaroven tato zprava iniciuje klienta, takze je nutna
                // TODO
                outmsg = serverMgr.CreateNetMessage();
                outmsg.Write((int)PacketType.ALL_ASTEROIDS);
                outmsg.Write(0);
                serverMgr.SendMessage(outmsg, p);

                // zprava ze muze zacit hra
                NetOutgoingMessage startMsg = serverMgr.CreateNetMessage();
                startMsg.Write((int)PacketType.START_GAME_RESPONSE);
                serverMgr.SendMessage(startMsg, p);
            }

            return IsRunning;
        }

        public bool CheckTournamentFinished(bool announce = false)
        {
            Player winner = matchManager.GetTournamentWinner();
            if (winner == null)
                return false;

            if (announce)
            {
                NetOutgoingMessage tournamentFinished = serverMgr.CreateNetMessage();
                tournamentFinished.Write((int)PacketType.TOURNAMENT_FINISHED);
                tournamentFinished.Write(winner.GetId());
                // vyhry a odehrane hry vsech hracu
                foreach (Player p in players)
                {
                    tournamentFinished.Write(p.GetId());
                    tournamentFinished.Write(p.Data.WonMatches);
                    tournamentFinished.Write(p.Data.PlayedMatches);
                }
                serverMgr.BroadcastMessage(tournamentFinished);
            }

            return true;
        }

        public void Update(float tpf)
        {
            if (!IsRunning)
                return;

            gameLevel.Update(tpf);
            totalTime += tpf;
        }

        public void PlayerLeft(Player p)
        {
            if (players.Contains(p))
                matchManager.OnPlayerLeave(p, IsRunning);
        }

        public void PlayerConnected(Player p)
        {
            matchManager.OnPlayerConnect(p, IsRunning);
        }

        public static int GetRequiredNumberOfMatches(int players, int rounds)
        {
            return FastMath.Factorial(players) * rounds / (2 * FastMath.Factorial(players - 2));
        }
    }
}
