using System;
using Orbit.Core.Scene.Entities;
using System.Windows.Media;
using System.Diagnostics;
using Lidgren.Network;

namespace Orbit.Core.Players
{
    public class Player
    {
        public PlayerData Data { get; set; }
        public NetConnection Connection { get; set; }
        public Base Baze  { get; set; }

        public SceneObject hook { get; set; }

        private Stopwatch mineTimer;
        private Stopwatch gunTimer;

        public Player()
        {
            mineTimer = new Stopwatch();
            mineTimer.Start();

            gunTimer = new Stopwatch();
            gunTimer.Start();
        }

        public int GetBaseIntegrity()
        {
            return Baze.Integrity;
        }

        public void UpdateBaseIntegrity(int amount)
        {
            Baze.Integrity += amount;
        }

        public void UpdateScore(int amount)
        {
            Data.Score += amount;
        }

        public PlayerPosition GetPosition()
        {
            return Data.PlayerPosition;
        }

        public Color GetPlayerColor()
        {
            return Data.PlayerColor;
        }

        public void UseMine()
        {
            mineTimer.Start();
        }

        public void UseGun()
        {
            gunTimer.Start();
        }

        public bool IsMineReady()
        {
            if (mineTimer.ElapsedMilliseconds > Data.MineCooldown)
            {
                mineTimer.Reset();
                return true;
            }

            return false;
        }

        public Boolean IsGunReady()
        {
            if (gunTimer.ElapsedMilliseconds > Data.BulletCooldown)
            {
                gunTimer.Reset();
                return true;
            }

            return false;
        }
    }
}
