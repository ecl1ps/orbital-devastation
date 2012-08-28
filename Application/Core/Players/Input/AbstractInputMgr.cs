using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Input;

namespace Orbit.Core.Players.Input
{
    public abstract class AbstractInputMgr : IInputMgr
    {
        protected ActionBarMgr actionBarMgr;

        public AbstractInputMgr(ActionBarMgr mgr)
        {
            actionBarMgr = mgr;
        }

        public virtual void OnKeyEvent(System.Windows.Input.KeyEventArgs e)
        {
            if (!e.IsDown)
                return;

            if (e.Key == (Key)int.Parse(GameProperties.Props.Get(PropertyKey.PLAYER_ACTION_1)))
                actionBarMgr.Click(0);
            else if (e.Key == (Key)int.Parse(GameProperties.Props.Get(PropertyKey.PLAYER_ACTION_2)))
                actionBarMgr.Click(1);
            else if (e.Key == (Key)int.Parse(GameProperties.Props.Get(PropertyKey.PLAYER_ACTION_3)))
                actionBarMgr.Click(2);
            else if (e.Key == (Key)int.Parse(GameProperties.Props.Get(PropertyKey.PLAYER_ACTION_4)))
                actionBarMgr.Click(3);
            else if (e.Key == (Key)int.Parse(GameProperties.Props.Get(PropertyKey.PLAYER_ACTION_5)))
                actionBarMgr.Click(4);

        }

        public virtual void OnCanvasClick(System.Windows.Point point, MouseButtonEventArgs e)
        {
        }

        public virtual void OnActionBarClick(System.Windows.Point point, MouseButtonEventArgs e)
        {
        }
    }
}
