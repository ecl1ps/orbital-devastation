using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Orbit.Core.Scene.Entities;

namespace Orbit.Core.Scene.Controls
{
    interface IDamageControl : IControl
    {
        bool Vulnerable { get; set; }

        void ProccessDamage(int damage, ISceneObject causedBy);
    }
}
