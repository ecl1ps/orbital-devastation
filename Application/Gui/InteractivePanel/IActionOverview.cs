using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Orbit.Core.SpecialActions.Spectator;

namespace Orbit.Gui.InteractivePanel
{
    interface IActionOverview
    {
        void RenderAction(ISpectatorAction action);
    }
}
