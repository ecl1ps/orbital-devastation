using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Orbit.Core.Client
{
    public interface IAppState
    {
        void update(float tpf);
    }
}
