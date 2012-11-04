using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Orbit.Core.Scene.Entities.Implementations;
using Orbit.Core.Scene.Entities;

namespace Orbit.Core.Scene.Controls.Implementations
{
    public class PowerHookControl : HookControl
    {
        public override void DoCollideWith(ISceneObject other, float tpf)
        {
            if ((me as PowerHook).CaughtObjects.Count <= hook.Owner.Data.HookMaxCatchedObjCount)
                CatchObject(other as ICatchable);
        }
    }
}
