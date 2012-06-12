using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Orbit.Core.Players;
using System.Windows.Media;
using Orbit.Core.Scene;

namespace Orbit.Server
{
    public class MatchMaker
    {
        private ServerMgr serverMgr;
        private List<Player> players;

        public MatchMaker(ServerMgr serverMgr, List<Player> players)
        {
            this.serverMgr = serverMgr;
            this.players = players;
        }

        public void CreateNewMatch()
        {
            foreach (Player p in players)
                p.Data.Active = false;

            players[0].Data.Active = true;
            players[1].Data.Active = true;


            PlayerPosition firstPlayerPosition = serverMgr.GetRandomGenerator().Next(2) == 0 ? PlayerPosition.LEFT : PlayerPosition.RIGHT;
            players[0].Data.PlayerPosition = firstPlayerPosition;
            players[1].Data.PlayerPosition = firstPlayerPosition == PlayerPosition.RIGHT ? PlayerPosition.LEFT : PlayerPosition.RIGHT;

            bool redBaseColor = serverMgr.GetRandomGenerator().Next(2) == 0 ? true : false;
            players[0].Data.PlayerColor = redBaseColor ? Colors.Red : Colors.Blue;
            players[1].Data.PlayerColor = redBaseColor ? Colors.Blue : Colors.Red;

            /*players[0].Baze = SceneObjectFactory.CreateBase(serverMgr, players[0].Data);
            serverMgr.AttachToScene(players[0].Baze);
            players[1].Baze = SceneObjectFactory.CreateBase(serverMgr, players[1].Data);
            serverMgr.AttachToScene(players[1].Baze);*/
        }
    }
}
