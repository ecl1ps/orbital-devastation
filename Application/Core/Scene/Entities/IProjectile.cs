using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Orbit.Core.Players;

namespace Orbit.Core.Scene.Entities
{
    public interface IProjectile : ISceneObject
    {
        Player Owner { get; set; }
    }
}
