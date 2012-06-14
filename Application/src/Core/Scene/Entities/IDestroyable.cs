using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Orbit.src.Core.Scene.Entities
{
    interface IDestroyable
    {
       void doDamage(int damage);
    }
}
