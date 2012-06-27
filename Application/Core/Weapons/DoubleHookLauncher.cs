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
    public class DoubleHookLauncher : HookLauncher
    {
        protected class HookData
        {
            public Hook Hook { get; set; }
            public float Time { get; set; }

            public HookData(Hook hook, float Time)
            {
                this.Hook = hook;
                this.Time = Time;
            }
        }

        private List<HookData> hooks;
        private int maxCount;

        public DoubleHookLauncher(SceneMgr mgr, Player owner) : base(mgr, owner)
        {
            hooks = new List<HookData>();
            maxCount = 2;
            Cost = 300;
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
            hooks.Add(new HookData(hook, SharedDef.HOOK_COOLDOWN));
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
                if (hooks.ElementAt(i).Hook.Dead && hooks.ElementAt(i).Time <= 0)
                    hooks.RemoveAt(i);
            }
        }

        public override void Update(float tpf)
        {
            hooks.ForEach(a => a.Time -= tpf);
        }


        public override void TriggerUpgrade(IWeapon old)
        {
            base.TriggerUpgrade(old);
            if (old is HookLauncher)
            {
                if (!(old as HookLauncher).IsReady())
                    hooks.Add(new HookData((old as HookLauncher).getHook(), 0));
            }
        }
    }
}
