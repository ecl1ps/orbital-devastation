using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Orbit.Core.Scene.Entities
{
    interface IControledDevice
    {
        public bool IsMovingDown { get; set; }

        public bool IsMovingTop { get; set; }

        public bool IsMovingLeft { get; set; }

        public bool IsMovingRight { get; set; }
    }
}
