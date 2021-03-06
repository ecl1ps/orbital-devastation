﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Orbit.Core.Weapons;
using Orbit.Core.Client;
using Lidgren.Network;
using System.Windows;
using Orbit.Core.Client.GameStates;
using Orbit.Core.Scene.Entities;

namespace Orbit.Core.Players
{ 
    public class StatsMgr
    {
        private static readonly log4net.ILog Logger = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        private SceneMgr sceneMgr;
        private static Dictionary<PlayerStats, Stat> allStats = new Dictionary<PlayerStats, Stat>()
        {
            { PlayerStats.MINE_1_COOLDOWN, new Stat(UpgradeLevel.LEVEL1, PlayerStats.MINE_1_COOLDOWN, Strings.powerup_mine_cooldown, -0.2f, -0.45f)},
            { PlayerStats.MINE_1_FALLING_SPEED, new Stat(UpgradeLevel.LEVEL1, PlayerStats.MINE_1_FALLING_SPEED, Strings.powerup_mine_falling_speed, +20f, +45f)},
            { PlayerStats.MINE_1_GROWTH_SPEED, new Stat(UpgradeLevel.LEVEL1, PlayerStats.MINE_1_GROWTH_SPEED, Strings.powerup_mine_growth_speed, +0.2f, +0.45f)},
            { PlayerStats.MINE_1_STRENGTH, new Stat(UpgradeLevel.LEVEL1, PlayerStats.MINE_1_STRENGTH, Strings.powerup_mine_power, +20f, +45f)},

            { PlayerStats.CANNON_1_COOLDOWN, new Stat(UpgradeLevel.LEVEL1, PlayerStats.CANNON_1_COOLDOWN, Strings.powerup_cannon_cooldown, -0.06f, -0.1f)},
            { PlayerStats.CANNON_1_DAMAGE, new Stat(UpgradeLevel.LEVEL1, PlayerStats.CANNON_1_DAMAGE, Strings.powerup_cannon_damage, +2f, +3f)},
            { PlayerStats.CANNON_1_SPEED, new Stat(UpgradeLevel.LEVEL1, PlayerStats.CANNON_1_SPEED, Strings.powerup_cannon_bullet_speed, +60f, +150f)},

            { PlayerStats.HOOK_1_SPEED, new Stat(UpgradeLevel.LEVEL1, PlayerStats.HOOK_1_SPEED, Strings.powerup_hook_speed, +30f, +60f)},
            { PlayerStats.HOOK_1_LENGTH, new Stat(UpgradeLevel.LEVEL1, PlayerStats.HOOK_1_LENGTH, Strings.powerup_hook_length, +60f, +120f)},
            { PlayerStats.HOOK_1_COOLDOWN, new Stat(UpgradeLevel.LEVEL1, PlayerStats.HOOK_1_COOLDOWN, Strings.powerup_hook_cooldown, -0.2f, -0.45f)},

            { PlayerStats.HEALING_KIT_1_REPAIR_BASE, new Stat(UpgradeLevel.LEVEL1, PlayerStats.HEALING_KIT_1_REPAIR_BASE, Strings.powerup_base_repair, +15f, +25f)},
            { PlayerStats.HEALING_KIT_1_FORTIFY_BASE, new Stat(UpgradeLevel.LEVEL1, PlayerStats.HEALING_KIT_1_FORTIFY_BASE, Strings.powerup_base_fortify, +15f, +25f)},
            { PlayerStats.HEALING_KIT_1_BONUS_HEAL, new Stat(UpgradeLevel.LEVEL1, PlayerStats.HEALING_KIT_1_BONUS_HEAL, Strings.powerup_heal_bonus, +2, +7)}
        };
    
        public StatsMgr(SceneMgr mgr)
        {
            sceneMgr = mgr;
        }
        
