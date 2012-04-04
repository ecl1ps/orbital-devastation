using System;
using Orbit.Core.Scene.Entities;
using System.Windows.Media;

namespace Orbit.Core.Player
{
    class PlayerData : IPlayerData
    {
        private Base myBase;
        private int score;
        private float mineGrowthSpeed;
        private float mineStrength;

        public PlayerData()
        {
            score = 0;
            mineGrowthSpeed = SharedDef.MINE_GROWTH_SPEED;
            mineStrength = SharedDef.MINE_STRENGTH;
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

        public float GetMineGrowthSpeed()
        {
            return mineGrowthSpeed;
        }

        public float GetMineStrength()
        {
            return mineStrength;
        }

        public Color GetPlayerColor()
        {
            return myBase.Color;
        }
    }
}
