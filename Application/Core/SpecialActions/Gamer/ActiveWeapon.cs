using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Orbit.Core.Weapons;
using Orbit.Core.Client;
using Orbit.Core.Players;
using Orbit.Gui;
using Microsoft.Xna.Framework;

namespace Orbit.Core.SpecialActions.Gamer
{
    public class ActiveWeapon : SpecialAction, IPlayerAction
    {
        private IActivableWeapon weapon;

        public ActiveWeapon() : base(null, null)
        {
            Cost = 0;
            BackgroundColor = Color.PaleTurquoise;
        }

        public ActiveWeapon(IActivableWeapon weapon)
            : base(weapon.SceneMgr, weapon.Owner)
        {
            this.weapon = weapon;
            ImageSource = "pack://application:,,,/resources/images/icons/" + weapon.Data.Icon;
            Name = weapon.Data.Name;
            Cost = 0;
            Cooldown = weapon.Data.Cooldown;
            BackgroundColor = Color.PaleTurquoise;
        }

        public override void StartAction()
        {
            if (!IsReady())
                return;

            base.StartAction();
            weapon.StartActivableAction();
        }

        public override bool IsReady()
        {
            return !IsOnCooldown() && weapon.IsActivableReady();
        }

        public override void ReadObject(Lidgren.Network.NetIncomingMessage msg)
        {
            base.ReadObject(msg);
            Name = msg.ReadString();
        }

        public override void WriteObject(Lidgren.Network.NetOutgoingMessage msg)
        {
            base.WriteObject(msg);
            msg.Write(Name);
        }
    }
}
