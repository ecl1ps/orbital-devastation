using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Input;
using System.Windows;
using Orbit.Core.Client.Interfaces;
using Microsoft.Xna.Framework;

namespace Orbit.Core.Players.Input
{
    public interface IInputMgr : IKeyPressListener, IMouseClickListener, IMouseMoveListener
    {
        void OnActionBarClick(Vector2 point, MouseButtonEventArgs e);
    }
}
