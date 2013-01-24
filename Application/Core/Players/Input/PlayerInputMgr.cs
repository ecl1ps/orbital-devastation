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

            emmitor = new ParticleEmmitor(sceneMgr, IdMgr.GetNewId(plr.GetId()));
            emmitor.EmitingTime = 2f;
            emmitor.EmmitingDirection = new Vector(0, -1);
            emmitor.MinAngle = (float)(-Math.PI / 20);
            emmitor.MaxAngle = (float)(Math.PI / 20);
            emmitor.MinForce = 30;
            emmitor.MaxForce = 50;
            emmitor.MinLife = 3f;
            emmitor.MaxLife = 3.5f;
            emmitor.Position = new Vector(500, 500);
            emmitor.MinSize = 3;
            emmitor.MaxSize = 4;
            emmitor.MaxAlpha = 0.6f;
            emmitor.Amount = 80;
            emmitor.Infinite = true;
            emmitor.Enabled = false;

            ParticleSphereFactory f = new ParticleSphereFactory();
            f.Color = Color.FromArgb(20, 0, 0, 0);
            emmitor.Factory = f;

            sceneMgr.DelayedAttachToScene(emmitor);

            ParticleEmmitor e = new ParticleEmmitor(sceneMgr, IdMgr.GetNewId(plr.GetId()));
            e.EmitingTime = 2f;
            e.EmmitingDirection = new Vector(0, -1);
            e.MinAngle = (float)(-Math.PI / 20);
            e.MaxAngle = (float)(Math.PI / 20);
            e.MinForce = 30;
            e.MaxForce = 50;
            e.MinLife = 3f;
            e.MaxLife = 3.5f;
            e.Position = new Vector(200, 500);
            e.MinSize = 3;
            e.MaxSize = 4;
            e.MaxAlpha = 0.6f;
            e.Amount = 100;
            e.Infinite = true;
            e.Enabled = false;


            ParticleSphereFactory fa = new ParticleSphereFactory();
            fa.Color = Color.FromArgb(20, 0, 0, 0);
            e.Factory = fa;

            sceneMgr.DelayedAttachToScene(e);
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
