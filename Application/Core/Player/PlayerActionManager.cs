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

namespace Orbit.Core.Players
{
    public class PlayerActionManager : IGameState
    {
        private SceneMgr mgr;

        public ActionBar ActionBar { get; set; }

        private HealActionWindow HealActionWindow;
        private WeaponActionWindow HookActionWindow;

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
                ActionBar.RenderTransform = new ScaleTransform(0.7, 0.7);
                mgr.GetCanvas().Children.Add(ActionBar);
                Canvas.SetLeft(ActionBar, 10);
                Canvas.SetTop(ActionBar, mgr.ViewPortSizeOriginal.Height * (SharedDef.ACTION_BAR_TOP_MARGIN_PCT));
                Canvas.SetZIndex(ActionBar, 100);
            }));
        }

        public void Update(float tpf)
        {
            CheckForUpgrades();
        }

        private void CheckForUpgrades()
        {
            mgr.GetCurrentPlayer().ShowGold();

            if (mgr.GetCurrentPlayer().HealingKit.Cost <= mgr.GetCurrentPlayer().Data.Gold &&
                (SharedDef.BASE_MAX_INGERITY - mgr.GetCurrentPlayer().GetBaseIntegrity()) >= SharedDef.HEAL_AMOUNT)
                CreateHealAction();

            if (mgr.GetCurrentPlayer().Hook.Next() != null && (mgr.GetCurrentPlayer().Hook.Next().Cost <= mgr.GetCurrentPlayer().Data.Gold))
                CreateWeaponAction(mgr.GetCurrentPlayer().Hook.Next());
        }

        private void CreateWeaponAction(IWeapon weapon)
        {
            switch (weapon.WeaponType)
            {
                case WeaponType.HOOK:
                    if (HookActionWindow == null || !HookActionWindow.IsVisible)
                        HookActionWindow = GuiObjectFactory.CreateAndAddWeaponAction(mgr, ActionBar, weapon, mgr.GetCurrentPlayer());
                    break;
            }
        }

        private void CreateHealAction()
        {
            if (HealActionWindow == null || !HealActionWindow.IsVisible)
            {
                HealActionWindow = GuiObjectFactory.CreateAndAddHealingIcon(mgr, ActionBar, mgr.GetCurrentPlayer().HealingKit);
            }
        }

        public void HideWeaponAction(IWeapon weapon)
        {
            switch (weapon.WeaponType)
            {
                case WeaponType.HOOK:
                    if (HookActionWindow != null)
                        mgr.Invoke(new Action(() =>
                        {
                            ActionBar.RemoveItem(HookActionWindow);
                        }));
                    break;
            }
        }

        private void HideHealAction()
        {
            if (HealActionWindow != null)
                mgr.Invoke(new Action(() =>
                {
                    ActionBar.RemoveItem(HealActionWindow);
                }));
        }
    }

}
