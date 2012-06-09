using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using Orbit.src.Gui;
using Orbit.src.Core.utils;
using Orbit.Core.Scene;
using System.Windows.Threading;

namespace Orbit.src.Core.Helpers
{
    class GuiObjectFactory
    {
        public static UIElement createHealingIcon(IHealingKit healingKit) {
            GoldActionWindow goldAction = null;

            SceneMgr.GetInstance().GetUIDispatcher().Invoke(DispatcherPriority.Send, new Action(() =>
            {
                goldAction = new HealActionWindow(healingKit);
                SceneMgr.GetInstance().ActionBar.addItem(goldAction);
            }));
            

            return goldAction;
        }
    }
}
