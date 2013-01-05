using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Orbit.Core.Client;
using Orbit.Core.Scene.Controls.Implementations;
using Orbit.Core.Scene.Entities;
using System.Windows;
using Orbit.Core.Scene.Entities.Implementations;
using Lidgren.Network;
using Orbit.Core.Helpers;
using Orbit.Core.Scene.Controls;

namespace Orbit.Core.SpecialActions.Spectator
{
    public class AsteroidThrow : SpectatorAction
    {
        public AsteroidThrow(SceneMgr mgr, Players.Player owner, params ISpectatorAction[] actions)
            : base(mgr, owner, actions)
        {
            Name = "Asteroid throw";
            ImageSource = "pack://application:,,,/resources/images/icons/asteroid-throw-icon.png";
            Type = SpecialActionType.ASTEROID_THROW;

            //nastavime parametry
            this.Cooldown = 4; //sec
            this.Range = new RangeGroup(new Range(AsteroidType.NORMAL, 1), new Range(AsteroidType.GOLDEN, 1));
        }

        protected override void StartAction(List<Asteroid> afflicted)
        {
            Vector v = new Vector();
            v = new Vector(control.Position.X, SharedDef.CANVAS_SIZE.Height) - control.Position;
            v.Normalize();

            int count = 0;

            foreach (Asteroid ast in afflicted)
            {
                    IMovementControl mc = ast.GetControlOfType<IMovementControl>();
                    if (mc != null)
                        mc.Speed = SharedDef.SPECTATOR_ASTEROID_THROW_SPEED;

                    ast.Direction = v;
                    count++;
            }

            NetOutgoingMessage msg = SceneMgr.CreateNetMessage();
            msg.Write((int)PacketType.ASTEROIDS_DIRECTIONS_CHANGE);
            msg.Write(count);

            foreach (Asteroid ast in afflicted)
            {
                msg.Write(ast.Id);
                msg.Write(ast.Direction);
            }

            SceneMgr.SendMessage(msg);
        }
    }
}
