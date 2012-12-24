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
    public class LeaderSpectatorMatchMaker : ITournamentMatchMaker
    {
        private List<Player> players;

        public LeaderSpectatorMatchMaker(List<Player> players)
        {
            this.players = players;
        }

        public Tuple<Player, Player> SelectPlayersForNewMatch()
        {
            if (players.Count < 3)
                throw new Exception("Not enough players -> you need at least 3 players so you can become spectator!");
            
            List<Player> nonLeaders = players.FindAll(p => p.Data.LobbyLeader == false);

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
