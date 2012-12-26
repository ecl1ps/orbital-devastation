using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Orbit.Core.Client;
using System.Windows;

namespace Orbit.Core.Scene.Entities.Implementations
{
    public class PowerHook : Hook
    {
        public List<ICatchable> CaughtObjects { get; set; }

        public PowerHook(SceneMgr mgr, long id)
            : base(mgr, id)
        {
            CaughtObjects = new List<ICatchable>();
            HookType = HookType.HOOK_POWER;
        }

        public override void OnCatch()
        {
            base.OnCatch();
            CaughtObjects.Add(CaughtObject);
        }

        public override void PulledCaughtObjectToBase()
        {
            CaughtObjects.ForEach(obj => ProccesCaughtObject(obj));
            DoRemoveMe();
        }

        protected override void AddGoldToOwner(int gold)
        {
            base.AddGoldToOwner((int) (gold * 1.5));
        }

        public override bool HasCaughtObject()
        {
            return CaughtObjects.Count == Owner.Data.HookMaxCatchedObjCount;
        }
    }
}
