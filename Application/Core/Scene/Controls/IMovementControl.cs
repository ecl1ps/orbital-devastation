using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Orbit.Core.Scene.Controls
{
    public interface IMovementControl : IControl
    {
        float Speed { get; set; }
    }
}