        public void OnPlayerCaughtPowerUp(Player plr, DeviceType type)
        {
            if (!plr.IsCurrentPlayerOrBot())
                return;

            Stat pickedStat = GetStatForDeviceTypeAndLevel(type, GetUpgradeLevel(plr, type));
                plr.Statistics.Stats.Add(pickedStat);

            Tuple<float, float> valAndPct = GenerateAndAddStatToPlayer(pickedStat, plr.Data);

            Vector textPos = new Vector(plr.GetBaseLocation().X + (plr.GetBaseLocation().Width / 2), plr.GetBaseLocation().Y - 40);
            sceneMgr.FloatingTextMgr.AddFloatingText(String.Format(Strings.Culture, Strings.ft_powerup, 
                pickedStat.text, (valAndPct.Item2 > 0 ? Strings.char_plus : string.Empty), valAndPct.Item2.ToString("0.0", Strings.Culture)), 
                textPos, FloatingTextManager.TIME_LENGTH_3, FloatingTextType.SYSTEM, 14, true, true);

            NetOutgoingMessage msg = sceneMgr.CreateNetMessage();
            msg.Write((int)PacketType.PLAYER_RECEIVED_POWERUP);
            msg.Write(plr.GetId());
            msg.Write((byte)pickedStat.type);
            msg.Write(valAndPct.Item1);
            sceneMgr.SendMessage(msg);
        }

        /// <summary>
        /// prvni hodnota je primo hodnota o kterou se stat zmenil, druha hodnota jsou procenta o ktera se zmenil
        /// </summary>
        /// <param name="stat">hodnota o kterou se stat zmenil</param>
        /// <param name="data">procenta o ktera se stat zmenil</param>
        /// <returns></returns>
        private Tuple<float, float> GenerateAndAddStatToPlayer(Stat stat, PlayerData data)
        {
            // ziskame rozmezi min - max
            float val = (float)sceneMgr.GetRandomGenerator().NextDouble() * (stat.max - stat.min) + stat.min;

            Logger.Debug("Added stat " + stat.type + " (" + val + ") to player " + data.Name);

            float addedPct = AddStatToPlayer(data, stat.type, val);

            return new Tuple<float, float>(val, addedPct);
        }

