using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Orbit.Core.Scene.Controls
{
    public interface IDamageControl
    {
        bool Vulnerable { get; set; }

        void ProccessDamage(int damage, Entities.ISceneObject causedBy);
    }
}
