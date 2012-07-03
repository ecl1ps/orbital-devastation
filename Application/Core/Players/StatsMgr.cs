﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Orbit.Core.Weapons;
using Orbit.Core.Client;
using Lidgren.Network;
using System.Windows;

namespace Orbit.Core.Players
{ 
    public class StatsMgr
    {
        private SceneMgr sceneMgr;
        private Dictionary<PlayerStats, Stat> allStats = new Dictionary<PlayerStats, Stat>();


        public StatsMgr(SceneMgr mgr)
        {
            sceneMgr = mgr;

            allStats.Add(PlayerStats.MINE_1_COOLDOWN, new Stat(UpgradeLevel.LEVEL1, PlayerStats.MINE_1_COOLDOWN, -0.1f, -0.3f));
            allStats.Add(PlayerStats.MINE_1_FALLING_SPEED, new Stat(UpgradeLevel.LEVEL1, PlayerStats.MINE_1_FALLING_SPEED, +10f, +30f));
            allStats.Add(PlayerStats.MINE_1_GROWTH_SPEED, new Stat(UpgradeLevel.LEVEL1, PlayerStats.MINE_1_GROWTH_SPEED, +0.1f, +0.3f));
            allStats.Add(PlayerStats.MINE_1_STRENGTH, new Stat(UpgradeLevel.LEVEL1, PlayerStats.MINE_1_STRENGTH, +10f, +30f));

            allStats.Add(PlayerStats.BULLET_1_COOLDOWN, new Stat(UpgradeLevel.LEVEL1, PlayerStats.BULLET_1_COOLDOWN, -0.03f, -0.07f));
            allStats.Add(PlayerStats.BULLET_1_DAMAGE, new Stat(UpgradeLevel.LEVEL1, PlayerStats.BULLET_1_DAMAGE, +1f, +2f));
            allStats.Add(PlayerStats.BULLET_1_SPEED, new Stat(UpgradeLevel.LEVEL1, PlayerStats.BULLET_1_SPEED, +30f, +100f));

            allStats.Add(PlayerStats.HOOK_1_SPEED, new Stat(UpgradeLevel.LEVEL1, PlayerStats.HOOK_1_SPEED, +20f, +40f));
            allStats.Add(PlayerStats.HOOK_1_LENGTH, new Stat(UpgradeLevel.LEVEL1, PlayerStats.HOOK_1_LENGTH, +40f, +80f));
            allStats.Add(PlayerStats.HOOK_1_COOLDOWN, new Stat(UpgradeLevel.LEVEL1, PlayerStats.HOOK_1_COOLDOWN, -0.1f, -0.3f));

            allStats.Add(PlayerStats.HEALING_KIT_1_REPAIR_BASE, new Stat(UpgradeLevel.LEVEL1, PlayerStats.HEALING_KIT_1_REPAIR_BASE, +15f, +25f));
            allStats.Add(PlayerStats.HEALING_KIT_1_FORTIFY_BASE, new Stat(UpgradeLevel.LEVEL1, PlayerStats.HEALING_KIT_1_FORTIFY_BASE, +10f, +20f));
        }
        
        public void OnPlayerCaughtPowerUp(Player plr, DeviceType type)
        {
            if (!plr.IsCurrentPlayer() && !plr.IsBot())
                return;

            Stat pickedStat = GetStatForDeviceTypeAndLevel(type, GetUpgradeLevel(plr, type));

            float addedValue = GenerateAndAddStatToPlayer(pickedStat, plr.Data);

            NetOutgoingMessage msg = sceneMgr.CreateNetMessage();
            msg.Write((int)PacketType.PLAYER_RECEIVED_POWERUP);
            msg.Write(plr.GetId());
            msg.Write((byte)pickedStat.type);
            msg.Write(addedValue);
            sceneMgr.SendMessage(msg);

        }

        private float GenerateAndAddStatToPlayer(Stat stat, PlayerData data)
        {
            // ziskame rozmezi min - max
            float val = (float)sceneMgr.GetRandomGenerator().NextDouble() * (stat.max - stat.min) + stat.min;
#if DEBUG
            Console.WriteLine("Added stat " + stat.type + " (" + val + ") to player " + data.Name);
#endif
            AddStatToPlayer(data, stat.type, val);

            return val;
        }

