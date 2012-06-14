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

namespace Orbit.Core.Helpers
{
    class GuiObjectFactory
    {
        public static UIElement CreateHealingIcon(SceneMgr mgr, IHealingKit healingKit)
        {
            GoldActionWindow goldAction = null;

            mgr.Invoke(new Action(() =>
            {
                goldAction = new HealActionWindow(healingKit);
                mgr.ActionBar.AddItem(goldAction);
            }));

            return goldAction;
        }

        public static GoldActionWindow CreateWeaponAction(SceneMgr mgr, IWeapon weapon, Player player)
        {
            GoldActionWindow goldAction = null;

            mgr.Invoke(new Action(() =>
            {
                goldAction = new WeaponActionWindow(weapon, player);
                mgr.ActionBar.AddItem(goldAction);
            }));

            return goldAction;
        }
    }
}
