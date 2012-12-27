using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Orbit.Core.Players;

namespace Orbit.Core.Server.Match
{
    /// <summary>
    /// manager ktery umoznuje hru vice nez dvou hracu a na vice kol s moznosti reconnectovat
    /// </summary>
    public abstract class AbstractTournamentMultiplayerMatchManager : AbstractTournamentMatchManager
    {
        private static readonly log4net.ILog Logger = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        public AbstractTournamentMultiplayerMatchManager(List<Player> players, Random randGen, int rounds)
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
            roundNumber++;

            // a vynulujeme zaznamy kdo hral s kym
            data.ForEach(d => d.playedWith.Clear());

            // a hledame hrace znovu pro dalsi kolo
            return SelectPlayersForNewMatch();
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
