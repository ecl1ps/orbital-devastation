using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Orbit.src.Core.utils
{
    public interface IHealingKit
    {
        int Cost { get; set;}

        void heal();
    }
}
