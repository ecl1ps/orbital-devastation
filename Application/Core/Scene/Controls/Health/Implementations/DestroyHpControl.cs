﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Orbit.Core.Scene.Controls.Health.Implementations
{
    public class DestroyHpControl : HpControl
    {
        protected override void OnDeath()
        {
            me.DoRemoveMe();
        }
    }
}
