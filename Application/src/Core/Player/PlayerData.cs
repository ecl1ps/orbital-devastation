using System;
using Orbit.Core.Scene.Entities;
using System.Windows.Media;
using System.Diagnostics;

namespace Orbit.Core.Player
{
    public class PlayerData
    {
        private Base myBase;
        private int score;
        private float mineGrowthSpeed;
        private float mineStrength;
        private int mineCooldown;
        private Stopwatch mineTimer;

        public PlayerData()
        {
            score = 0;
            mineGrowthSpeed = SharedDef.MINE_GROWTH_SPEED;
            mineStrength = SharedDef.MINE_STRENGTH;
            mineTimer = new Stopwatch();
            mineTimer.Start();
            mineCooldown = SharedDef.MINE_COOLDOWN;
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

        public void UseMine()
        {
            mineTimer.Start();
        }

        public bool IsMineReady()
        {
            if (mineTimer.ElapsedMilliseconds > mineCooldown)
            {
                mineTimer.Reset();
                return true;
            }

            return false;
        }
    }
}