        public float AddStatToPlayer(PlayerData data, PlayerStats type, float val)
        {
            float pct = 0;

            switch (type)
            {
                case PlayerStats.MINE_1_COOLDOWN:
                    pct = data.MineCooldown;
                    data.MineCooldown += val;
                    break;
                case PlayerStats.MINE_1_FALLING_SPEED:
                    pct = data.MineFallingSpeed;
                    data.MineFallingSpeed += (int)val;
                    break;
                case PlayerStats.MINE_1_GROWTH_SPEED:
                    pct = data.MineGrowthSpeed;
                    data.MineGrowthSpeed += val;
                    break;
                case PlayerStats.MINE_1_STRENGTH:
                    pct = data.MineStrength;
                    data.MineStrength += val;
                    break;

                case PlayerStats.CANNON_1_COOLDOWN:
                    pct = data.BulletCooldown;
                    data.BulletCooldown += val;
                    break;
                case PlayerStats.CANNON_1_DAMAGE:
                    pct = data.BulletDamage;
                    data.BulletDamage += (int)val;
                    break;
                case PlayerStats.CANNON_1_SPEED:
                    pct = data.BulletSpeed;
                    data.BulletSpeed += (int)val;
                    break;

                case PlayerStats.HOOK_1_COOLDOWN:
                    pct = data.HookCooldown;
                    data.HookCooldown += val;
                    break;
                case PlayerStats.HOOK_1_LENGTH:
                    pct = data.HookLenght;
                    data.HookLenght += (int)val;
                    break;
                case PlayerStats.HOOK_1_SPEED:
                    pct = data.HookSpeed;
                    data.HookSpeed += (int)val;
                    break;
                case PlayerStats.HEALING_KIT_1_BONUS_HEAL:
                    pct = data.MaxBaseIntegrity * SharedDef.HEAL_AMOUNT + data.BonusHeal;
                    data.BonusHeal += (int)val;
                    break;
                case PlayerStats.HEALING_KIT_1_REPAIR_BASE:
                    if (sceneMgr != null)
                    {
                        Player p = sceneMgr.GetPlayer(data.Id);
                        pct = p.Data.MaxBaseIntegrity;
                        int prevIntegrity = p.GetBaseIntegrity();
                        p.ChangeBaseIntegrity((int)val, true);
                        val = p.GetBaseIntegrity() - prevIntegrity;
                    }
                    else
                    {
                        data.BaseIntegrity += (int)val;
                        if (data.BaseIntegrity > data.MaxBaseIntegrity)
                            data.BaseIntegrity = data.MaxBaseIntegrity;
                    }
                    break;
                case PlayerStats.HEALING_KIT_1_FORTIFY_BASE:
                    int healVal = data.BaseIntegrity * (data.MaxBaseIntegrity + (int)val) / data.MaxBaseIntegrity - data.BaseIntegrity;
                    pct = data.MaxBaseIntegrity;
                    data.MaxBaseIntegrity += (int)val;
                    if (sceneMgr != null)
                        sceneMgr.GetPlayer(data.Id).ChangeBaseIntegrity(healVal, true);
                    else
                    {
                        data.BaseIntegrity += healVal;
                        if (data.BaseIntegrity > data.MaxBaseIntegrity)
                            data.BaseIntegrity = data.MaxBaseIntegrity;
                    }
                    break;
            }

            // vypocet o kolik se procentualne zmeni stat
            pct = val * 100 / pct;
            return pct;
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
                        case UpgradeLevel.LEVEL3:
                        case UpgradeLevel.LEVEL1:
                        case UpgradeLevel.LEVEL2:
                        default:
                            return allStats[(PlayerStats)sceneMgr.GetRandomGenerator().Next((int)PlayerStats.CANNON_1_MIN + 1, (int)PlayerStats.CANNON_1_MAX)];
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
                    return plr.Cannon.UpgradeLevel;
                case DeviceType.HOOK:
                    return plr.Hook.UpgradeLevel;
                case DeviceType.HEALING_KIT:
                    return plr.HealingKit.UpgradeLevel;
                default:
                    throw new Exception("Received invalid DeviceType");
            }
        }
    }

    public struct Stat : ISendable
    {
        public UpgradeLevel level;
        public PlayerStats type;
        public string text;
        public float min;
        public float max;

        public Stat(UpgradeLevel level, PlayerStats type, string text, float min, float max)
        {
            this.level = level;
            this.type = type;
            this.text = text;
            this.min = min;
            this.max = max;
        }

        public void WriteObject(NetOutgoingMessage msg)
        {
            msg.Write((int)level);
            msg.Write((int)type);
            msg.Write(text);
            msg.Write(min);
            msg.Write(max);
        }

        public void ReadObject(NetIncomingMessage msg)
        {
            level = (UpgradeLevel)msg.ReadInt32();
            type = (PlayerStats)msg.ReadInt32();
            text = msg.ReadString();
            min = msg.ReadFloat();
            max = msg.ReadFloat();
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

        MINE_2_MIN,
        MINE_2_MAX,

        MINE_3_MIN,
        MINE_3_MAX,

        CANNON_1_MIN,
        CANNON_1_SPEED,
        CANNON_1_COOLDOWN,
        CANNON_1_DAMAGE,
        CANNON_1_MAX,

        CANNON_2_MIN,
        CANNON_2_MAX,

        CANNON_3_MIN,
        CANNON_3_MAX,

        HOOK_1_MIN,
        HOOK_1_LENGTH,
        HOOK_1_SPEED,
        HOOK_1_COOLDOWN,
        HOOK_1_MAX,

        HOOK_2_MIN,
        HOOK_2_MAX,

        HOOK_3_MIN,
        HOOK_3_MAX,

        HEALING_KIT_1_MIN,
        HEALING_KIT_1_REPAIR_BASE,
        HEALING_KIT_1_FORTIFY_BASE,
        HEALING_KIT_1_BONUS_HEAL,
        HEALING_KIT_1_MAX,
    }
}
