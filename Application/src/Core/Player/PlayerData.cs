using System;
using Orbit.Core.Scene.Entities;
using System.Windows.Media;
using System.Diagnostics;
using Orbit.Core.Scene;

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
        public int BulletSpeed { get; set; }
        public int BulletCooldown { get; set; }
        public int BulletDamage { get; set; }
        public int HookLenght { get; set; }
        public int HookSpeed { get; set; }

        private int gold;
        public int Gold
        {
            get
            {
                return gold;
            }
            set
            {
                gold = value;
                proccesGoldData();
            }
        }

        private void proccesGoldData()
        {
            SceneMgr.GetInstance().ShowStatusText(3, "Gold: " + Gold);
        }
        
        public PlayerData()
        {
            Score = 0;
            Gold = 0;
            PlayerColor = Colors.Black;
            MineGrowthSpeed = SharedDef.MINE_GROWTH_SPEED;
            MineStrength = SharedDef.MINE_STRENGTH;
            MineCooldown = SharedDef.MINE_COOLDOWN;
            MineFallingSpeed = SharedDef.MINE_FALLING_SPEED;
            BulletSpeed = SharedDef.BULLET_SPEED;
            BulletCooldown = SharedDef.BULLET_COOLDOWN;
            BulletDamage = SharedDef.BULLET_DMG;
            HookLenght = SharedDef.HOOK_LENGHT;
            HookSpeed = SharedDef.HOOK_SPEED;
        }

    }
}
