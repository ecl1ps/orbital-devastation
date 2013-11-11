using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Orbit.Core.Scene.Entities
{
    interface ISpheric : ISceneObject
    {
        int Radius { get; set; }
    }
}
