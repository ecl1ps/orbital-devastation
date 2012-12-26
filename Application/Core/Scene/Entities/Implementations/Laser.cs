using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Orbit.Core.Players;
using System.Windows.Media;
using System.Windows;
using Orbit.Core.Client;
using Orbit.Core.Helpers;
using Orbit.Core.Scene.Controls.Implementations;
using Lidgren.Network;
using Orbit.Core.Scene.CollisionShapes;

namespace Orbit.Core.Scene.Entities.Implementations
{
    class Laser : Line, IProjectile, ISendable
    {
        public Player Owner { get; set; }

        public Laser(Player owner, SceneMgr mgr, long id)
            : base(mgr, id)
        {
            Owner = owner;
        }

        public Laser(Player owner, long id, SceneMgr mgr, Vector start, Vector end, Color color, int width) 
            : base(mgr, id, start, end, color, width)
        {
            Owner = owner;
        }

        public void WriteObject(NetOutgoingMessage msg)
        {
            msg.Write((int)PacketType.NEW_LASER);
            msg.WriteObjectLaser(this);
        }

        public void ReadObject(NetIncomingMessage msg)
        {
            msg.ReadObjectLaser(this);

            LineCollisionShape cs = new LineCollisionShape();
            cs.Start = Start;
            cs.End = End;
            CollisionShape = cs;
            
            CreateLine();
        }
    }
}
