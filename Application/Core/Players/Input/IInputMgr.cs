using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Input;
using System.Windows;
using Orbit.Core.Client.Interfaces;

namespace Orbit.Core.Players.Input
{
    public interface IInputMgr : IKeyPressListener, IMouseClickListener
    {
        void OnActionBarClick(Point point, MouseButtonEventArgs e);
    }
}
