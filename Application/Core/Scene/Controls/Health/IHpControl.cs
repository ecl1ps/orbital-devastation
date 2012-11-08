using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Orbit.Core.Scene.Controls;

namespace Orbit.Core.Controls.Health
{
    interface IHpControl : IControl
    {
        int Hp { get; set; }

        int MaxHp { get; set; }

        void RefillHp();
    }
}
