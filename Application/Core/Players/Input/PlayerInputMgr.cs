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

        public PlayerInputMgr(Player p, SceneMgr sceneMgr, ActionBarMgr actionMgr) : base(actionMgr, sceneMgr) 
        {
            plr = p;
        }

        public override void OnKeyEvent(KeyEventArgs e)
        {
            base.OnKeyEvent(e);
            if (e.Key == Key.A)
            {
                ParticleEmmitor emmitor = new ParticleEmmitor(sceneMgr, IdMgr.GetNewId(plr.GetId()));
                emmitor.EmitingTime = 10;
                emmitor.EmmitingDirection = new Vector(1, 1);
                emmitor.MinAngle = (float) -Math.PI;
                emmitor.MaxAngle = (float) Math.PI;
                emmitor.MinForce = 50;
                emmitor.MaxForce = 70;
                emmitor.MinLife = 1;
                emmitor.MaxLife = 3;
                emmitor.Position = new Vector(500, 500);
                emmitor.MinSize = 2;
                emmitor.MaxSize = 6;
                emmitor.MaxAlpha = 0.6f;
                emmitor.Amount = 5;
                emmitor.FireAll = true;

                ParticleSphereFactory f = new ParticleSphereFactory();
                f.Color = Colors.Red;
                emmitor.Factory = f;

                sceneMgr.DelayedAttachToScene(emmitor);

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
