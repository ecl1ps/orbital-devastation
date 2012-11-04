using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Orbit.Core.Scene.Entities;

namespace Orbit.Core.Scene.Controls.Collisions
{
    public interface ICollisionReactionControl
    {
        void DoCollideWith(ISceneObject other, float tpf);
    }
}
