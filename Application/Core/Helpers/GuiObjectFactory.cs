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
using Orbit.Core.SpecialActions.Spectator;
using Orbit.Gui.InteractivePanel;
using System.Windows.Controls;

namespace Orbit.Core.Helpers
{
    class GuiObjectFactory
    {
        public static ActionUC CreateAndAddActionUC(SceneMgr mgr, ActionBar actionbar, ActionController<ActionUC> controller)
        {
            ActionUC wnd = null;

            mgr.Invoke(new Action(() =>
            {
                wnd = ActionUC.CreateWindow(controller);
                actionbar.AddItem(wnd);
            }));

            return wnd;
        }

        public static AlertBox CreateAndAddAlertBox(SceneMgr mgr, Vector position)
        {
            AlertBox box = new AlertBox();
                
            mgr.GetCanvas().Children.Add(box);
            Canvas.SetLeft(box, position.X);
            Canvas.SetTop(box, position.Y);
            Canvas.SetZIndex(box, 100);

            return box;
        }
    }
}
