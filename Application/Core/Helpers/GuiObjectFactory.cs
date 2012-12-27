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

        public static HidingPanel CreateHidingPanel(SceneMgr mgr)
        {
            HidingPanel panel = null;

            mgr.Invoke(new Action(() =>
            {
                panel = new HidingPanel();
                mgr.GetCanvas().Children.Add(panel);
                Canvas.SetTop(panel, 100);
            }));

            return panel;
        }

        //TODO jak udelat <Object> nebo <?> 
        public static ActionOverview CreateAndAddActionOverview(SceneMgr mgr, IInteractivePanel<ActionOverview> panel, ISpectatorAction action) 
        {
            ActionOverview item = null;

            mgr.Invoke(new Action(() => {
                item = new ActionOverview();
                item.RenderAction(action);
                panel.AddItem(item);
            }));

            return item;
        }
    }
}
