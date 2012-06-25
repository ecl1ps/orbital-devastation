using System;
using Orbit.Core.Scene.Entities;
using System.Windows.Media;
using System.Diagnostics;
using Orbit.Core.Scene;
using Orbit.Core.Weapons;
using System.Windows;
using Orbit.Core.Utils;
using Orbit.Gui;
using Orbit.Core.Helpers;

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
        public string HashId { get; set; }
        public PlayerType PlayerType { get; set; }
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
        public float HookCooldown { get; set; }
        public int BaseIntegrity { get; set; }
        /// <summary>
        /// na klientovi pouzivat funkcni Player.SetGoldAndShow() nebo Player.AddGoldAndShow()
        /// </summary>
        public int Gold { get; set; }
        public bool LobbyReady { get; set; }
        public bool LobbyLeader { get; set; }
        public bool StartReady { get; set; }
        public int PlayedMatches { get; set; }
        public int WonMatches { get; set; }

        public PlayerData()
        {
            Score = 0;
            Gold = SharedDef.START_GOLD;
            PlayerType = PlayerType.HUMAN;
            BaseIntegrity = SharedDef.BASE_MAX_INGERITY;
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
            HookCooldown = SharedDef.HOOK_COOLDOWN;
        }
    }

    public class PlayerBaseLocation
    {
        private static Rect BASE_LEFT = new Rect(SharedDef.VIEW_PORT_SIZE.Width * 0.1, SharedDef.VIEW_PORT_SIZE.Height * 0.9,
                                                 SharedDef.VIEW_PORT_SIZE.Width * 0.3, SharedDef.VIEW_PORT_SIZE.Height * 0.1);

        private static Rect BASE_RIGHT = new Rect(SharedDef.VIEW_PORT_SIZE.Width * 0.6, SharedDef.VIEW_PORT_SIZE.Height * 0.9,
                                                  SharedDef.VIEW_PORT_SIZE.Width * 0.3, SharedDef.VIEW_PORT_SIZE.Height * 0.1);

        public static Rect GetBaseLocation(Player p)
        {
            return GetBaseLocation(p.Data.PlayerPosition);
        }

        public static Rect GetBaseLocation(PlayerPosition pos)
        {
            return pos == PlayerPosition.LEFT ? BASE_LEFT : BASE_RIGHT;
        }
    }
}
