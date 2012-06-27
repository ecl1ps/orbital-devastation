using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Orbit.Core.Players;

namespace Orbit.Core.Scene.Entities
{
    public interface IDamageable : ISceneObject
    {
        Player Owner { get; set; }
    }
}
