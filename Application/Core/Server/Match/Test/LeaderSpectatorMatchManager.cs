﻿using System;
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

            List<Player> nonLeaders = players.FindAll(p => p.Data.LobbyLeader == false);
            if (nonLeaders.Count > 0)
                nonLeaders[0].Data.Active = true;
            if (nonLeaders.Count > 1)
                nonLeaders[1].Data.Active = true;

            return new Tuple<Player, Player>(nonLeaders.Count > 0 ? nonLeaders[0] : null, nonLeaders.Count > 1 ? nonLeaders[1] : null);
        }

        public void OnMatchEnd(Player plr, GameEnd endType)
        {
        }

        public Player GetWinner()
        {
            return null;
        }

        public virtual bool HasRightNumberOfPlayersForStart()
        {
            return true;
        }
    }
}