using System;
using Orbit.Core.Scene.Entities;
using System.Windows.Media;
using System.Diagnostics;
using Orbit.Core.Scene;
using Orbit.Core.Weapons;
using System.Windows;

namespace Orbit.Core.Players
{
    public class PlayerData
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public bool Active { get; set; }
        public SceneMgr SceneMgr { get; set; }
        public PlayerPosition PlayerPosition { get; set; }
        public Color PlayerColor { get; set; }
        public int Score { get; set; }
        public float MineGrowthSpeed { get; set; }
        public float MineStrength { get; set; }
        public float MineCooldown { get; set; }
        public int MineFallingSpeed { get; set; }
        public int BulletSpeed { get; set; }
        public float BulletCooldown { get; set; }
        public int BulletDamage { get; set; }
        public int HookLenght { get; set; }
        public int HookSpeed { get; set; }

        public IWeapon Hook { get; set; }
        public IWeapon Mine { get; set; }
        public IWeapon Canoon { get; set; }

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

        public Vector VectorPosition
        {
            get
            {
                Vector vector = new Vector(SceneMgr.ViewPortSizeOriginal.Width, SceneMgr.ViewPortSizeOriginal.Height * 0.85);

                switch (PlayerPosition)
                {
                    case PlayerPosition.LEFT:
                        vector.X *= 0.1;
                        break;
                    case PlayerPosition.RIGHT:
                        vector.X *= 0.6;
                        break;
                    default:
                        return new Vector();
                }

                return vector;
            }
        }

        private void proccesGoldData()
        {
            if (Gold == 0)
                return;

            if (IsCurrentPlayer())
                SceneMgr.ShowStatusText(4, "Gold: " + Gold);
        }

        private bool IsCurrentPlayer()
        {
            return SceneMgr is SceneMgr && (SceneMgr as SceneMgr).GetCurrentPlayer().GetId() == Id;
        }

        public PlayerData(SceneMgr sceneMgr)
        {
            SceneMgr = sceneMgr;
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
