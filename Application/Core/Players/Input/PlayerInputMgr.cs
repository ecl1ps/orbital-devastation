using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Input;
using System.Windows;
using Orbit.Core.Client;

namespace Orbit.Core.Players.Input
{
    public class PlayerInputMgr : AbstractInputMgr
    {
        private Player plr;
        private SceneMgr mgr;

        public PlayerInputMgr(Player p, SceneMgr sceneMgr, ActionBarMgr actionMgr) : base(actionMgr) 
        {
            plr = p;
            mgr = sceneMgr;
        }

        public override void OnCanvasClick(Point point, MouseButtonEventArgs e)
        {
            base.OnCanvasClick(point, e);
            if (e.ChangedButton == MouseButton.Left)
                plr.Mine.ProccessClickEvent(point, e.ChangedButton, e.ButtonState);
            else if (e.ChangedButton == MouseButton.Middle)
                plr.Hook.ProccessClickEvent(point, e.ChangedButton, e.ButtonState);
            else if (e.ChangedButton == MouseButton.Right)
                plr.Canoon.ProccessClickEvent(point, e.ChangedButton, e.ButtonState);
        }
    }
}
