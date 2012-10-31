using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Orbit.Core.Weapons;
using Orbit.Core.Client;
using Lidgren.Network;
using System.Windows;
using Orbit.Core.Client.GameStates;

namespace Orbit.Core.Players
{ 
    public class StatsMgr
    {
        private SceneMgr sceneMgr;
        private Dictionary<PlayerStats, Stat> allStats = new Dictionary<PlayerStats, Stat>();

        public StatsMgr(SceneMgr mgr)
        {
            sceneMgr = mgr;

            allStats.Add(PlayerStats.MINE_1_COOLDOWN, new Stat(UpgradeLevel.LEVEL1, PlayerStats.MINE_1_COOLDOWN, "Mine cooldown", -0.1f, -0.3f));
            allStats.Add(PlayerStats.MINE_1_FALLING_SPEED, new Stat(UpgradeLevel.LEVEL1, PlayerStats.MINE_1_FALLING_SPEED, "Mine falling speed", +10f, +30f));
            allStats.Add(PlayerStats.MINE_1_GROWTH_SPEED, new Stat(UpgradeLevel.LEVEL1, PlayerStats.MINE_1_GROWTH_SPEED, "Mine growth speed", +0.1f, +0.3f));
            allStats.Add(PlayerStats.MINE_1_STRENGTH, new Stat(UpgradeLevel.LEVEL1, PlayerStats.MINE_1_STRENGTH, "Mine power", +10f, +30f));

            allStats.Add(PlayerStats.CANNON_1_COOLDOWN, new Stat(UpgradeLevel.LEVEL1, PlayerStats.CANNON_1_COOLDOWN, "Cannon cooldown", -0.03f, -0.07f));
            allStats.Add(PlayerStats.CANNON_1_DAMAGE, new Stat(UpgradeLevel.LEVEL1, PlayerStats.CANNON_1_DAMAGE, "Cannon damage", +1f, +2f));
            allStats.Add(PlayerStats.CANNON_1_SPEED, new Stat(UpgradeLevel.LEVEL1, PlayerStats.CANNON_1_SPEED, "Cannon bullet speed", +30f, +100f));

            allStats.Add(PlayerStats.CANNON_3_CHARGE_TIME, new Stat(UpgradeLevel.LEVEL3, PlayerStats.CANNON_3_CHARGE_TIME, "Laser charging time", -0.03f, -0.05f));
            allStats.Add(PlayerStats.CANNON_3_DAMAGE, new Stat(UpgradeLevel.LEVEL3, PlayerStats.CANNON_3_DAMAGE, "Laser damage", +1f, +2f));
            allStats.Add(PlayerStats.CANNON_3_DAMAGE_INTERVAL, new Stat(UpgradeLevel.LEVEL3, PlayerStats.CANNON_3_DAMAGE_INTERVAL, "Laser damage interval", -0.01f, -0.03f));

            allStats.Add(PlayerStats.HOOK_1_SPEED, new Stat(UpgradeLevel.LEVEL1, PlayerStats.HOOK_1_SPEED, "Hook speed", +20f, +40f));
            allStats.Add(PlayerStats.HOOK_1_LENGTH, new Stat(UpgradeLevel.LEVEL1, PlayerStats.HOOK_1_LENGTH, "Hook length", +40f, +80f));
            allStats.Add(PlayerStats.HOOK_1_COOLDOWN, new Stat(UpgradeLevel.LEVEL1, PlayerStats.HOOK_1_COOLDOWN, "Hook cooldown", -0.1f, -0.3f));

            allStats.Add(PlayerStats.HEALING_KIT_1_REPAIR_BASE, new Stat(UpgradeLevel.LEVEL1, PlayerStats.HEALING_KIT_1_REPAIR_BASE, "Base repair", +25f, +35f));
            allStats.Add(PlayerStats.HEALING_KIT_1_FORTIFY_BASE, new Stat(UpgradeLevel.LEVEL1, PlayerStats.HEALING_KIT_1_FORTIFY_BASE, "Base fortify", +15f, +25f));
        }
        
        public void OnPlayerCaughtPowerUp(Player plr, DeviceType type)
        {
            if (!plr.IsCurrentPlayerOrBot())
                return;

            Stat pickedStat = GetStatForDeviceTypeAndLevel(type, GetUpgradeLevel(plr, type));

            float addedValue = GenerateAndAddStatToPlayer(pickedStat, plr.Data);

            Vector textPos = new Vector(plr.GetBaseLocation().X + (plr.GetBaseLocation().Width / 2), plr.GetBaseLocation().Y - 40);
            sceneMgr.FloatingTextMgr.AddFloatingText(pickedStat.text + (addedValue > 0 ? " +" : " ") + addedValue.ToString("0.0"), textPos, FloatingTextManager.TIME_LENGTH_3,
                FloatingTextType.SYSTEM, 14, true, true);

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

                case PlayerStats.CANNON_1_COOLDOWN:
                    data.BulletCooldown += val;
                    break;
                case PlayerStats.CANNON_1_DAMAGE:
                    data.BulletDamage += (int)val;
                    break;
                case PlayerStats.CANNON_1_SPEED:
                    data.BulletSpeed += (int)val;
                    break;
                case PlayerStats.CANNON_3_CHARGE_TIME:
                    data.LaserChargingTime += val;
                    break;
                case PlayerStats.CANNON_3_DAMAGE:
                    data.LaserDamage += (int)val;
                    break;
                case PlayerStats.CANNON_3_DAMAGE_INTERVAL:
                    data.LaserDamageInterval += val;
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
                    int healVal = data.BaseIntegrity * (data.MaxBaseIntegrity + (int)val) / data.MaxBaseIntegrity - data.BaseIntegrity;
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
                            return allStats[(PlayerStats)sceneMgr.GetRandomGenerator().Next((int)PlayerStats.CANNON_3_MIN + 1, (int)PlayerStats.CANNON_3_MAX)];
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
        CANNON_3_CHARGE_TIME,
        CANNON_3_DAMAGE,
        CANNON_3_DAMAGE_INTERVAL,
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
        HEALING_KIT_1_MAX,
    }
}
