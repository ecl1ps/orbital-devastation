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

        public PowerHook(SceneMgr mgr)
            : base(mgr)
        {
            CaughtObjects = new List<ICatchable>();
            HookType = HookType.HOOK_POWER;
        }

        public override void Catch(ICatchable caught, Vector hitVector)
        {
            base.Catch(caught, hitVector);
            CaughtObjects.Add(caught);
        }

        public override void DoCollideWith(ICollidable other, float tpf)
        {
                if(CaughtObjects.Count <= Owner.Data.HookMaxCatchedObjCount)
                    CatchObject(other as ICatchable);
        }

        public override void PulledCaughtObjectToBase()
        {
            CaughtObjects.ForEach(obj => proccesCaughtObject(obj));
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
