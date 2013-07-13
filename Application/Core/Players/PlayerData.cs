using System;
using Orbit.Core.Scene.Entities;
using Microsoft.Xna.Framework;
using System.Diagnostics;
using Orbit.Core.Scene;
using Orbit.Core.Weapons;
using System.Windows;
using Orbit.Core.Utils;
using Orbit.Gui;
using Orbit.Core.Helpers;
using Lidgren.Network;

namespace Orbit.Core.Players
{
    /// <summary>
    /// PlayerData musi byt opravdu jen hodnoty, ktere definuji hrace, 
    /// nesmi byt nijak vazany na scenu a graficke objekty,
    /// naopak musi obsahovat vsechny dulezite hodnoty, jelikoz jsou posilany
    /// jako stav hrace (a mohou byt pozdeji i ukladany)
    /// </summary>
    public class PlayerData : ISendable
    {
        // UDAJE HRACE
        public int Id { get; set; }
        public string HashId { get; set; }
        public string Name { get; set; }
        public Color SpecialColor { get; set; }
        public Color PlayerColor { get; set; }
        public PlayerType PlayerType { get; set; }
        public BotType BotType { get; set; }

        /// <summary>
        /// na klientovi pouzivat funkcni Player.SetGoldAndShow() nebo Player.AddGoldAndShow()
        /// </summary>
        public int Gold { get; set; }
        public int Score { get; set; }
        public int MatchPoints { get; set; }

        // STATY
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
        public int HookMaxCatchedObjCount { get; set; }
        public int HookActivePullableWeight { get; set; }
        public float HookActivePullReachDistance { get; set; }

        public int BonusHeal { get; set; }

        public int BaseIntegrity { get; set; }
        public int MaxBaseIntegrity { get; set; }

        // UDAJE TURNAJE
        public bool LobbyLeader { get; set; }
        public int PlayedMatches { get; set; }
        public int WonMatches { get; set; }

        // UDAJE ZAPASU
        public bool LobbyReady { get; set; }
        public bool StartReady { get; set; }
        public bool Active { get; set; }
        public PlayerPosition PlayerPosition { get; set; }
        public Vector2 MiningModuleStartPos { get; set; }
        public int FriendlyPlayerId { get; set; }

        public PlayerData()
        {
            Reset();
            
            MatchPoints = 0;
            Score = 0;

            PlayerType = PlayerType.HUMAN;
            BotType = BotType.NONE;

            LobbyLeader = false;
            PlayedMatches = 0;
            WonMatches = 0;

            PlayerPosition = PlayerPosition.INVALID;
        }

        /// <summary>
        /// zde MUSI byt vsechny atributy, ktere se maji zresetovat pri konci jedne hry turnaje
        /// </summary>
        public void Reset()
        {
            Gold = SharedDef.START_GOLD;
            MatchPoints = 0;

            MaxBaseIntegrity = SharedDef.BASE_MAX_INGERITY;
            BaseIntegrity = MaxBaseIntegrity;

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
            HookMaxCatchedObjCount = SharedDef.HOOK_MAX_OBJ_COUNT;
            HookActivePullableWeight = SharedDef.HOOK_ACTIVE_PULLABLE_WEIGHT;
            HookActivePullReachDistance = SharedDef.HOOK_ACTIVE_PULL_REACH_DIST;

            BonusHeal = SharedDef.BONUS_HEAL;

            LobbyReady = false;
            StartReady = false;
            FriendlyPlayerId = 0;
        }

