using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Orbit.Core.Scene.Entities
{
    public interface IDestroyable : ISceneObject
    {
        void TakeDamage(int damage, ISceneObject from);
    }
}
