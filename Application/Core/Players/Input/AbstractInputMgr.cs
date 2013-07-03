using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Windows.Input;
using Orbit.Core.Client;
using Microsoft.Xna.Framework;

namespace Orbit.Core.Players.Input
{
    public abstract class AbstractInputMgr : IInputMgr
    {
        protected ActionBarMgr actionBarMgr;
        protected SceneMgr sceneMgr;

        public AbstractInputMgr(ActionBarMgr actionBarMgr, SceneMgr sceneMgr)
        {
            this.actionBarMgr = actionBarMgr;
            this.sceneMgr = sceneMgr;

            sceneMgr.AddMouseListener(this);
            sceneMgr.AddKeyListener(this);
        }

        public virtual void OnKeyEvent(System.Windows.Input.KeyEventArgs e)
        {
            if (!e.IsDown || sceneMgr.UserActionsDisabled)
                return;

            if (e.Key == (Key)int.Parse(GameProperties.Props.Get(PropertyKey.PLAYER_ACTION_1), CultureInfo.InvariantCulture))
                actionBarMgr.Click(0);
            else if (e.Key == (Key)int.Parse(GameProperties.Props.Get(PropertyKey.PLAYER_ACTION_2), CultureInfo.InvariantCulture))
                actionBarMgr.Click(1);
            else if (e.Key == (Key)int.Parse(GameProperties.Props.Get(PropertyKey.PLAYER_ACTION_3), CultureInfo.InvariantCulture))
                actionBarMgr.Click(2);
            else if (e.Key == (Key)int.Parse(GameProperties.Props.Get(PropertyKey.PLAYER_ACTION_4), CultureInfo.InvariantCulture))
                actionBarMgr.Click(3);
            else if (e.Key == (Key)int.Parse(GameProperties.Props.Get(PropertyKey.PLAYER_ACTION_5), CultureInfo.InvariantCulture))
                actionBarMgr.Click(4);

        }

        public virtual void OnCanvasClick(Vector2 point, MouseButtonEventArgs e)
        {
        }

        public virtual void OnActionBarClick(Vector2 point, MouseButtonEventArgs e)
        {
        }
    }
}
