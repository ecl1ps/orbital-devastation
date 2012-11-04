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
    public class AsteroidThrow : SpecialAction
    {
        protected MiningModuleControl Control;

        public AsteroidThrow(MiningModuleControl control, SceneMgr mgr, Players.Player owner)
            : base(mgr, owner)
        {
            Name = "Asteroid throw";
            ImageSource = "pack://application:,,,/resources/images/icons/asteroid-throw-icon.png";
            Type = SpecialActionType.ASTEROID_THROW;
            Control = control;
            Cost = 500;
        }

        public override void StartAction()
        {
            base.StartAction();

            Vector v = new Vector();
            v = new Vector(Control.Position.X, SharedDef.CANVAS_SIZE.Height) - Control.Position;
            v.Normalize();

            int count = 0;

            foreach (MiningObject afflicted in Control.currentlyMining)
            {
                if (afflicted.Obj is Asteroid)
                {
                    IMovementControl control = afflicted.Obj.GetControlOfType<IMovementControl>();
                    if (control != null)
                        control.Speed = SharedDef.SPECTATOR_ASTEROID_THROW_SPEED;

                    (afflicted.Obj as Asteroid).Direction = v;
                    count++;
                }
            }

            NetOutgoingMessage msg = SceneMgr.CreateNetMessage();
            msg.Write((int) PacketType.ASTEROIDS_DIRECTIONS_CHANGE);
            msg.Write(count);

            foreach (MiningObject afflicted in Control.currentlyMining)
            {
                if (afflicted.Obj is Asteroid)
                {
                    msg.Write((afflicted.Obj as Asteroid).Id);
                    msg.Write((afflicted.Obj as Asteroid).Direction);
                }
            }

            SceneMgr.SendMessage(msg);
            
        }

        public override bool IsReady()
        {
            return Owner.Data.Gold >= Cost && Control.currentlyMining.Count != 0;
        }
    }
}
