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
            this.Range = new RangeGroup(new Range(AsteroidType.NORMAL, 2), new Range(AsteroidType.GOLDEN, 1), new Range(AsteroidType.UNSTABLE, 1), new Range(AsteroidType.SPAWNED, 1));
        }

        private void Slow(Asteroid ast)
        {
            IMovementControl c = ast.GetControlOfType<IMovementControl>();
            LinearRotationControl c1 = ast.GetControlOfType<LinearRotationControl>();
            if (c == null || c1 == null)
                return;

            List<IControl> controls = new List<IControl>();
            controls.Add(c);
            controls.Add(c1);

            AsteroidFreezeControl removal = new AsteroidFreezeControl();
            removal.ControlsForDisabling = controls;
            removal.Time = 1.5f;

            ast.AddControl(removal);
            
            // led je posunut o pulku radiusu doleva
            Vector p = new Vector(ast.Position.X - (ast.Radius / 2), ast.Position.Y - (ast.Radius / 2));
            // potom jeho sirka musi byt prumer + 2 * posunuti (vlevo a vpravo) => r * (2 + 0.5 + 0.5) 
            IceSquare s = SceneObjectFactory.CreateIceSquare(SceneMgr, p, new Size(ast.Radius * 3, ast.Radius * 3)); 
            s.AddControl(new LimitedLifeControl(1.5f));
            SceneMgr.DelayedAttachToScene(s);
        }

        public override void StartAction(List<Asteroid> afflicted)
        {
            NetOutgoingMessage msg = SceneMgr.CreateNetMessage();
            msg.Write((int)PacketType.ASTEROID_SLOW_START);

            msg.Write(Owner.GetId());
            msg.Write(afflicted.Count);
            msg.Write(3.0f);

            afflicted.ForEach(aff => { 
                Slow(aff); 
                msg.Write(aff.Id);
            });

            SceneMgr.SendMessage(msg);
        }
    }
}
