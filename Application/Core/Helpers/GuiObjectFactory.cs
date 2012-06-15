using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using Orbit.Gui;
using Orbit.Core.Utils;
using Orbit.Core.Scene;
using System.Windows.Threading;
using Orbit.Core.Weapons;
using Orbit.Core.Players;
using Orbit.Core.Client;
using Orbit.Gui.ActionControllers;

namespace Orbit.Core.Helpers
{
    class GuiObjectFactory
    {
        public static BuyActionWindow CreateAndAddBuyActionWindow(SceneMgr mgr, ActionBar actionbar, ActionController controller)
        {
            BuyActionWindow wnd = null;

            mgr.Invoke(new Action(() =>
            {
                wnd = BuyActionWindow.CreateWindow(controller);
                actionbar.AddItem(wnd);
            }));

            return wnd;
        }
    }
}
