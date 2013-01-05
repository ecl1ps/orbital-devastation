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
                spectators.Sort(delegate(Player p1, Player p2) { return p1.Data.Score.CompareTo(p2.Data.Score); });

                Player weaker;
                Player stronger;
                //porovname jejich score, podle toho urcime kdo je lepsi / horsi
                int compared = active.Item1.Data.Score.CompareTo(active.Item2.Data.Score);
                //kdyz jsou na tom stejne tak to neresime (priradime stejne jako kdyz prvni je silnejsi)
                if (compared == 0 || compared > 1)
                {
                    stronger = active.Item1;
                    weaker = active.Item2;
                }
                else
                {
                    stronger = active.Item2;
                    weaker = active.Item1;
                }

                //kdyz je pocet spectatoru sudy priradime kazdemu hraci stejny pocet spectatoru
                if (count % 2 == 0)
                {
                    for (int i = 0; i < spectators.Count; i += 2)
                    {
                        //spectatorovi vejs (ma vetsi score) priradime slabsiho hrace
                        spectators[i].Data.FriendlyPlayerId = weaker.GetId();
                        spectators[i + 1].Data.FriendlyPlayerId = stronger.GetId();
                    }
                }
                //lichy pocet = nejlepsi spectator bude solo
                else
                {
                    for (int i = 1; i < spectators.Count; i += 2)
                    {
                        spectators[i].Data.FriendlyPlayerId = weaker.GetId();
                        spectators[i + 1].Data.FriendlyPlayerId = stronger.GetId();
                    }
                }
            }
        }

        public override void OnMatchEnd(Player plr, GameEnd endType, float time, ServerMgr server)
        {
            base.OnMatchEnd(plr, endType, time, server);

            //kdyz vyhrajeme tak dostanu viteze
            if (endType == GameEnd.WIN_GAME)
            {
                foreach (Player p in players)
                    if (!p.IsActivePlayer())
                        RewardSpectator(plr, p, time, server);
            }
        }

        private void RewardSpectator(Player winner, Player spectator, float totalTime, ServerMgr server)
        {
            //spectator hral nacas
            if (spectator.Data.FriendlyPlayerId == 0)
            {
                String time = ParseTime(totalTime);
                int val = (int) (FastMath.LinearInterpolate(0, SharedDef.SPECTATOR_WIN_BONUS, totalTime / SharedDef.SPECTATOR_MAX_TIME_BONUS) * SharedDef.SOLO_SPECTATOR_WIN_MULTIPLY);
                spectator.Data.Score += val;
                SendFloatingTextMessage(spectator, "Thanks to your effort game lasted " + time + " . You acquire " + val + " score", server);
            }
            //spectator musel ochranovat hrace
            else if (spectator.Data.FriendlyPlayerId == winner.GetId())
            {
                int val = SharedDef.SPECTATOR_WIN_BONUS;
                spectator.Data.Score += val;
                SendFloatingTextMessage(spectator, "You win. You acquire " + val + " score", server);
            }
            //spectator prohral
            else
            {
                SendFloatingTextMessage(spectator, "You loose", server);
            }

            SendPlayerScore(spectator, server);
        }

        private String ParseTime(float value)
        {
            int min = (int)(value / 60);
            int sec = (int) value % 60;

            return min + ":" + sec;
        }
    }
}
