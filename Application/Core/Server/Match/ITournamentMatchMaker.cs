using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Orbit.Core.Players;

namespace Orbit.Core.Server.Match
{
    // kazdy s kazdym
    // pri sudem poctu po dvojicich a pak vitezove
    // pri lichem dvojice a jedna trojice (kazdy s kazdym)
    public interface ITournamentMatchMaker
    {
        void SelectPlayersForNewMatch(out Player plr1, out Player plr2);

        void MatchEnded(Player plr, GameEnd endType);

        Player GetWinner();
    }
}
