using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Orbit.Core.Players;
using System.Windows;
using Lidgren.Network;

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
            Tuple<Player, Player> newPlayers = SelectPlayersBasic();
            if (newPlayers != null)
            {
                //pripravime spectatory
                List<Player> spectators = new List<Player>(players);
                spectators.Remove(newPlayers.Item1);
                spectators.Remove(newPlayers.Item2);
                AssignPlayersToSpectators(newPlayers, spectators);

                return newPlayers;
            }

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

        public override void AssignPlayersToSpectators(Tuple<Player, Player> active, List<Player> spectators)
        {
            int count = spectators.Count;
            //kdyz je zadny spectator nebo jenom jeden neprirazujeme hrace
            if (count > 1)
            {
                //sesortime podle score
                spectators.Sort(delegate(Player p1, Player p2) { return p2.Data.MatchPoints.CompareTo(p1.Data.MatchPoints); });

                Player weaker;
                Player stronger;
                //porovname jejich score, podle toho urcime kdo je lepsi / horsi
                int compared = active.Item1.Data.MatchPoints.CompareTo(active.Item2.Data.MatchPoints);
                //kdyz jsou na tom stejne tak to neresime (priradime stejne jako kdyz prvni je silnejsi)
                if (compared == 0 || compared > 1)
                {
                    stronger = active.Item2;
                    weaker = active.Item1;
                }
                else
                {
                    stronger = active.Item1;
                    weaker = active.Item2;
                }

                //kdyz je pocet spectatoru sudy priradime kazdemu hraci stejny pocet spectatoru
                if (count % 2 == 0)
                {
                    for (int i = 0; i < spectators.Count; i += 2)
                    {
                        //spectatorovi vejs (ma vetsi score) priradime slabsiho hrace
                        spectators[i].Data.FriendlyPlayerId = weaker.GetId();
                        weaker.Data.FriendlyPlayerId = spectators[i].GetId();
                        spectators[i + 1].Data.FriendlyPlayerId = stronger.GetId();
                        stronger.Data.FriendlyPlayerId = spectators[i + 1].GetId();
                    }
                }
                //lichy pocet = nejlepsi spectator bude solo
                else
                {
                    spectators[0].Data.FriendlyPlayerId = 0;

                    for (int i = 1; i < spectators.Count; i += 2)
                    {
                        spectators[i].Data.FriendlyPlayerId = weaker.GetId();
                        weaker.Data.FriendlyPlayerId = spectators[i].GetId();
                        spectators[i + 1].Data.FriendlyPlayerId = stronger.GetId();
                        stronger.Data.FriendlyPlayerId = spectators[i + 1].GetId();
                    }
                }
            }
        }

        public override void OnMatchEnd(Player plr, GameEnd endType, float time, ServerMgr server)
        {
            base.OnMatchEnd(plr, endType, time, server);
        }

        protected override void RewardPlayers(Player winner, Player looser, ServerMgr server)
        {
            winner.Data.WonMatches++;
            int scoreWin = ComputeScoreWinner(winner, looser, server.Time);
            int scoreLoose = ComputeScoreLooser(looser, winner, server.Time);

            winner.Data.Score += scoreWin;
            looser.Data.Score += scoreLoose;

            SendPlayerScore(winner, server);
            SendPlayerScore(looser, server);

            SendTextMessage(winner, "You are victorious. You get " + scoreWin + " score", server);
            SendTextMessage(looser, "You lost. You get " + scoreLoose + " score", server);

            foreach (Player p in players)
            {
                p.Data.MatchPoints = 0;
                if (!p.IsActivePlayer())
                    RewardSpectator(winner, p, scoreWin, scoreLoose, server.Time, server);
            }
        }

        protected void RewardSpectator(Player winner, Player spectator, int winnerScore, int looserScore, float totalTime, ServerMgr server)
        {
            //spectator hral nacas
            if (spectator.Data.FriendlyPlayerId == 0)
            {
                String time = ParseTime(totalTime);
                int val = (int) FastMath.LinearInterpolate(SharedDef.SOLO_SPECTATOR_MIN_SCORE, SharedDef.SOLO_SPECTATOR_MAX_SCORE, ComputeTimePerc(totalTime, SharedDef.SOLO_SPECTATOR_MAX_TIME, SharedDef.SOLO_SPECTATOR_MIN_TIME));
                spectator.Data.MatchPoints += val;
                SendTextMessage(spectator, "Thanks to your effort game lasted " + time + " . You acquire " + val + " score", server);
            }
            //spectator musel ochranovat hrace
            else if (spectator.Data.FriendlyPlayerId == winner.GetId())
            {
                int val = (int) (winnerScore * SharedDef.SPECTATOR_SCORE_BONUS);
                spectator.Data.Score += val;
                SendTextMessage(spectator, "You win. You acquire " + val + " score", server);
            }
            //spectator prohral
            else
            {
                int val = (int)(looserScore * SharedDef.SPECTATOR_SCORE_BONUS);
                spectator.Data.Score += val;
                SendTextMessage(spectator, "You lost. You acquire " + val + " score", server);
            }

            SendPlayerScore(spectator, server);
        }

        private int ComputeScoreWinner(Player me, Player other, float time)
        {
            double timeCoef = FastMath.LinearInterpolate(1, 0.5, ComputeTimePerc(time, SharedDef.MAX_TIME, SharedDef.MIN_TIME));
            float pointsCoef = other.Data.MatchPoints != 0 ? Math.Min(me.Data.MatchPoints / other.Data.MatchPoints, 1) : 1;
            return (int) (FastMath.LinearInterpolate(SharedDef.VICTORY_MIN_SCORE, SharedDef.VICTORY_MAX_SCORE, pointsCoef) * timeCoef);
        }

        private int ComputeScoreLooser(Player me, Player other, float time)
        {
            double timeCoef = FastMath.LinearInterpolate(0.5, 1, ComputeTimePerc(time, SharedDef.MAX_TIME, SharedDef.MIN_TIME));
            float pointsCoef = other.Data.MatchPoints != 0 ? Math.Min(me.Data.MatchPoints / other.Data.MatchPoints, 1) : 1;
            return (int)(FastMath.LinearInterpolate(SharedDef.LOOSE_MIN_SCORE, SharedDef.LOOSE_MAX_SCORE, pointsCoef) * timeCoef);
        }

        private double ComputeTimePerc(float time, float maxTime, float minTime)
        {
            if (time < SharedDef.MIN_TIME)
                return 0;
            else if (time > SharedDef.MAX_TIME)
                return 1;

            double val = (time - minTime) / (maxTime - minTime);
            if (val > 1)
                val = 1;
            else if (val < 0)
                val = 0;

            return val;
        }

        private String ParseTime(float value)
        {
            int min = (int)(value / 60);
            int sec = (int) value % 60;

            return min + ":" + sec;
        }
    }
}
