using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Orbit.Core.Utils;
using Orbit.Core.Helpers;
using System.Windows;
using Orbit.Core.Scene;
using Orbit.Core.Weapons;
using Orbit.Core.Players;
using Orbit.Core.Client;
using Orbit.Gui;
using System.Windows.Controls;
using Microsoft.Xna.Framework;
using Orbit.Gui.ActionControllers;
using Orbit.Core.SpecialActions;

namespace Orbit.Core.Players
{
    public class WindowController {
        public ActionUC Window { get; set; }
        public ActionController<ActionUC> Controller { get; set; }

        public WindowController(ActionUC window, ActionController<ActionUC> controller)
        {
            Window = window;
            Controller = controller;
        }
    }

    public class ActionBarMgr : IGameState
    {
        private SceneMgr mgr;
        private List<WindowController> windows;
        private List<ISpecialAction> actions;

        public ActionBar ActionBar { get; set; }

        public ActionBarMgr(SceneMgr manager)
        {
            mgr = manager;
            windows = new List<WindowController>();
        }

        public void CreateActionBarItems(List<ISpecialAction> actions, bool spectator)
        {
            this.actions = actions;
            mgr.Invoke(new Action(() =>
            {
                ActionBar = LogicalTreeHelper.FindLogicalNode(Application.Current.MainWindow, "ActionBarUC") as ActionBar;
                ActionUC window;
                ActionController<ActionUC> controller;

                foreach (ISpecialAction action in actions)
                {
                    controller = spectator ? new SpectatorActionController(mgr, action) : new ActionControllerImpl(mgr, action);
                    window = GuiObjectFactory.CreateAndAddActionUC(mgr, ActionBar, controller);
                    windows.Add(new WindowController(window, controller));
                }
            }));
        }

        public void Update(float tpf)
        {
            mgr.GetCurrentPlayer().ShowGold();
            actions.ForEach(action => action.Update(tpf));

            for (int i = 0; i < windows.Count; ++i)
            {
                WindowController windowController = windows[i];
                windowController.Controller.Update(windowController.Window, tpf);
                if (windowController.Controller.Action.IsReady())
                    windowController.Window.Activate();
                else
                    windowController.Window.Deactivate();
            }
        }

        public void Click(int index)
        {
            if (index < windows.Count)
                windows[index].Controller.ActionClicked(windows[index].Window);
        }

        public void SwitchAction(ISpecialAction oldAction, ISpecialAction newAction)
        {
            WindowController c = windows.Find(w => w.Controller.Action == oldAction);
            c.Controller = new ActionControllerImpl(mgr, newAction);
            c.Window.AttachNewController(c.Controller);

            actions.Remove(oldAction);
            actions.Add(newAction);
        }
    }

}
