using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;

namespace Orbit.Core.Client.Interfaces
{
    public interface IMouseMoveListener
    {
        void OnMouseMove(Point point);
    }
}
