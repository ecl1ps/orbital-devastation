using Orbit.Core.Server.Match;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using Orbit.Core.Players;
using System.Collections.Generic;
using Orbit.Core;
using Orbit.Core.Server;

namespace Tests
{
    /// <summary>
    ///This is a test class for WinsThenScoreMatchManagerTest and is intended
    ///to contain all WinsThenScoreMatchManagerTest Unit Tests
    ///</summary>
    [TestClass()]
    public class WinsThenScoreMatchManagerTest
    {
        #region Additional test attributes
        // 
        //You can use the following additional attributes as you write your tests:
        //
        //Use ClassInitialize to run code before running the first test in the class
        //[ClassInitialize()]
        //public static void MyClassInitialize(TestContext testContext)
        //{
        //}
        //
        //Use ClassCleanup to run code after all tests in a class have run
        //[ClassCleanup()]
        //public static void MyClassCleanup()
        //{
        //}
        //
        //Use TestInitialize to run code before running each test
        //[TestInitialize()]
        //public void MyTestInitialize()
        //{
        //}
        //
        //Use TestCleanup to run code after each test has run
        //[TestCleanup()]
        //public void MyTestCleanup()
        //{
        //}
        //
        #endregion

        /// <summary>
        /// test obvykleho prubehu turnaje - nikdo neleavuje, nikdo se nepripojuje zpet
        ///</summary>
        [TestMethod()]
        public void TestRegularTournaments()
        {
            RunRegularTournament(2, 1);
            RunRegularTournament(2, 2);
            RunRegularTournament(2, 3);
            RunRegularTournament(2, 6);
            RunRegularTournament(3, 1);
            RunRegularTournament(3, 2);
            RunRegularTournament(3, 3);
            RunRegularTournament(3, 4);
            RunRegularTournament(3, 5);
            RunRegularTournament(4, 1);
            RunRegularTournament(4, 2);
            RunRegularTournament(4, 3);
            RunRegularTournament(4, 4);
            RunRegularTournament(4, 5);
            RunRegularTournament(5, 1);
            RunRegularTournament(5, 2);
            RunRegularTournament(5, 3);
            RunRegularTournament(5, 4);
            RunRegularTournament(5, 5);
            RunRegularTournament(6, 1);
            RunRegularTournament(6, 2);
            RunRegularTournament(6, 3);
            RunRegularTournament(6, 4);
            RunRegularTournament(6, 5);
            RunRegularTournament(6, 13);
        }

        private void RunRegularTournament(int playerCount, int rounds)
        {
            List<Player> players = CreatePlayers(playerCount);

            WinsThenScoreMatchManager mgr = new WinsThenScoreMatchManager(players, new Random(), rounds);

            int requiredMatches = GameManager.GetRequiredNumberOfMatches(players.Count, rounds);
            for (int i = 1; i <= requiredMatches; ++i)
            {
                Tuple<Player, Player> selected = mgr.SelectPlayersForNewMatch();

                Assert.IsNotNull(selected.Item1, "selected player can't be null");
                Assert.IsNotNull(selected.Item2, "selected player can't be null");
                Assert.AreNotEqual(selected.Item1, selected.Item2, "selected players can't be the same");

                mgr.OnMatchEnd(selected.Item1, GameEnd.WIN_GAME);

                Player winner = mgr.GetTournamentWinner();
                if (i != requiredMatches)
                    Assert.IsNull(winner, "winner can't be know until last match");
                else
                {
                    Assert.IsNotNull(winner, "winner must be known after last match");
                    int maxWins = 0;
                    players.ForEach(d => { if (d.Data.WonMatches > maxWins) maxWins = d.Data.WonMatches; });
                    Assert.IsTrue(maxWins == winner.Data.WonMatches, "winner must the one with most wins");
                }
            }
        }

        private List<Player> CreatePlayers(int count)
        {
            List<Player> plrs = new List<Player>(count);
            for (int i = 1; i <= count; ++i)
                plrs.Add(CreatePlayer(i));
            return plrs;
        }

        private Player CreatePlayer(int id)
        {
            Player plr = new Player(null);
            plr.Data = new PlayerData();
            plr.Data.Id = id;
            plr.Data.Name = plr.GetId().ToString();
            plr.Data.HashId = plr.GetId().ToString();
            return plr;
        }

