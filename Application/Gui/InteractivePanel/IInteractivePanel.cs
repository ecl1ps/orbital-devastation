using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;

namespace Orbit.Gui.InteractivePanel
{
    interface IInteractivePanel<T>
    {
        void AddItem(UIElement elem);

        void RemoveItem(UIElement elem);

        T getItem(int i);

        void ClearAll();
    }
}
