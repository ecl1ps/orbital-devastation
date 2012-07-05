using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Input;
using System.Windows;
using Orbit.Core.Client;

namespace Orbit.Core.Players.Input
{
    public class SpectatorInputMgr : IInputMgr
    {
        private Player plr;
        private SceneMgr mgr;

        public SpectatorInputMgr(Player p, SceneMgr sceneMgr)
        {
            plr = p;
            mgr = sceneMgr;
        }

        public void OnCanvasClick(Point point, MouseButtonEventArgs e)
        {

        }

        public void OnActionBarClick(Point point, MouseButtonEventArgs e)
        {

        }

        public void OnKeyEvent(KeyEventArgs e)
        {

        }
    }
}
