using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Orbit.Core.Scene.Entities
{
    public interface IControledDevice
    {
        bool IsMovingDown { get; set; }

        bool IsMovingTop { get; set; }

        bool IsMovingLeft { get; set; }

        bool IsMovingRight { get; set; }
    }
}
