using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Orbit.Core.Scene.Entities;

namespace Orbit.src.Core.Scene.Entities
{
    interface IContainsGold : ISceneObject
    {
        int Gold { get; set; }
    }
}
