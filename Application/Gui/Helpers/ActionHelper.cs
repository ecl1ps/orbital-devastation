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

namespace Orbit.Gui.Helpers
{
    public class ActionHelper
    {
        private SceneMgr mgr;

        private UIElement HealActionWindow;
        private UIElement HookActionWindow;

        public ActionHelper(SceneMgr manager)
        {
            mgr = manager;
        }

        public void CreateWeaponAction(IWeapon weapon)
        {
            switch (weapon.WeaponType)
            {
                case WeaponType.HOOK:
                    if (HookActionWindow == null || !HookActionWindow.IsVisible)
                        HookActionWindow = GuiObjectFactory.CreateWeaponAction(mgr, weapon, mgr.GetCurrentPlayer());
                    break;
            }
        }

        public void CreateHealAction(IHealingKit healingKit)
        {
            if (HealActionWindow == null || !HealActionWindow.IsVisible)
            {
                HealActionWindow = GuiObjectFactory.CreateHealingIcon(mgr, healingKit);
            }
        }

        public void HideWeaponAction(IWeapon weapon)
        {
            switch (weapon.WeaponType)
            {
                case WeaponType.HOOK:
                    if (HookActionWindow != null)
                        mgr.ActionBar.RemoveItem(HealActionWindow);
                    break;
            }
        }

        public void hideHealAction()
        {
            if (HealActionWindow != null)
                mgr.ActionBar.RemoveItem(HealActionWindow);
        }
    }

}
