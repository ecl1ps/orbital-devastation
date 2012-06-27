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
    public class PlayerActionManager : IGameState
    {
        private SceneMgr mgr;

        public ActionBar ActionBar { get; set; }

        private BuyActionUC HealActionWindow;
        private BuyActionUC HookActionWindow;
        private BuyActionUC MineActionWindow;
        private BuyActionUC CannonActionWindow;

        public PlayerActionManager(SceneMgr manager)
        {
            mgr = manager;
            CreateActionBar();
        }

        private void CreateActionBar()
        {
            mgr.Invoke(new Action(() =>
            {
                ActionBar = LogicalTreeHelper.FindLogicalNode(Application.Current.MainWindow, "ActionBarUC") as ActionBar;
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
                CreateHealAction();
            else
                RemoveHealAction();

            if (mgr.GetCurrentPlayer().Hook.Next() != null && (mgr.GetCurrentPlayer().Hook.Next().Cost <= mgr.GetCurrentPlayer().Data.Gold))
                CreateWeaponAction(mgr.GetCurrentPlayer().Hook.Next());
            else
                RemoveHookAction();

            if (mgr.GetCurrentPlayer().Canoon.Next() != null && (mgr.GetCurrentPlayer().Canoon.Next().Cost <= mgr.GetCurrentPlayer().Data.Gold))
                CreateWeaponAction(mgr.GetCurrentPlayer().Canoon.Next());
            else 
                RemoveCannonAction();
            if (mgr.GetCurrentPlayer().Mine.Next() != null && (mgr.GetCurrentPlayer().Mine.Next().Cost <= mgr.GetCurrentPlayer().Data.Gold))
                CreateWeaponAction(mgr.GetCurrentPlayer().Mine.Next());
            else
                RemoveMineAction();
        }

        private void RemoveMineAction()
        {
            if (MineActionWindow != null && MineActionWindow.IsVisible)
                MineActionWindow.Remove();
        }
        private void RemoveCannonAction()
        {
            if (CannonActionWindow != null && CannonActionWindow.IsVisible)
            {
                CannonActionWindow.Remove();
            }
        }

        private void RemoveHookAction()
        {
            if (HookActionWindow != null && HookActionWindow.IsVisible)
            {
                HookActionWindow.Remove();
            }
        }

        private void RemoveHealAction()
        {
            if(HealActionWindow != null && HealActionWindow.IsVisible) 
            {
                HealActionWindow.Remove();
            }
        }

        private void CreateWeaponAction(IWeapon weapon)
        {
            switch (weapon.WeaponType)
            {
                case WeaponType.HOOK:
                    if (HookActionWindow == null || !HookActionWindow.IsVisible)
                        HookActionWindow = GuiObjectFactory.CreateAndAddBuyActionWindow(mgr, ActionBar, new WeaponActionController(mgr, weapon, mgr.GetCurrentPlayer()));
                    break;
                case WeaponType.MINE:
                    if (MineActionWindow == null || !MineActionWindow.IsVisible)
                        MineActionWindow = GuiObjectFactory.CreateAndAddBuyActionWindow(mgr, ActionBar, new WeaponActionController(mgr, weapon, mgr.GetCurrentPlayer()));
                    break;
                case WeaponType.CANNON:
                    if (CannonActionWindow == null || !CannonActionWindow.IsVisible)
                        CannonActionWindow = GuiObjectFactory.CreateAndAddBuyActionWindow(mgr, ActionBar, new WeaponActionController(mgr, weapon, mgr.GetCurrentPlayer()));
                    break;
            }
        }

        private void CreateHealAction()
        {
            if (HealActionWindow == null || !HealActionWindow.IsVisible)
            {
                HealActionWindow = GuiObjectFactory.CreateAndAddBuyActionWindow(mgr, ActionBar,  new HealActionController(mgr, mgr.GetCurrentPlayer().HealingKit));
            }
        }

       /* public void HideWeaponAction(IWeapon weapon)
        {
            switch (weapon.WeaponType)
            {
                case WeaponType.HOOK:
                    if (HookActionWindow != null)
                        HealActionWindow.Remove();
                    break;
            }
        }

        private void HideHealAction()
        {
            if (HealActionWindow != null)
                HealActionWindow.Remove();
        }*/
    }

}
