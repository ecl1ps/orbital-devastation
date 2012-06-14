using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Orbit.Core.Utils
{
    public interface IHealingKit
    {
        int Cost { get; set;}

        void Heal();
    }
}
