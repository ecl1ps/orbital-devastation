using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Input;

namespace Orbit.Core.Client.Interfaces
{
    public interface IMouseClickListener
    {
        void OnCanvasClick(Vector2 point, MouseButtonEventArgs e);
    }
}
