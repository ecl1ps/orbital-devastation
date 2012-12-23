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
        public static ActionUC CreateAndAddBuyActionWindow(SceneMgr mgr, ActionBar actionbar, ActionController<ActionUC> controller)
        {
            ActionUC wnd = null;

            mgr.Invoke(new Action(() =>
            {
                wnd = ActionUC.CreateWindow(controller);
                actionbar.AddItem(wnd);
            }));

            return wnd;
        }
    }
}