        public void AddStatToPlayer(PlayerData data, PlayerStats type, float val)
        {
            switch (type)
            {
                case PlayerStats.MINE_1_COOLDOWN:
                    data.MineCooldown += val;
                    break;
                case PlayerStats.MINE_1_FALLING_SPEED:
                    data.MineFallingSpeed += (int)val;
                    break;
                case PlayerStats.MINE_1_GROWTH_SPEED:
                    data.MineGrowthSpeed += val;
                    break;
                case PlayerStats.MINE_1_STRENGTH:
                    data.MineStrength += val;
                    break;

                case PlayerStats.BULLET_1_COOLDOWN:
                    data.BulletCooldown += val;
                    break;
                case PlayerStats.BULLET_1_DAMAGE:
                    data.BulletDamage += (int)val;
                    break;
                case PlayerStats.BULLET_1_SPEED:
                    data.BulletSpeed += (int)val;
                    break;

                case PlayerStats.HOOK_1_COOLDOWN:
                    data.HookCooldown += val;
                    break;
                case PlayerStats.HOOK_1_LENGTH:
                    data.HookLenght += (int)val;
                    break;
                case PlayerStats.HOOK_1_SPEED:
                    data.HookSpeed += (int)val;
                    break;

                case PlayerStats.HEALING_KIT_1_REPAIR_BASE:
                    if (sceneMgr != null)
                        sceneMgr.GetPlayer(data.Id).ChangeBaseIntegrity((int)val, true);
                    else
                    {
                        data.BaseIntegrity += (int)val;
                        if (data.BaseIntegrity > data.MaxBaseIntegrity)
                            data.BaseIntegrity = data.MaxBaseIntegrity;
                    }
                    break;
                case PlayerStats.HEALING_KIT_1_FORTIFY_BASE:
                    data.MaxBaseIntegrity += (int)val;
                    if (sceneMgr != null)
                        sceneMgr.GetPlayer(data.Id).ChangeBaseIntegrity((int)val, true);
                    else
                    {
                        data.BaseIntegrity += (int)val;
                        if (data.BaseIntegrity > data.MaxBaseIntegrity)
                            data.BaseIntegrity = data.MaxBaseIntegrity;
                    }
                    break;
            }
        }

        private Stat GetStatForDeviceTypeAndLevel(DeviceType type, UpgradeLevel upgradeLevel)
        {
            switch (type)
            {
                case DeviceType.MINE:
                    switch (upgradeLevel)
                    {
                        case UpgradeLevel.LEVEL1:
                        case UpgradeLevel.LEVEL2:
                        case UpgradeLevel.LEVEL3:
                        default:
                            return allStats[(PlayerStats)sceneMgr.GetRandomGenerator().Next((int)PlayerStats.MINE_1_MIN + 1, (int)PlayerStats.MINE_1_MAX)];
                    }
                case DeviceType.CANNON:
                    switch (upgradeLevel)
                    {
                        case UpgradeLevel.LEVEL1:
                        case UpgradeLevel.LEVEL2:
                        case UpgradeLevel.LEVEL3:
                        default:
                            return allStats[(PlayerStats)sceneMgr.GetRandomGenerator().Next((int)PlayerStats.BULLET_1_MIN + 1, (int)PlayerStats.BULLET_1_MAX)];
                    }
                case DeviceType.HOOK:
                    switch (upgradeLevel)
                    {
                        case UpgradeLevel.LEVEL1:
                        case UpgradeLevel.LEVEL2:
                        case UpgradeLevel.LEVEL3:
                        default:
                            return allStats[(PlayerStats)sceneMgr.GetRandomGenerator().Next((int)PlayerStats.HOOK_1_MIN + 1, (int)PlayerStats.HOOK_1_MAX)];
                    }
                case DeviceType.HEALING_KIT:
                    switch (upgradeLevel)
                    {
                        case UpgradeLevel.LEVEL1:
                        case UpgradeLevel.LEVEL2:
                        case UpgradeLevel.LEVEL3:
                        default:
                            return allStats[(PlayerStats)sceneMgr.GetRandomGenerator().Next((int)PlayerStats.HEALING_KIT_1_MIN + 1, (int)PlayerStats.HEALING_KIT_1_MAX)];
                    }
                default:
                    throw new Exception("Received invalid DeviceType");
            }
        }

        private UpgradeLevel GetUpgradeLevel(Player plr, DeviceType type)
        {
            switch (type)
            {
                case DeviceType.MINE:
                    return plr.Mine.UpgradeLevel;
                case DeviceType.CANNON:
                    return plr.Canoon.UpgradeLevel;
                case DeviceType.HOOK:
                    return plr.Hook.UpgradeLevel;
                case DeviceType.HEALING_KIT:
                    return plr.HealingKit.UpgradeLevel;
                default:
                    throw new Exception("Received invalid DeviceType");
            }
        }
    }

    public struct Stat
    {
        public UpgradeLevel level;
        public PlayerStats type;
        public float min;
        public float max;

        public Stat(UpgradeLevel level, PlayerStats type, float min, float max)
        {
            this.level = level;
            this.type = type;
            this.min = min;
            this.max = max;
        }
    }

    public enum PlayerStats
    {
        MINE_1_MIN,
        MINE_1_GROWTH_SPEED,
        MINE_1_STRENGTH,
        MINE_1_COOLDOWN,
        MINE_1_FALLING_SPEED,
        MINE_1_MAX,

        BULLET_1_MIN,
        BULLET_1_SPEED,
        BULLET_1_COOLDOWN,
        BULLET_1_DAMAGE,
        BULLET_1_MAX,

        HOOK_1_MIN,
        HOOK_1_LENGTH,
        HOOK_1_SPEED,
        HOOK_1_COOLDOWN,
        HOOK_1_MAX,

        HEALING_KIT_1_MIN,
        HEALING_KIT_1_REPAIR_BASE,
        HEALING_KIT_1_FORTIFY_BASE,
        HEALING_KIT_1_MAX
    }
}