using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Orbit.Core.Scene.Entities
{
    public interface IDestroyable
    {
       void TakeDamage(int damage);
    }
}
