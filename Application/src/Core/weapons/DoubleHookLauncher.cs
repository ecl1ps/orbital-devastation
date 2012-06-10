using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Orbit.Core.Weapons;
using Orbit.Core.Scene.Entities;

namespace Orbit.src.Core.Weapons
{
    class DoubleHookLauncher : HookLauncher
    {
        private List<Hook> hooks;
        private int maxCount;

        public DoubleHookLauncher()
        {
            hooks = new List<Hook>();
            maxCount = 2;
            Cost = 1000;
            Name = "Double hook launcher";
            WeaponType = WeaponType.HOOK;
        }

        public override IWeapon Next()
        {
            return null;
        }

        protected override Hook createHook(System.Windows.Point point, Orbit.Core.Players.Player player)
        {
            Hook hook = base.createHook(point, player);
            hooks.Add(hook);
            return hook;
        }

        public override bool IsReady()
        {
            cleanHooks();
            return hooks.Count < maxCount;
        }

        private void cleanHooks()
        {
            for (int i = hooks.Count - 1; i >= 0; i--)
            {
                if (hooks.ElementAt(i).Dead)
                    hooks.RemoveAt(i);
            }
        }
    }
}
