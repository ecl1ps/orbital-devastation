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
            plr.Mine.ProccessClickEvent(point, e.ChangedButton, e.ButtonState);
            plr.Hook.ProccessClickEvent(point, e.ChangedButton, e.ButtonState);
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
