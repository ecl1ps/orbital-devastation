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

namespace Orbit.Core.Helpers
{
    class GuiObjectFactory
    {
        public static HealActionWindow CreateAndAddHealingIcon(SceneMgr mgr, ActionBar actionbar, IHealingKit healingKit)
        {
            HealActionWindow goldAction = null;

            mgr.Invoke(new Action(() =>
            {
                goldAction = new HealActionWindow(healingKit);
                actionbar.AddItem(goldAction);
            }));

            return goldAction;
        }

        public static WeaponActionWindow CreateAndAddWeaponAction(SceneMgr mgr, ActionBar actionbar, IWeapon weapon, Player player)
        {
            WeaponActionWindow goldAction = null;

            mgr.Invoke(new Action(() =>
            {
                goldAction = new WeaponActionWindow(weapon, player);
                actionbar.AddItem(goldAction);
            }));

            return goldAction;
        }
    }
}
