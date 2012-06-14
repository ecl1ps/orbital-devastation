using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Orbit.Core.Scene.Entities
{
    interface IDestroyable
    {
       void DoDamage(int damage);
    }
}
