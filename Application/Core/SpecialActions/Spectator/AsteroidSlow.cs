﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Orbit.Core.Client;
using Orbit.Core.Scene.Entities.Implementations;
using Orbit.Core.Scene.Controls;
using Orbit.Core.Scene.Controls.Implementations;
using Orbit.Core.Helpers;
using System.Windows;
using Orbit.Core.Client.Shaders;
using System.Windows.Media;

namespace Orbit.Core.SpecialActions.Spectator
{
    public class AsteroidSlow : SpectatorAction
    {
        public AsteroidSlow(SceneMgr mgr, Players.Player owner)
            : base(mgr, owner)
        {
            Name = "Asteroid slow";
            Type = SpecialActionType.ASTEROID_SLOW;
            ImageSource = "pack://application:,,,/resources/images/icons/asteroid-slow-icon.png";

            //nastavime parametry
            this.Cooldown = 8; //sekundy
            this.Range = new RangeGroup(new Range(AsteroidType.NORMAL, 2), new Range(AsteroidType.GOLDEN, 2));
        }

        private void Slow(Asteroid ast)
        {
            IMovementControl c = ast.GetControlOfType<IMovementControl>();
            LinearRotationControl c1 = ast.GetControlOfType<LinearRotationControl>();
            if(c == null)
                return;

            List<IControl> controls = new List<IControl>();
            controls.Add(c);
            controls.Add(c1);

            AsteroidFreezeControl removal = new AsteroidFreezeControl();
            removal.ToRemove = controls;
            removal.Time = 3;

            ast.AddControl(removal);
            
            // led je posunut o pulku radiusu doleva
            Vector p = new Vector(ast.Position.X - (ast.Radius / 2), ast.Position.Y - (ast.Radius / 2));
            // potom jeho sirka musi byt prumer + 2 * posunuti (vlevo a vpravo) => r * (2 + 0.5 + 0.5) 
            IceSquare s = SceneObjectFactory.CreateIceSquare(SceneMgr, p, new Size(ast.Radius * 3, ast.Radius * 3)); 
            s.AddControl(new LimitedLifeControl(3));
            SceneMgr.DelayedAttachToScene(s);
        }

        protected override void StartAction(List<Asteroid> afflicted)
        {
            afflicted.ForEach(a => Slow(a));
        }
    }
}
