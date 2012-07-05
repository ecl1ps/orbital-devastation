using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Input;
using System.Windows;
using Orbit.Core.Client;

namespace Orbit.Core.Players.Input
{
    public class PlayerInputMgr : IInputMgr
    {
        private Player plr;
        private SceneMgr mgr;

        public PlayerInputMgr(Player p, SceneMgr sceneMgr)
        {
            plr = p;
            mgr = sceneMgr;
        }

        public void OnCanvasClick(Point point, MouseButtonEventArgs e)
        {
            if (e.ChangedButton == MouseButton.Left)
                plr.Mine.ProccessClickEvent(point, e.ChangedButton, e.ButtonState);
            else if (e.ChangedButton == MouseButton.Middle)
                plr.Hook.ProccessClickEvent(point, e.ChangedButton, e.ButtonState);
            else if (e.ChangedButton == MouseButton.Right)
                plr.Canoon.ProccessClickEvent(point, e.ChangedButton, e.ButtonState);
        }

        public void OnActionBarClick(Point point, MouseButtonEventArgs e)
        {

        }

        public void OnKeyEvent(KeyEventArgs e)
        {

        }
    }
}
