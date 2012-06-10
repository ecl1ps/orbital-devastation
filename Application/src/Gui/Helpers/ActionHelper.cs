using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Orbit.src.Core.utils;
using Orbit.src.Core.Helpers;
using System.Windows;
using Orbit.Core.Scene;
using Orbit.Core.Weapons;
using Orbit.Core.Players;

namespace Orbit.src.Gui.Helpers
{
    public class ActionHelper
    {
        private static ActionHelper instance = null;

        private UIElement HealActionWindow;
        private UIElement HookActionWindow;

        public void CreateWeaponAction(IWeapon weapon, Player player)
        {
            switch (weapon.WeaponType)
            {
                case WeaponType.HOOK:
                    if (HookActionWindow == null || !HookActionWindow.IsVisible)
                        HookActionWindow = GuiObjectFactory.createWeaponAction(weapon, player);
                    break;
            }
        }

        public void createHealAction(IHealingKit healingKit)
        {
            if (HealActionWindow == null || !HealActionWindow.IsVisible)
            {
                HealActionWindow = GuiObjectFactory.createHealingIcon(healingKit);
            }
        }

        public void hideWeaponAction(IWeapon weapon)
        {
            switch (weapon.WeaponType)
            {
                case WeaponType.HOOK:
                    if(HookActionWindow != null)
                        SceneMgr.GetInstance().ActionBar.removeItem(HealActionWindow);
                    break;
            }
        }

        public void hideHealAction()
        {
            if(HealActionWindow != null)
                SceneMgr.GetInstance().ActionBar.removeItem(HealActionWindow);
        }

        public static ActionHelper getInstance()
        {
            if (instance == null)
                instance = new ActionHelper();

            return instance;
        }
    }

}
