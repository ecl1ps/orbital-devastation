using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Orbit.src.Core.utils;
using Orbit.src.Core.Helpers;
using System.Windows;
using Orbit.Core.Scene;

namespace Orbit.src.Gui.Helpers
{
    public class ActionHelper
    {
        private static ActionHelper instance = null;

        private UIElement HealActionWindow;

        public void createHealAction(IHealingKit healingKit)
        {
            if (HealActionWindow == null || !HealActionWindow.IsVisible)
            {
                HealActionWindow = GuiObjectFactory.createHealingIcon(healingKit);
            }
        }

        public static ActionHelper getInstance()
        {
            if (instance == null)
                instance = new ActionHelper();

            return instance;
        }
    }

}
