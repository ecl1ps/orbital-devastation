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
            if (hook.HasCaughtObject())
                return;

            if (!(other is ICatchable))
                return;

            if ((me as PowerHook).CaughtObjects.Count <= hook.Owner.Data.HookMaxCatchedObjCount)
                CatchObject(other as ICatchable);
        }

        protected override void CatchObject(ICatchable caught)
        {
            base.CatchObject(caught);

            if ((me as PowerHook).CaughtObjects.Count == 1)
                Returning = true;
        }
    }
}
