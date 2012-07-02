using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Orbit.Core.Weapons;

namespace Orbit.Core.Utils
{
    public interface IHealingKit
    {
        UpgradeLevel UpgradeLevel { get; set; }

        int Cost { get; set;}

        void Heal();
    }
}
