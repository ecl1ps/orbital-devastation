using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Windows.Input;
using Orbit.Core.Client;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

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

            sceneMgr.InputMgr.AddClickListener(this);
            sceneMgr.InputMgr.AddMoveListener(this);
            sceneMgr.InputMgr.AddKeyListener(Keys.D1, this);
            sceneMgr.InputMgr.AddKeyListener(Keys.D2, this);
            sceneMgr.InputMgr.AddKeyListener(Keys.D3, this);
            sceneMgr.InputMgr.AddKeyListener(Keys.D4, this);
            sceneMgr.InputMgr.AddKeyListener(Keys.D5, this);
        }

        public virtual void OnCanvasClick(Client.GameStates.MouseButtons button, Vector2 point, bool down)
        {
        }

        public virtual void OnKeyEvent(Microsoft.Xna.Framework.Input.Keys key, bool down)
        {
            if (!down || sceneMgr.UserActionsDisabled)
                return;

            Console.WriteLine(key.ToString());


            /*
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
             */ 
        }

        public void OnActionBarClick(Vector2 point, MouseButtonEventArgs e)
        {
        }

        public virtual void OnMouseMove(Client.GameStates.MouseMoveEvent e)
        {
        }
    }
}
