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
        public AsteroidThrow(SceneMgr mgr, Players.Player owner)
            : base(mgr, owner)
        {
            Name = "Asteroid throw";
            ImageSource = "pack://application:,,,/resources/images/icons/asteroid-throw-icon.png";
            Type = SpecialActionType.ASTEROID_THROW;

            //nastavime parametry
            this.Cooldown = 4; //sec
            this.Normal = new RangeGroup(AsteroidType.NORMAL, new Range(2, 3));
            this.Gold = new RangeGroup(AsteroidType.GOLDEN, new Range(1, 2));
        }

        public override void StartAction()
        {
            if (!control.Enabled)
                return;

            base.StartAction();

            Vector v = new Vector();
            v = new Vector(control.Position.X, SharedDef.CANVAS_SIZE.Height) - control.Position;
            v.Normalize();

            int count = 0;

            foreach (MiningObject afflicted in control.currentlyMining)
            {
                if (afflicted.Obj is Asteroid)
                {
                    IMovementControl mc = afflicted.Obj.GetControlOfType<IMovementControl>();
                    if (mc != null)
                        mc.Speed = SharedDef.SPECTATOR_ASTEROID_THROW_SPEED;

                    (afflicted.Obj as IMovable).Direction = v;
                    count++;
                }
            }

            NetOutgoingMessage msg = SceneMgr.CreateNetMessage();
            msg.Write((int) PacketType.ASTEROIDS_DIRECTIONS_CHANGE);
            msg.Write(count);

            foreach (MiningObject afflicted in lockedObjects)
            {
                if (afflicted.Obj is Asteroid)
                {
                    msg.Write((afflicted.Obj as ISceneObject).Id);
                    msg.Write((afflicted.Obj as IMovable).Direction);
                }
            }

            SceneMgr.SendMessage(msg);
            
        }
    }
}