        public void WriteObject(NetOutgoingMessage msg)
        {
            msg.Write(Id);
            msg.Write(HashId);
            msg.Write(Name);
            msg.Write(SpecialColor);
            msg.Write(PlayerColor);
            msg.Write((byte)PlayerType);
            msg.Write((byte)BotType);

            msg.Write(Gold);
            msg.Write(MatchPoints);
            msg.Write(Score);

            msg.Write(MineCooldown);
            msg.Write(MineGrowthSpeed);
            msg.Write(MineStrength);
            msg.Write(MineFallingSpeed);

            msg.Write(BulletCooldown);
            msg.Write(BulletDamage);
            msg.Write(BulletSpeed);

            msg.Write(HookLenght);
            msg.Write(HookSpeed);
            msg.Write(HookCooldown);
            msg.Write(HookMaxCatchedObjCount);
            msg.Write(HookActivePullableWeight);
            msg.Write(HookActivePullReachDistance);

            msg.Write(BonusHeal);

            msg.Write(BaseIntegrity);
            msg.Write(MaxBaseIntegrity);

            msg.Write(LobbyLeader);
            msg.Write(PlayedMatches);
            msg.Write(WonMatches);

            msg.Write(LobbyReady);
            msg.Write(StartReady);
            msg.Write(Active);
            msg.Write((byte)PlayerPosition);
            msg.Write(MiningModuleStartPos);
            msg.Write(FriendlyPlayerId);
        }

        public void ReadObject(NetIncomingMessage msg)
        {
            Id = msg.ReadInt32();
            HashId = msg.ReadString();
            Name = msg.ReadString();
            SpecialColor = msg.ReadColor();
            PlayerColor = msg.ReadColor();
            PlayerType = (PlayerType)msg.ReadByte();
            BotType = (BotType)msg.ReadByte();

            Gold = msg.ReadInt32();
            MatchPoints = msg.ReadInt32();
            Score = msg.ReadInt32();

            MineCooldown = msg.ReadFloat();
            MineGrowthSpeed = msg.ReadFloat();
            MineStrength = msg.ReadFloat();
            MineFallingSpeed = msg.ReadInt32();

            BulletCooldown = msg.ReadFloat();
            BulletDamage = msg.ReadInt32();
            BulletSpeed = msg.ReadInt32();

            HookLenght = msg.ReadInt32();
            HookSpeed = msg.ReadInt32();
            HookCooldown = msg.ReadFloat();
            HookMaxCatchedObjCount = msg.ReadInt32();
            HookActivePullableWeight = msg.ReadInt32();
            HookActivePullReachDistance = msg.ReadFloat();

            BonusHeal = msg.ReadInt32();

            BaseIntegrity = msg.ReadInt32();
            MaxBaseIntegrity = msg.ReadInt32();

            LobbyLeader = msg.ReadBoolean();
            PlayedMatches = msg.ReadInt32();
            WonMatches = msg.ReadInt32();

            LobbyReady = msg.ReadBoolean();
            StartReady = msg.ReadBoolean();
            Active = msg.ReadBoolean();
            PlayerPosition = (PlayerPosition)msg.ReadByte();
            MiningModuleStartPos = msg.ReadVector();
            FriendlyPlayerId = msg.ReadInt32();
        }
    }

    public class PlayerBaseLocation
    {
        private static Rectangle BASE_LEFT = new Rectangle((int) (SharedDef.VIEW_PORT_SIZE.Width * 0.12), (int) (SharedDef.VIEW_PORT_SIZE.Height * 0.9),
                                                 (int) (SharedDef.VIEW_PORT_SIZE.Width * 0.26), (int) (SharedDef.VIEW_PORT_SIZE.Height * 0.1));

        private static Rectangle BASE_RIGHT = new Rectangle((int) (SharedDef.VIEW_PORT_SIZE.Width * 0.62), (int) (SharedDef.VIEW_PORT_SIZE.Height * 0.9),
                                                  (int) (SharedDef.VIEW_PORT_SIZE.Width * 0.26), (int) (SharedDef.VIEW_PORT_SIZE.Height * 0.1));

        public static Rectangle GetBaseLocation(Player p)
        {
            return GetBaseLocation(p.Data.PlayerPosition);
        }

        public static Rectangle GetBaseLocation(PlayerPosition pos)
        {
            return pos == PlayerPosition.LEFT ? BASE_LEFT : BASE_RIGHT;
        }
    }
}
