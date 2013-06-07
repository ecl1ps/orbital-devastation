using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Input;

namespace Orbit.Core.Weapons
{
    public interface IWeaponClickListener
    {
        /// <summary>
        ///  V případě že funkce vrátí TRUE přestane weapon vyhodnocovat event
        /// </summary>
        bool ProccessClickEvent(Point point, MouseButton button, MouseButtonState buttonState);
    }
}