        /// <summary>
        /// test prubehu turnaje kdy nejaky hrac leavne uprostred zapasu
        /// </summary>
        [TestMethod()]
        public void TestTournamentsWithLeave()
        {
            RunTournamentWithPlayerLeaveDuringFight(3, 1, 1, 3);
            RunTournamentWithPlayerLeaveDuringFight(3, 1, 1, 2);
            RunTournamentWithPlayerLeaveDuringFight(3, 2, 2, 3);
            RunTournamentWithPlayerLeaveDuringFight(3, 2, 1, 2);
            RunTournamentWithPlayerLeaveDuringFight(3, 3, 1, 3);
            RunTournamentWithPlayerLeaveDuringFight(3, 3, 3, 1);
        }

        private void RunTournamentWithPlayerLeaveDuringFight(int playerCount, int rounds, int leaveAtMatch, int leaverId)
        {
            List<Player> players = CreatePlayers(playerCount);

            WinsThenScoreMatchManager mgr = new WinsThenScoreMatchManager(players, new Random(), rounds);

            int requiredMatches = GameManager.GetRequiredNumberOfMatches(players.Count, rounds);
            for (int i = 1; i <= requiredMatches; ++i)
            {
                Tuple<Player, Player> selected = mgr.SelectPlayersForNewMatch();

                Assert.IsNotNull(selected.Item1, "selected player can't be null");
                Assert.IsNotNull(selected.Item2, "selected player can't be null");
                Assert.AreNotEqual(selected.Item1, selected.Item2, "selected players can't be the same");

                if (i == leaveAtMatch)
                    mgr.OnPlayerLeave(players[leaverId - 1], true);

                mgr.OnMatchEnd(selected.Item1, GameEnd.WIN_GAME);

                Player winner = mgr.GetTournamentWinner();
                if (winner != null)
                {
                    int maxWins = 0;
                    players.ForEach(d => { if (d.Data.WonMatches > maxWins) maxWins = d.Data.WonMatches; });
                    Assert.IsTrue(maxWins == winner.Data.WonMatches, "winner must the one with most wins");
                    Console.Error.WriteLine("players: " + playerCount + " rounds: " + rounds + " left " + leaverId + " at round " + leaveAtMatch + 
                        " won: " + winner.GetId() + " with " + winner.Data.WonMatches + " wins");
                    return;
                }
            }
            Assert.Fail("winner must be found at some point.");
        }

        /// <summary>
        /// test prubehu turnaje kdy nejaky hrac leavne po zapasu turnaje
        /// </summary>
        [TestMethod()]
        public void TestTournamentsWithLeaveAtLobby()
        {
            RunTournamentWithPlayerLeaveAftergFight(3, 1, 1, 3);
            RunTournamentWithPlayerLeaveAftergFight(3, 1, 1, 2);
            RunTournamentWithPlayerLeaveAftergFight(3, 2, 2, 3);
            RunTournamentWithPlayerLeaveAftergFight(3, 2, 1, 2);
            RunTournamentWithPlayerLeaveAftergFight(3, 3, 1, 3);
            RunTournamentWithPlayerLeaveAftergFight(3, 3, 3, 1);
        }

        private void RunTournamentWithPlayerLeaveAftergFight(int playerCount, int rounds, int leaveAfterMatch, int leaverId)
        {
            List<Player> players = CreatePlayers(playerCount);

            WinsThenScoreMatchManager mgr = new WinsThenScoreMatchManager(players, new Random(), rounds);

            int requiredMatches = GameManager.GetRequiredNumberOfMatches(players.Count, rounds);
            for (int i = 1; i <= requiredMatches; ++i)
            {
                if (i == leaveAfterMatch + 1)
                    mgr.OnPlayerLeave(players[leaverId - 1], true);

                Tuple<Player, Player> selected = mgr.SelectPlayersForNewMatch();

                Assert.IsNotNull(selected.Item1, "selected player can't be null");
                Assert.IsNotNull(selected.Item2, "selected player can't be null");
                Assert.AreNotEqual(selected.Item1, selected.Item2, "selected players can't be the same");

                mgr.OnMatchEnd(selected.Item1, GameEnd.WIN_GAME);

                Player winner = mgr.GetTournamentWinner();
                if (winner != null)
                {
                    int maxWins = 0;
                    players.ForEach(d => { if (d.Data.WonMatches > maxWins) maxWins = d.Data.WonMatches; });
                    Assert.IsTrue(maxWins == winner.Data.WonMatches, "winner must the one with most wins");
                    Console.Error.WriteLine("players: " + playerCount + " rounds: " + rounds + " left " + leaverId + " after round " + leaveAfterMatch +
                        " won: " + winner.GetId() + " with " + winner.Data.WonMatches + " wins");
                    return;
                }
            }
            Assert.Fail("winner must be found at some point.");
        }
    }
}
