using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Input;

namespace Orbit.Core.Client.Interfaces
{
    public interface IKeyPressListener
    {
        void OnKeyEvent(KeyEventArgs e);
    }
}
