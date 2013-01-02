using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Orbit.Core.Scene.Controls.Collisions;
using Orbit.Core.Scene.Entities;

namespace Orbit.Core.Scene.Controls.Implementations
{
    public class StaticShieldCollisionReactionControl : Control, ICollisionReactionControl
    {
        public void DoCollideWith(ISceneObject other, float tpf)
        {
            if (other is IDestroyable)
                other.DoRemoveMe();
        }
    }
}
