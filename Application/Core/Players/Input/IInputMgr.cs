using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Input;
using System.Windows;

namespace Orbit.Core.Players.Input
{
    public interface IInputMgr
    {
        void OnCanvasClick(Point point, MouseButtonEventArgs e);

        void OnActionBarClick(Point point, MouseButtonEventArgs e);

        void OnKeyEvent(KeyEventArgs e);
    }
}
