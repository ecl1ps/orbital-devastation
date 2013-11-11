using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Orbit.Core.Players;

namespace Orbit.Core.Server.Match
{
    /// <summary>
    /// skirmish hra - 1 hrac a 1 bot
    /// </summary>
    public class SkirmishMatchManager : AbstractTournamentMatchManager
    {
        public static MatchManagerInfo Info = new MatchManagerInfo(true, Strings.mm_type_one_player);

        public SkirmishMatchManager(List<Player> players, Random randGen, int rounds)
            : base(players, randGen, rounds)
        {
        }

        public override Tuple<Player, Player> SelectPlayersForNewMatch()
        {
            players[0].Data.Active = true;
            if (players.Count > 1)
                players[1].Data.Active = true;
            return new Tuple<Player, Player>(players[0], players.Count > 1 ? players[1] : null);
        }

        public override Player GetTournamentWinner()
        {
            return players.Find(p => p.Data.WonMatches == 1);
        }

        public override bool HasRightNumberOfPlayersForStart()
        {
            return players.FindAll(p => p.Data.PlayerType == PlayerType.HUMAN).Count == 1;
        }
    }
}
