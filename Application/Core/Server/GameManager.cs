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

namespace Orbit.Core.Server
{
    public class GameManager
    {
        private ServerMgr serverMgr;
        private List<Player> players;
        private List<ISceneObject> objects;

        public int Level { get; set; }
        public bool IsRunning { get; set; }

        public GameManager(ServerMgr serverMgr, List<Player> players)
        {
            this.serverMgr = serverMgr;
            this.players = players;
            //todo
            Level = 1;
        }

        public void CreateNewMatch()
        {
            // pri solo hre se vytvori jeden bot
            if (players.Count == 1)
                serverMgr.CreateAndAddPlayer("Bot");

            CreateNewLevel();

            foreach (Player p in players)
                p.Data.Active = false;

            // TODO: algoritmem vybrat hrace pro novy zapas
            Player plr1 = players[0];
            Player plr2 = players[1];
            // MatchMaker.SelectPlayersForNewMatch(players, out plr1, out plr2);

            plr1.Data.Active = true;
            plr2.Data.Active = true;


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

        private void SendMatchData(Player p = null)
        {
            // poslani vsech hracu
            NetOutgoingMessage outmsg = serverMgr.CreateAllPlayersDataMessage();
            if (p == null)
                serverMgr.BroadcastMessage(outmsg);
            else
                serverMgr.SendMessage(outmsg, p);

            // poslani vsech asteroidu
            outmsg = serverMgr.CreateNetMessage();
            outmsg = serverMgr.CreateNetMessage();
            outmsg.Write((int)PacketType.ALL_ASTEROIDS);

            Int32 count = 0;
            objects.ForEach(new Action<ISceneObject>(x => { if (x is Asteroid) count++; }));
            outmsg.Write(count);

            foreach (ISceneObject obj in objects)
                if (obj is Asteroid)
                    (obj as Asteroid).WriteObject(outmsg);

            if (p == null)
                serverMgr.BroadcastMessage(outmsg);
            else
                serverMgr.SendMessage(outmsg, p);
        }

        public bool RequestStartMatch(Player p)
        {
            // do tournamentu se nemuzou pridat dalsi hraci, kteri nebyli v lobby pri startu
            if (serverMgr.GameType == Gametype.LOBBY_GAME && !players.Exists(pl => pl.GetId() == p.GetId()))
                return false;

            p.Data.StartReady = true;

            if ((p.IsActivePlayer() && !IsRunning && players.Count(plr => plr.IsActivePlayer() && plr.Data.StartReady) == 2)
                || serverMgr.GameType == Gametype.LOBBY_GAME)
            {
                if (serverMgr.GameType == Gametype.LOBBY_GAME)
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
                if (serverMgr.GameType == Gametype.LOBBY_GAME)
                {
                    NetOutgoingMessage tournamentMsg = serverMgr.CreateNetMessage();
                    tournamentMsg.Write((int)PacketType.TOURNAMENT_STARTING);
                    serverMgr.SendMessage(tournamentMsg, p);
                }

                SendMatchData(p);

                NetOutgoingMessage startMsg = serverMgr.CreateNetMessage();
                startMsg.Write((int)PacketType.START_GAME_RESPONSE);
                serverMgr.SendMessage(startMsg, p);
            }

            return IsRunning;
        }
    }
}
