using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Orbit.Core.Client.Interfaces
{
    interface Invoker
    {
        void Invoke(Action a);

        void BeginInvoke(Action a);
    }
}
