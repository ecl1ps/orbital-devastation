using System;
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
using Lidgren.Network;

namespace Orbit.Core.SpecialActions.Spectator
{
    public class AsteroidSlow : SpectatorAction
    {
        protected float exactBonus;

        public AsteroidSlow(SceneMgr mgr, Players.Player owner)
            : base(mgr, owner)
        {
            Name = "Asteroid slow";
            Type = SpecialActionType.ASTEROID_SLOW;
            ImageSource = "pack://application:,,,/resources/images/icons/asteroid-slow-icon.png";

            //nastavime parametry
            this.Cooldown = 8; //sekundy
            this.CastingTime = 0.5f;
            this.CastingColor = Colors.Blue;
            this.Range = new Range(3);
            this.exactBonus = 1.2f;
        }

        private void Slow(Asteroid ast, bool exact)
        {
            IMovementControl c = ast.GetControlOfType<IMovementControl>();
            LinearRotationControl c1 = ast.GetControlOfType<LinearRotationControl>();
            if (c == null || c1 == null)
                return;

            float time = exact ? 1.5f * exactBonus : 1.5f;

            List<IControl> controls = new List<IControl>();
            controls.Add(c);
            controls.Add(c1);

            AsteroidFreezeControl removal = new AsteroidFreezeControl();
            removal.ControlsForDisabling = controls;
            removal.Time = time;

            ast.AddControl(removal);
            
            // led je posunut o pulku radiusu doleva
            Vector p = new Vector(ast.Position.X - (ast.Radius / 2), ast.Position.Y - (ast.Radius / 2));
            // potom jeho sirka musi byt prumer + 2 * posunuti (vlevo a vpravo) => r * (2 + 0.5 + 0.5) 
            IceSquare s = SceneObjectFactory.CreateIceSquare(SceneMgr, p, new Size(ast.Radius * 3, ast.Radius * 3)); 
            s.AddControl(new LimitedLifeControl(time));
            SceneMgr.DelayedAttachToScene(s);
        }

        public override void StartAction(List<Asteroid> afflicted, bool exact)
        {
            afflicted.ForEach(aff => { 
                Slow(aff, exact); 
            });
        }
    }
}
