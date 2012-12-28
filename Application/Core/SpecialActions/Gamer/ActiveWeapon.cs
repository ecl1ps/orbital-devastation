using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Orbit.Core.Weapons;
using Orbit.Core.Client;
using Orbit.Core.Players;
using Orbit.Gui;

namespace Orbit.Core.SpecialActions.Gamer
{
    public class ActiveWeapon : SpecialAction
    {
        private IActivableWeapon weapon;

        public ActiveWeapon(IActivableWeapon weapon)
            : base(weapon.SceneMgr, weapon.Owner)
        {
            this.weapon = weapon;
            ImageSource = "pack://application:,,,/resources/images/icons/" + weapon.ActivableIcon;
            Name = weapon.ActivableName;
            Cost = 0;
            Cooldown = 5;
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
    }
}
