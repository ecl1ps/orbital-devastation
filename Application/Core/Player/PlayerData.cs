using System;
using Orbit.Scene.Entities;

namespace Orbit.Core.Player
{
    class PlayerData : IPlayerData
    {
        private Base myBase;
        private int score;

        public PlayerData()
        {
            score = 0;
        }

        public int GetScore()
        {
            return score;
        }

        public void UpdateScore(int amount)
        {
            score += amount;
        }

        public int GetBaseIntegrity()
        {
            return myBase.Integrity;
        }

        public void UpdateBaseIntegrity(int amount)
        {
            myBase.Integrity += amount;
        }

        public PlayerPosition GetPosition()
        {
            return myBase.BasePosition;
        }

        public void SetBase(Base newBase) 
        {
            myBase = newBase;
        }
    }
}
