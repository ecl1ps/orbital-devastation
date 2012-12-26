using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Orbit.Core.Players;

namespace Orbit.Core.Server.Match
{
    /// <summary>
    /// kazdy hraje s kazdym a kdyz potom neni jednoznacny vyherce (treba dva maji dve vyhry), tak rozhodne score
    /// </summary>
    public class WinsThenScoreMatchManager : AbstractTournamentMatchManager
    {
        private static readonly log4net.ILog Logger = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        public static readonly MatchManagerInfo Info = new MatchManagerInfo(false, "Each vs. Each (most wins then score)");

        public WinsThenScoreMatchManager(List<Player> players, Random randGen, int rounds)
            : base(players, randGen, rounds)
        {
        }

        public override Tuple<Player, Player> SelectPlayersForNewMatch()
        {
            // zneaktivnime vsechny hrace
            SetAllPlayersInactive();

            // zkusime najit hrace beznou cestou
            Tuple<Player, Player> newPlayes = SelectPlayersBasic();
            if (newPlayes != null)
                return newPlayes;

            // zde uz kazdy odehral s kazdym

            // skocime na dalsi kolo
            if (roundNumber++ == roundCount)
                throw new Exception("Error: players are trying to start round " + roundNumber + " but tournament is set for " + roundCount + "round(s)!");

            // a vynulujeme zaznamy kdo hral s kym
            data.ForEach(d => d.playedWith.Clear());

            // a hledame hrace znovu pro dalsi kolo
            return SelectPlayersForNewMatch();
        }

        /// <summary>
        /// vraci bud viteze nebo null, pokud vitez jeste neni
        /// </summary>
        public override Player GetTournamentWinner()
        {
            // jeste nejsme v poslednim kole
            if (roundNumber < roundCount)
                return null;

            // pokud nehral kazdy s kazdym
            if (!EveryOnePlayedWithNumberOfPlayers(players.Count - 1))
                return null;

            int maxWins = 0;
            data.ForEach(d => { if (d.WonGames > maxWins) maxWins = d.WonGames; });

            if (maxWins == 0)
                return null;

            // hraci, kteri se deli o prvni misto ve vyhranych zapasech
            List<Player> bestPlayers = GetPlayersWhoWonNumberOfMatches(maxWins);
            if (bestPlayers.Count == 1)
                return bestPlayers[0];
            else
            {
                // kdyz jich je vic, tak vyhrava hrac s nejvetsim score
                IOrderedEnumerable<Player> bp = bestPlayers.OrderByDescending(p => p.Data.Score);
                return bp.First();
            }
        }

        private List<Player> GetPlayersWhoWonNumberOfMatches(int wins)
        {
            List<Player> bestPlayers = new List<Player>();

            // vrati vsechny hrace, kteri maji wins vyher
            data.ForEach(d => { if (d.WonGames == wins) bestPlayers.Add(players.Find(p => p.Data.HashId == d.Owner)); });

            return bestPlayers;
        }

        public override bool HasRightNumberOfPlayersForStart()
        {
            return players.FindAll(p => p.Data.PlayerType == PlayerType.HUMAN).Count >= 2;
        }

        public override void OnPlayerLeave(Player plr, bool gameRunning)
        {
            PlayerMatchData d = GetPlayerMatchData(plr);
            if (d != null)
            {
                if (plr.IsActivePlayer() && gameRunning)
                {
                    Player otherPlr = players.Find(p => p.IsActivePlayer() && p.Data.Id != plr.Data.Id);
                    SetPlayedTogether(plr, otherPlr);
                    GetPlayerMatchData(otherPlr).WonGames++;
                    otherPlr.Data.WonMatches++;
                }
                d.IsOnline = false;
            }
            else
                Logger.Warn("Left player " + plr.Data.Name + " but has no data in tournament");
        }

        public override void OnPlayerConnect(Player plr, bool gameRunning)
        {
            PlayerMatchData d = GetPlayerMatchData(plr);
            if (d != null)
                d.IsOnline = true;
            else
                Logger.Warn("Reconnected player " + plr.Data.Name + " but has no data in tournament");
        }
    }
}
