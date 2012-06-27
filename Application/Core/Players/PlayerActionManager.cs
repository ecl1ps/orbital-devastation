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

        private BuyActionWindow HealActionWindow;
        private BuyActionWindow HookActionWindow;
        private BuyActionWindow CannonActionWindow;

        public PlayerActionManager(SceneMgr manager)
        {
            mgr = manager;
            CreateActionBar();
        }

        private void CreateActionBar()
        {
            mgr.Invoke(new Action(() =>
            {
                ActionBar = new ActionBar();
                ActionBar.RenderTransform = new ScaleTransform(0.8, 0.8);
                mgr.GetCanvas().Children.Add(ActionBar);
                Canvas.SetLeft(ActionBar, 10);
                Canvas.SetTop(ActionBar, mgr.ViewPortSizeOriginal.Height * (SharedDef.ACTION_BAR_TOP_MARGIN_PCT));
                Canvas.SetZIndex(ActionBar, 100);
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
