using Orbit.Core.Server.Match;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using Orbit.Core.Players;
using System.Collections.Generic;

namespace Tests
{
    /// <summary>
    ///This is a test class for WinsThenScoreMatchMakerTest and is intended
    ///to contain all WinsThenScoreMatchMakerTest Unit Tests
    ///</summary>
    [TestClass()]
    public class WinsThenScoreMatchMakerTest
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
        ///A test for GetPlayersWhoWonNumberOfMatches
        ///</summary>
        [TestMethod()]
        public void GetPlayersWhoWonNumberOfMatchesTest()
        {
            PrivateObject param0 = null; // TODO: Initialize to an appropriate value
            WinsThenScoreMatchMaker_Accessor target = new WinsThenScoreMatchMaker_Accessor(param0); // TODO: Initialize to an appropriate value
            int wins = 0; // TODO: Initialize to an appropriate value
            List<Player> expected = null; // TODO: Initialize to an appropriate value
            List<Player> actual;
            actual = target.GetPlayersWhoWonNumberOfMatches(wins);
            Assert.AreEqual(expected, actual);
            Assert.Inconclusive("Verify the correctness of this test method.");
        }

        /// <summary>
        ///A test for GetWinner
        ///</summary>
        [TestMethod()]
        public void GetWinnerTest()
        {
            List<Player> players = null; // TODO: Initialize to an appropriate value
            Random randGen = null; // TODO: Initialize to an appropriate value
            int rounds = 0; // TODO: Initialize to an appropriate value
            WinsThenScoreMatchMaker target = new WinsThenScoreMatchMaker(players, randGen, rounds); // TODO: Initialize to an appropriate value
            Player expected = null; // TODO: Initialize to an appropriate value
            Player actual;
            actual = target.GetWinner();
            Assert.AreEqual(expected, actual);
            Assert.Inconclusive("Verify the correctness of this test method.");
        }

        /// <summary>
        ///A test for SelectPlayersForNewMatch
        ///</summary>
        [TestMethod()]
        public void SelectPlayersForNewMatchTest()
        {
            List<Player> players = null; // TODO: Initialize to an appropriate value
            Random randGen = null; // TODO: Initialize to an appropriate value
            int rounds = 0; // TODO: Initialize to an appropriate value
            WinsThenScoreMatchMaker target = new WinsThenScoreMatchMaker(players, randGen, rounds); // TODO: Initialize to an appropriate value
            Tuple<Player, Player> expected = null; // TODO: Initialize to an appropriate value
            Tuple<Player, Player> actual;
            actual = target.SelectPlayersForNewMatch();
            Assert.AreEqual(expected, actual);
            Assert.Inconclusive("Verify the correctness of this test method.");
        }
    }
}
