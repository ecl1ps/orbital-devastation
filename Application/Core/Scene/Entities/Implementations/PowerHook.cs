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
        public PowerHook(SceneMgr mgr, long id)
            : base(mgr, id)
        {
            HookType = HookType.HOOK_POWER;
        }
    }
}
