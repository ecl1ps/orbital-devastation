using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Orbit.Core.Weapons;
using Orbit.Core.Client;
using Orbit.Core.Players;
using Orbit.Gui;
using Orbit.Gui.ActionControllers;
using Microsoft.Xna.Framework;
using Lidgren.Network;
using Orbit.Core.Client.GameStates;

namespace Orbit.Core.SpecialActions.Gamer
{
    class WeaponUpgrade : SpecialAction, IPlayerAction
    {
        private IWeapon currentWeapon;

        public WeaponUpgrade()
            : base(null, null)
        {
            ImageSource = "pack://application:,,,/resources/images/icons/upgrade.png";
            BackgroundColor = Color.AntiqueWhite;
            Cooldown = 0;
        }

        public WeaponUpgrade(WeaponUpgrade upgrade) : base(upgrade.SceneMgr, upgrade.Owner)
        {
            ImageSource = upgrade.ImageSource;
            BackgroundColor = upgrade.BackgroundColor;
            Cooldown = upgrade.Cooldown;

            LoadWeapon(upgrade.GetWeapon());
        }

        public WeaponUpgrade(IWeapon weapon) : base(weapon.SceneMgr, weapon.Owner)
        {
            LoadWeapon(weapon);

            ImageSource = "pack://application:,,,/resources/images/icons/upgrade.png";
            BackgroundColor = Color.AntiqueWhite;
            Cooldown = 0;
        }

        private void LoadWeapon(IWeapon weapon)
        {
            this.currentWeapon = weapon;

            if (weapon == null || weapon.NextSpecialAction() == null || !(weapon.NextSpecialAction() is WeaponUpgrade))
            {
                Name = "Unavailable";
                Cost = 0;
            }
            else
            {
                Name = (weapon.NextSpecialAction() as WeaponUpgrade).GetWeapon().Name;
                Cost = (weapon.NextSpecialAction() as WeaponUpgrade).GetWeapon().Cost;
            }
        }

        public override void StartAction()
        {
            if (IsReady())
            {
                StartCoolDown();
                Owner.Statistics.Actions.Add(new WeaponUpgrade(this));
                shared.ForEach(a => a.StartCoolDown());
                UpgradeWeapon();
                LoadActivableActionIfAvailable();
            }
        }

        private void LoadActivableActionIfAvailable()
        {
            if (currentWeapon.NextSpecialAction() == null || !(currentWeapon.NextSpecialAction() is ActiveWeapon))
                return;

            ActionBarMgr mgr = SceneMgr.StateMgr.GetGameStateOfType<ActionBarMgr>();
            mgr.SwitchAction(this, currentWeapon.NextSpecialAction());
        }

        private void UpgradeWeapon()
        {
            if (!(currentWeapon.NextSpecialAction() is WeaponUpgrade))
                return;

            switch (currentWeapon.DeviceType)
            {
                case DeviceType.CANNON:
                    Owner.Canoon = (currentWeapon.NextSpecialAction() as WeaponUpgrade).GetWeapon();
                    break;
                case DeviceType.HOOK:
                    Owner.Hook = (currentWeapon.NextSpecialAction() as WeaponUpgrade).GetWeapon();
                    break;
                case DeviceType.MINE:
                    Owner.Mine = (currentWeapon.NextSpecialAction() as WeaponUpgrade).GetWeapon();
                    break;

                default:
                    throw new Exception("unknown weapon type " + currentWeapon.DeviceType.ToString());
            }

            SendPlayerBoughtUpgrade();

            LoadWeapon((currentWeapon.NextSpecialAction() as WeaponUpgrade).GetWeapon());
        }

        private void SendPlayerBoughtUpgrade()
        {
            NetOutgoingMessage msg = SceneMgr.CreateNetMessage();
            msg.Write((int)PacketType.PLAYER_BOUGHT_UPGRADE);
            msg.Write(Owner.GetId());
            msg.Write((byte)currentWeapon.DeviceType);
            SceneMgr.SendMessage(msg);
        }

        public override bool IsReady()
        {
            return !IsOnCooldown() && currentWeapon.NextSpecialAction() != null && (currentWeapon.NextSpecialAction() as WeaponUpgrade).GetWeapon() != null && (currentWeapon.NextSpecialAction() as WeaponUpgrade).GetWeapon().Cost <= Owner.Data.Gold;
        }

        public IWeapon GetWeapon()
        {
            return currentWeapon;
        }

        public override void ReadObject(NetIncomingMessage msg)
        {
            base.ReadObject(msg);
            Name = msg.ReadString();
        }

        public override void WriteObject(NetOutgoingMessage msg)
        {
            base.WriteObject(msg);
            msg.Write(Name);
        }
    }
}
