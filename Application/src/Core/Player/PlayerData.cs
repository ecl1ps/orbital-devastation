using System;
using Orbit.Core.Scene.Entities;
using System.Windows.Media;
using System.Diagnostics;
using Orbit.Core.Scene;
using Orbit.Core.Weapons;
using System.Windows;

namespace Orbit.Core.Players
{
    /// <summary>
    /// PlayerData musi byt opravdu jen hodnoty, ktere definuji hrace, 
    /// nesmi byt nijak vazany na scenu a graficke objekty,
    /// naopak musi obsahovat vsechny dulezite hodnoty, jelikoz jsou posilany
    /// jako stav hrace (a mohou byt pozdeji i ukladany)
    /// </summary>
    public class PlayerData
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public bool Active { get; set; }
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
        public int BaseIntegrity { get; set; }
        /// <summary>
        /// na klientovi pouzivat funkcni Player.SetGoldAndShow() nebo Player.AddGoldAndShow()
        /// </summary>
        public int Gold { get; set; }

        public PlayerData()
        {
            Score = 0;
            Gold = 0;
            BaseIntegrity = 100;
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
