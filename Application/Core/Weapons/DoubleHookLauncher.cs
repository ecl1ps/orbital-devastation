using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Orbit.Core.Weapons;
using Orbit.Core.Scene.Entities;
using Orbit.Core.Players;
using Orbit.Core.Scene;
using System.Windows;
using Orbit.Core.Scene.Entities.Implementations;
using Orbit.Core.Client;

namespace Orbit.Core.Weapons
{
    class DoubleHookLauncher : HookLauncher
    {
        private List<Hook> hooks;
        private int maxCount;

        public DoubleHookLauncher(SceneMgr mgr, Player owner) : base(mgr, owner)
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

        protected override Hook CreateHook(Point point)
        {
            Hook hook = base.CreateHook(point);
            hooks.Add(hook);
            return hook;
        }

        public override bool IsReady()
        {
            CleanHooks();
            return hooks.Count < maxCount;
        }

        private void CleanHooks()
        {
            for (int i = hooks.Count - 1; i >= 0; i--)
            {
                if (hooks.ElementAt(i).Dead)
                    hooks.RemoveAt(i);
            }
        }
    }
}
