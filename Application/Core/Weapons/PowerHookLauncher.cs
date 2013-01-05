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
using Orbit.Core.SpecialActions.Gamer;
using Orbit.Core.Scene.Controls.Implementations;

namespace Orbit.Core.Weapons
{
    public class PowerHookLauncher : DoubleHookLauncher, IActivableWeapon
    {
        public ActivableData Data { get; set; }

        private Hook lastHook;

        public PowerHookLauncher(SceneMgr mgr, Player owner)
            : base(mgr, owner)
        {
            Cost = 700;
            Name = "Power hook launcher";
            UpgradeLevel = UpgradeLevel.LEVEL3;
            Data = new ActivableData("Force Pull", "active.png", 10);
        }

        public override ISpecialAction NextSpecialAction()
        {
            if (next == null)
                next = new ActiveWeapon(this);

            return next;
        }

        protected override Hook CreateHook(Point point)
        {
            if (lastHook != null)
                lastHook.GetControlOfType<HighlightingControl>().Enabled = false;

            lastHook = SceneObjectFactory.CreatePowerHook(SceneMgr, point, Owner);

            HighlightingControl hc = new HighlightingControl();
            lastHook.AddControl(hc);

            hooks.Add(new HookData(lastHook, Owner.Data.HookCooldown));
            return lastHook;
        }

        public bool IsActivableReady()
        {
            bool ready = lastHook != null && !lastHook.Dead && !lastHook.GetControlOfType<PowerHookControl>().ForcePullUsed;

            if (ready)
                lastHook.GetControlOfType<HighlightingControl>().Enabled = true;

            return ready;
        }

        public void StartActivableAction()
        {
            lastHook.GetControlOfType<PowerHookControl>().EmitForcePullField();
        }
    }
}
