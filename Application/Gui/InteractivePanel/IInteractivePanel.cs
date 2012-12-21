using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;

namespace Orbit.Gui.InteractivePanel
{
    interface IInteractivePanel
    {
        void AddItem(UIElement elem);

        void RemoveItem(UIElement elem);

        void ClearAll();
    }
}
