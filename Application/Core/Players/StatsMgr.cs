using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Orbit.Core.Weapons;
using Orbit.Core.Client;
using Lidgren.Network;

namespace Orbit.Core.Players
{ 
    public class StatsMgr
    {
        private SceneMgr sceneMgr;
        private Dictionary<PlayerStats, Stat> allStats = new Dictionary<PlayerStats, Stat>();


        public StatsMgr(SceneMgr mgr)
        {
            sceneMgr = mgr;

            allStats.Add(PlayerStats.MINE_1_COOLDOWN, new Stat(WeaponLevel.LEVEL1, PlayerStats.MINE_1_COOLDOWN, 0.1f, 0.3f));
            allStats.Add(PlayerStats.MINE_1_FALLING_SPEED, new Stat(WeaponLevel.LEVEL1, PlayerStats.MINE_1_FALLING_SPEED, 10f, 30f));
            allStats.Add(PlayerStats.MINE_1_GROWTH_SPEED, new Stat(WeaponLevel.LEVEL1, PlayerStats.MINE_1_GROWTH_SPEED, 0.1f, 0.3f));
            allStats.Add(PlayerStats.MINE_1_STRENGTH, new Stat(WeaponLevel.LEVEL1, PlayerStats.MINE_1_STRENGTH, 10f, 30f));

            allStats.Add(PlayerStats.BULLET_1_COOLDOWN, new Stat(WeaponLevel.LEVEL1, PlayerStats.BULLET_1_COOLDOWN, 0.1f, 0.2f));
            allStats.Add(PlayerStats.BULLET_1_DAMAGE, new Stat(WeaponLevel.LEVEL1, PlayerStats.BULLET_1_DAMAGE, 1f, 2f));
            allStats.Add(PlayerStats.BULLET_1_SPEED, new Stat(WeaponLevel.LEVEL1, PlayerStats.BULLET_1_SPEED, 30f, 100f));

            allStats.Add(PlayerStats.HOOK_1_COOLDOWN, new Stat(WeaponLevel.LEVEL1, PlayerStats.HOOK_1_COOLDOWN, 20f, 40f));
            allStats.Add(PlayerStats.HOOK_1_LENGTH, new Stat(WeaponLevel.LEVEL1, PlayerStats.HOOK_1_LENGTH, 40f, 80f));
            allStats.Add(PlayerStats.HOOK_1_SPEED, new Stat(WeaponLevel.LEVEL1, PlayerStats.HOOK_1_SPEED, 0.1f, 0.3f));
        }
        
        public void OnPlayerCaughtPowerUp(Player plr, WeaponType type)
        {
            Stat pickedStat = GetStatForWeaponTypeAndLevel(type, GetWeaponLevel(plr, type));

            float addedValue = GenerateAndAddStatToPlayer(pickedStat, plr.Data);

            NetOutgoingMessage msg = sceneMgr.CreateNetMessage();
            msg.Write((int)PacketType.PLAYER_RECEIVED_POWERUP);
            msg.Write((byte)pickedStat.type);
            msg.Write(addedValue);
            sceneMgr.SendMessage(msg);

        }

        private float GenerateAndAddStatToPlayer(Stat stat, PlayerData data)
        {
            // ziskame rozmezi min - max
            float val = (float)sceneMgr.GetRandomGenerator().NextDouble() * (stat.max - stat.min) + stat.min;

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
            }
        }

        private Stat GetStatForWeaponTypeAndLevel(WeaponType type, WeaponLevel weaponLevel)
        {
            switch (type)
            {
                case WeaponType.MINE:
                    switch (weaponLevel)
                    {
                        case WeaponLevel.LEVEL1:
                        case WeaponLevel.LEVEL2:
                        case WeaponLevel.LEVEL3:
                        default:
                            return allStats[(PlayerStats)sceneMgr.GetRandomGenerator().Next((int)PlayerStats.MINE_1_MIN, (int)PlayerStats.MINE_1_MAX)];
                    }
                case WeaponType.CANNON:
                    switch (weaponLevel)
                    {
                        case WeaponLevel.LEVEL1:
                        case WeaponLevel.LEVEL2:
                        case WeaponLevel.LEVEL3:
                        default:
                            return allStats[(PlayerStats)sceneMgr.GetRandomGenerator().Next((int)PlayerStats.BULLET_1_MIN, (int)PlayerStats.BULLET_1_MAX)];
                    }
                case WeaponType.HOOK:
                    switch (weaponLevel)
                    {
                        case WeaponLevel.LEVEL1:
                        case WeaponLevel.LEVEL2:
                        case WeaponLevel.LEVEL3:
                        default:
                            return allStats[(PlayerStats)sceneMgr.GetRandomGenerator().Next((int)PlayerStats.HOOK_1_MIN, (int)PlayerStats.HOOK_1_MAX)];
                    }
                default:
                    throw new Exception("Received invalid WeaponType");
            }
        }

        private WeaponLevel GetWeaponLevel(Player plr, WeaponType type)
        {
            switch (type)
            {
                case WeaponType.MINE:
                    return plr.Mine.WeaponLevel;
                case WeaponType.CANNON:
                    return plr.Canoon.WeaponLevel;
                case WeaponType.HOOK:
                    return plr.Hook.WeaponLevel;
                default:
                    throw new Exception("Received invalid WeaponType");
            }
        }
    }

    public struct Stat
    {
        public WeaponLevel level;
        public PlayerStats type;
        public float min;
        public float max;

        public Stat(WeaponLevel level, PlayerStats type, float min, float max)
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
    }
}
