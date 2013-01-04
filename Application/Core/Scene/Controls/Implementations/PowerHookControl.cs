using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Orbit.Core.Scene.Entities.Implementations;
using Orbit.Core.Scene.Entities;
using System.Windows;

namespace Orbit.Core.Scene.Controls.Implementations
{
    public class PowerHookControl : HookControl
    {
        public override void DoCollideWith(ISceneObject other, float tpf)
        {
            if (HasFullCapacity())
                return;

            if (!(other is ICatchable))
                return;

            TryToCatchCollidedObject(other as ICatchable);
        }

        protected override void TryToCatchCollidedObject(ICatchable caught)
        {
            base.TryToCatchCollidedObject(caught);

            if (caughtObjects.Count == 1)
                Returning = true;
        }

        protected override void AddGoldToOwner(int gold)
        {
            base.AddGoldToOwner((int)(gold * 1.5));
        }

        protected override bool HasFullCapacity()
        {
            return caughtObjects.Count == hook.Owner.Data.HookMaxCatchedObjCount;
        }
    }
}
