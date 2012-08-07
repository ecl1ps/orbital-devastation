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

namespace Orbit.Core.Players
{
    public class ActionBarMgr : IGameState
    {
        private SceneMgr mgr;

        public ActionBar ActionBar { get; set; }

        private BuyActionUC HealActionWindow;
        private BuyActionUC HookActionWindow;
        private BuyActionUC MineActionWindow;
        private BuyActionUC CannonActionWindow;

        public ActionBarMgr(SceneMgr manager)
        {
            mgr = manager;
        }

        public void CreateActionBarItems()
        {
            mgr.Invoke(new Action(() =>
            {
                ActionBar = LogicalTreeHelper.FindLogicalNode(Application.Current.MainWindow, "ActionBarUC") as ActionBar;
                HookActionWindow = GuiObjectFactory.CreateAndAddBuyActionWindow(mgr, ActionBar, new WeaponActionController(mgr, mgr.GetCurrentPlayer().Hook.Next(), mgr.GetCurrentPlayer()));
                MineActionWindow = GuiObjectFactory.CreateAndAddBuyActionWindow(mgr, ActionBar, new WeaponActionController(mgr, mgr.GetCurrentPlayer().Mine.Next(), mgr.GetCurrentPlayer()));
                CannonActionWindow = GuiObjectFactory.CreateAndAddBuyActionWindow(mgr, ActionBar, new WeaponActionController(mgr, mgr.GetCurrentPlayer().Canoon.Next(), mgr.GetCurrentPlayer()));
                HealActionWindow = GuiObjectFactory.CreateAndAddBuyActionWindow(mgr, ActionBar, new HealActionController(mgr, mgr.GetCurrentPlayer().HealingKit));
            }));
        }

        public void Update(float tpf)
        {
            // zatim ne pro spectatory
            if (!mgr.GetCurrentPlayer().IsActivePlayer())
                return;

            CheckForUpgrades();
        }

        private void CheckForUpgrades()
        {
            mgr.GetCurrentPlayer().ShowGold();

            if (mgr.GetCurrentPlayer().HealingKit.Cost <= mgr.GetCurrentPlayer().Data.Gold &&
                (SharedDef.BASE_MAX_INGERITY - mgr.GetCurrentPlayer().GetBaseIntegrity()) >= SharedDef.HEAL_AMOUNT)
                HealActionWindow.Activate();
            else
                HealActionWindow.Deactivate();

            if (mgr.GetCurrentPlayer().Hook.Next() != null && (mgr.GetCurrentPlayer().Hook.Next().Cost <= mgr.GetCurrentPlayer().Data.Gold))
                HookActionWindow.Activate();
            else
                HookActionWindow.Deactivate();

            if (mgr.GetCurrentPlayer().Canoon.Next() != null && (mgr.GetCurrentPlayer().Canoon.Next().Cost <= mgr.GetCurrentPlayer().Data.Gold))
                CannonActionWindow.Activate();
            else
                CannonActionWindow.Deactivate();

            if (mgr.GetCurrentPlayer().Mine.Next() != null && (mgr.GetCurrentPlayer().Mine.Next().Cost <= mgr.GetCurrentPlayer().Data.Gold))
                MineActionWindow.Activate();
            else
                MineActionWindow.Deactivate();
        }
    }

}
