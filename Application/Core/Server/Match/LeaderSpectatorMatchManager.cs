using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Orbit.Core.Players;

namespace Orbit.Core.Server.Match
{
    /// <summary>
    /// testovaci MM, ktery vrati dva hrace, kteri nejsou leaderem (aby leader byl vzdy spectator)
    /// </summary>
    public class LeaderSpectatorMatchManager : ITournamentMatchManager
    {
        private List<Player> players;

        public LeaderSpectatorMatchManager(List<Player> players)
        {
            this.players = players;
        }

        public Tuple<Player, Player> SelectPlayersForNewMatch()
        {
            // zneaktivnime vsechny hrace
            foreach (Player p in players)
                p.Data.Active = false;

            if (players.Count < 3)
                throw new Exception("Not enough players -> you need at least 3 players so you can become spectator!");
            
            List<Player> nonLeaders = players.FindAll(p => p.Data.LobbyLeader == false);
            nonLeaders[0].Data.Active = true;
            nonLeaders[1].Data.Active = true;

            return new Tuple<Player, Player>(nonLeaders[0], nonLeaders[1]);
        }

        public void OnMatchEnd(Player plr, GameEnd endType)
        {
        }

        public Player GetWinner()
        {
            return null;
        }
    }
}
