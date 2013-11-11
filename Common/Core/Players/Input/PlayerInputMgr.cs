using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using Orbit.Core.Client;
using Microsoft.Xna.Framework;
using Orbit.Core.Client.GameStates;

namespace Orbit.Core.Players.Input
{
    public class PlayerInputMgr : AbstractInputMgr
    {
        private Player plr;

        public PlayerInputMgr(Player p, SceneMgr sceneMgr, ActionBarMgr actionMgr) : base(actionMgr, sceneMgr) 
        {
            plr = p;
        }

        public override void OnCanvasClick(Client.GameStates.MouseButtons button, Vector2 point, bool down)
        {
            if (sceneMgr.UserActionsDisabled)
                return;

            base.OnCanvasClick(button, point, down);
            if (button == MouseButtons.Left)
                plr.Mine.ProccessClickEvent(point, button, down);
            else if (button == MouseButtons.Middle)
                plr.Hook.ProccessClickEvent(point, button, down);
            else if (button == MouseButtons.Right)
                plr.Canoon.ProccessClickEvent(point, button, down);
        }
    }
}
