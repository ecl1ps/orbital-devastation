using System;
using Orbit.Core.Scene.Entities;
using System.Windows.Media;
using System.Diagnostics;

namespace Orbit.Core.Players
{
    public class PlayerData
    {
        public PlayerPosition PlayerPosition { get; set; }
        public Color PlayerColor { get; set; }
        public int Score { get; set; }
        public float MineGrowthSpeed { get; set; }
        public float MineStrength { get; set; }
        public int MineCooldown { get; set; }
        public int MineFallingSpeed { get; set; }
        private int BulletSpeed { get; set; }
        private int BulletCooldown { get; set; }
        
        public PlayerData()
        {
            Score = 0;
            PlayerColor = Colors.Black;
            MineGrowthSpeed = SharedDef.MINE_GROWTH_SPEED;
            MineStrength = SharedDef.MINE_STRENGTH;
            MineCooldown = SharedDef.MINE_COOLDOWN;
            MineFallingSpeed = SharedDef.MINE_FALLING_SPEED;
        }

    }
}
