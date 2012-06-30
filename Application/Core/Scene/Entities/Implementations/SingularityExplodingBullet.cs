using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Orbit.Core.Scene.Entities;
using Orbit.Core.Players;
using Orbit.Core.Scene.Controls;
using Lidgren.Network;
using Orbit.Core;
using Orbit.Core.Client;
using Orbit.Core.Helpers;
using System.Windows.Threading;
using System.Windows.Media;
using System.Windows.Shapes;
using Orbit.Core.Scene.Controls.Implementations;
using System.Windows;


namespace Orbit.Core.Scene.Entities.Implementations
{
    class SingularityExplodingBullet : SingularityBullet
    {
        public SingularityExplodingBullet(SceneMgr mgr) : base(mgr)
        {
        }

        protected override void UpdateGeometricState()
        {

            ((geometryElement as Path).Data as EllipseGeometry).RadiusX = Radius;
            ((geometryElement as Path).Data as EllipseGeometry).RadiusY = Radius;
        }

        public override void DoCollideWith(ICollidable other)
        {
            if (other is IMovable)
            {
                FiringSingularityControl c = GetControlOfType(typeof(FiringSingularityControl)) as FiringSingularityControl;
                if (c != null)
                    c.CollidedWith(other as IMovable);
            }
        }

        public void SpawnMinions()
        {
            CreateSmallBullet(Math.PI / 12);
            CreateSmallBullet(0);
            CreateSmallBullet(-Math.PI / 12);
        }

        private void CreateSmallBullet(double rotation)
        {
            SingularityBullet smallBullet = new SingularityBullet(SceneMgr);
            smallBullet.Id = IdMgr.GetNewId(SceneMgr.GetCurrentPlayer().GetId());
            smallBullet.Owner = Owner;
            Vector dir = Direction;
            dir.Normalize();
            smallBullet.Direction = dir.Rotate(rotation);
            smallBullet.Position = Center;
            smallBullet.Radius = 2;
            smallBullet.Damage = Damage/2;
            smallBullet.Color = Color;

            smallBullet.SetGeometry(SceneGeometryFactory.CreateConstantColorEllipseGeometry(smallBullet));

            LinearMovementControl nmc = new LinearMovementControl();
            nmc.InitialSpeed = (GetControlOfType(typeof(LinearMovementControl)) as LinearMovementControl).InitialSpeed;
            smallBullet.AddControl(nmc);

            SceneMgr.DelayedAttachToScene(smallBullet);
        }

        public override void WriteObject(NetOutgoingMessage msg)
        {
            msg.Write((int)PacketType.NEW_SINGULARITY_EXPLODING_BULLET);
            msg.WriteObjectSingularityBullet(this);
            msg.WriteControls(GetControlsCopy());
        }

        public override void ReadObject(NetIncomingMessage msg)
        {
            msg.ReadObjectSingularityBullet(this);
            IList<IControl> controls = msg.ReadControls();
            foreach (Control c in controls)
                AddControl(c);
        }
    }
}
