using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using Orbit.Core.Client;
using Orbit.Core.Players;
using Orbit.Core.Scene.Entities.Implementations;
using Orbit.Core.Helpers;
using Orbit.Core.SpecialActions;

namespace Orbit.Core.Weapons
{
    class PowerHookLauncher : DoubleHookLauncher
    {
        public PowerHookLauncher(SceneMgr mgr, Player owner)
            : base(mgr, owner)
        {
            Cost = 700;
            Name = "Power hook launcher";
        }

        public override ISpecialAction NextSpecialAction()
        {
            return null;
        }

        protected override Hook CreateHook(Point point)
        {
            Hook hook = SceneObjectFactory.CreatePowerHook(SceneMgr, point, Owner);
            hooks.Add(new HookData(hook, Owner.Data.HookCooldown));
            return hook;
        }
    }
}
