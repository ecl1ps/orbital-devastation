using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Orbit.Core
{
    public interface IUpdatable
    {
        void Update(float tpf);
    }
}
