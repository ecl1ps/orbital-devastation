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

namespace Orbit.Server
{
    public class MatchMaker
    {
        private ServerMgr serverMgr;
        private List<Player> players;
        private List<ISceneObject> objects;
        public int Level { get; set; }

        public MatchMaker(ServerMgr serverMgr, List<Player> players)
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
                serverMgr.CreatePlayer("Bot");

            CreateNewLevel();

            foreach (Player p in players)
                p.Data.Active = false;

            // TODO: algoritmem vybrat hrace pro novy zapas
            Player plr1 = players[0];
            Player plr2 = players[1];

            plr1.Data.Active = true;
            plr2.Data.Active = true;


            PlayerPosition firstPlayerPosition = serverMgr.GetRandomGenerator().Next(2) == 0 ? PlayerPosition.LEFT : PlayerPosition.RIGHT;
            plr1.Data.PlayerPosition = firstPlayerPosition;
            plr2.Data.PlayerPosition = firstPlayerPosition == PlayerPosition.RIGHT ? PlayerPosition.LEFT : PlayerPosition.RIGHT;

            bool redBaseColor = serverMgr.GetRandomGenerator().Next(2) == 0 ? true : false;
            plr1.Data.PlayerColor = redBaseColor ? Colors.Red : Colors.Blue;
            plr2.Data.PlayerColor = redBaseColor ? Colors.Blue : Colors.Red;

            SendMatchData();
        }

        private void CreateNewLevel()
        {
            objects = new List<ISceneObject>();

            switch (Level)
            {
                case 1:
                    CreateAsteroidField(SharedDef.ASTEROID_COUNT);
                    break;
            }
        }

        private void CreateAsteroidField(int count)
        {
            /*for (int i = 0; i < count; ++i)
                objects.Add(SceneObjectFactory.CreateNewRandomAsteroid(this, i % 2 == 0));*/
        }

        private void SendMatchData()
        {
            // poslani dat hracu
            NetOutgoingMessage outmsg = serverMgr.CreateNetMessage();

            outmsg.Write((int)PacketType.ALL_PLAYER_DATA);
            outmsg.Write(players.Count);

            foreach (Player plr in players)
                outmsg.WriteObjectPlayerData(plr.Data);

            serverMgr.BroadcastMessage(outmsg);

            // poslani vsech asteroidu
            outmsg = serverMgr.CreateNetMessage();
            outmsg.Write((int)PacketType.ALL_ASTEROIDS);

            Int32 count = 0;
            objects.ForEach(new Action<ISceneObject>(x => { if (x is Asteroid) count++; }));
            outmsg.Write(count);

            foreach (ISceneObject obj in objects)
                if (obj is Asteroid)
                    (obj as Asteroid).WriteObject(outmsg);

            serverMgr.BroadcastMessage(outmsg);
            
        }
    }
}
