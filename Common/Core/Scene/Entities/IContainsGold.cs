using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Orbit.Core.Scene.Entities;

namespace Orbit.Core.Scene.Entities
{
    public interface IContainsGold : ISceneObject
    {
        int Gold { get; set; }
    }
}
