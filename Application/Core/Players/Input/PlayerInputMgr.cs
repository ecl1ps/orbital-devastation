using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Input;
using System.Windows;
using Orbit.Core.Client;
using Orbit.Core.Scene.Particles.Implementations;
using System.Windows.Media;

namespace Orbit.Core.Players.Input
{
    public class PlayerInputMgr : AbstractInputMgr
    {
        private Player plr;
        private ParticleEmmitor emmitor;

        public PlayerInputMgr(Player p, SceneMgr sceneMgr, ActionBarMgr actionMgr) : base(actionMgr, sceneMgr) 
        {
            plr = p;
        }

        public override void OnKeyEvent(KeyEventArgs e)
        {
            base.OnKeyEvent(e);
            if (e.Key == Key.A)
            {
                emmitor.Enabled = true;
            }
        }

        public override void OnCanvasClick(Point point, MouseButtonEventArgs e)
        {
            if (sceneMgr.UserActionsDisabled)
                return;

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
