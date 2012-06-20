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

namespace Orbit.Core.Server.Match
{
    public class GameManager
    {
        private ServerMgr serverMgr;
        private List<Player> players;
        private SortedSet<string> tournamentPlayerIds;
        private List<ISceneObject> objects;
        private ITournamentMatchMaker matchMaker;

        public int Level { get; set; }
        public bool IsRunning { get; set; }
        private bool matchCreated;

        public GameManager(ServerMgr serverMgr, List<Player> players)
        {
            this.serverMgr = serverMgr;
            this.players = players;
            matchMaker = new OneToAllThenScoreMatchMaker(players, serverMgr.GetRandomGenerator());
            tournamentPlayerIds = new SortedSet<string>();
            if (serverMgr.GameType == Gametype.TOURNAMENT_GAME)
                players.ForEach(p => tournamentPlayerIds.Add(p.Data.HashId));
            // TODO
            Level = 1;
            matchCreated = false;
        }

        public void CreateNewMatch()
        {
            if (matchCreated)
                return;

            matchCreated = true;

            // pri solo hre se vytvori jeden bot
            if (players.Count == 1)
            {
                Player bot = serverMgr.CreateAndAddPlayer("Bot", "NullBotHash");
                bot.Data.PlayerType = PlayerType.BOT;
                bot.Data.StartReady = true;
            }

            CreateNewLevel();

            foreach (Player p in players)
                p.Data.Active = false;

            Player plr1 = players[0];
            Player plr2 = players[1];

            if (serverMgr.GameType == Gametype.TOURNAMENT_GAME)
                matchMaker.SelectPlayersForNewMatch(out plr1, out plr2);

            plr1.Data.Active = true;
            plr2.Data.Active = true;

            plr1.Data.PlayedMatches += 1;
            plr2.Data.PlayedMatches += 1;

            PlayerPosition firstPlayerPosition = serverMgr.GetRandomGenerator().Next(2) == 0 ? PlayerPosition.LEFT : PlayerPosition.RIGHT;
            plr1.Data.PlayerPosition = firstPlayerPosition;
            plr2.Data.PlayerPosition = firstPlayerPosition == PlayerPosition.RIGHT ? PlayerPosition.LEFT : PlayerPosition.RIGHT;

            bool redBaseColor = serverMgr.GetRandomGenerator().Next(2) == 0 ? true : false;
            plr1.Data.PlayerColor = redBaseColor ? Colors.Red : Colors.Blue;
            plr2.Data.PlayerColor = redBaseColor ? Colors.Blue : Colors.Red;
        }

        private void CreateNewLevel()
        {
            objects = new List<ISceneObject>();

            switch (Level)
            {
                case 1:
                    CreateAsteroidField(SharedDef.ASTEROID_COUNT);
                    break;
                default:
                    break;
            }
        }

        public void GameEnded(Player plr, GameEnd endType)
        {
            IsRunning = false;
            matchCreated = false;

            if (serverMgr.GameType == Gametype.TOURNAMENT_GAME)
                matchMaker.MatchEnded(plr, endType);
        }

        private void CreateAsteroidField(int count)
        {
            for (int i = 0; i < count; ++i)
                objects.Add(ServerSceneObjectFactory.CreateNewRandomAsteroid(serverMgr, i % 2 == 0));
        }

        public void ObjectDestroyed(long id)
        {
            ISceneObject obj = objects.Find(o => o.Id == id);
            if (obj == null)
                return;

            objects.Remove(obj);

            NetOutgoingMessage msg = serverMgr.CreateNetMessage();

            if (obj is Asteroid)
            {
                obj = ServerSceneObjectFactory.CreateNewAsteroidOnEdge(serverMgr, (obj as Asteroid).IsHeadingRight);
                (obj as Asteroid).WriteObject(msg);
            }

            objects.Add(obj);
            serverMgr.BroadcastMessage(msg);
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
            Player winner = matchMaker.GetWinner();
            if (winner == null)
                return false;

            if (announce)
            {
                NetOutgoingMessage tournamentFinished = serverMgr.CreateNetMessage();
                tournamentFinished.Write((int)PacketType.TOURNAMENT_FINISHED);
                tournamentFinished.Write(winner.GetId());
                serverMgr.BroadcastMessage(tournamentFinished);
            }

            return true;
        }
    }
}
