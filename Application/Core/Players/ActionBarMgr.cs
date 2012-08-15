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
using System.Windows.Media;
using Orbit.Gui.ActionControllers;
using Orbit.Core.SpecialActions;

namespace Orbit.Core.Players
{
    struct WindowController {
        public BuyActionUC Window { get; set; }
        public ActionController Controller { get; set; }

        public WindowController(BuyActionUC window, ActionController controller) : this() {
            Window = window;
            Controller = controller;
        }
    }

    public class ActionBarMgr : IGameState
    {
        private SceneMgr mgr;
        private List<WindowController> windows;

        public ActionBar ActionBar { get; set; }

        private BuyActionUC HealActionWindow;
        private BuyActionUC HookActionWindow;
        private BuyActionUC MineActionWindow;
        private BuyActionUC CannonActionWindow;

        public ActionBarMgr(SceneMgr manager)
        {
            mgr = manager;
            windows = new List<WindowController>();
        }

        public void CreateActionBarItems(List<ISpecialAction> actions)
        {
            mgr.Invoke(new Action(() =>
            {
                ActionBar = LogicalTreeHelper.FindLogicalNode(Application.Current.MainWindow, "ActionBarUC") as ActionBar;
                BuyActionUC window;
                ActionController controller;

                foreach (ISpecialAction action in actions)
                {
                    controller = new ActionControllerImpl(mgr, action);
                    window = GuiObjectFactory.CreateAndAddBuyActionWindow(mgr, ActionBar, controller);
                    windows.Add(new WindowController(window, controller));
                }
            }));
        }

        public void Update(float tpf)
        {
            foreach (WindowController windowController in windows)
            {
                if (windowController.Controller.Action.IsReady())
                    windowController.Window.Activate();
                else
                    windowController.Window.Deactivate();
            }
        }
    }

}
