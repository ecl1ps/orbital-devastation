using System;
using System.Windows;
using Orbit.Core.Scene.Controls;
using System.Windows.Threading;
using System.Windows.Media;
using Lidgren.Network;
using System.Collections.Generic;
using Orbit.Core.Players;
using Orbit.Core.Client;
using Orbit.Core.Scene.Controls.Implementations;
using Orbit.Core.Helpers;
using Orbit.Core.Scene.CollisionShapes;
using Orbit.Gui.Visuals;

namespace Orbit.Core.Scene.Entities.Implementations
{
    public class SingularityMine : Sphere, ISendable, IProjectile
    {
        public bool IsVisible { get; set; } // neposilan - pouzivano pouze controlem SingularityControl
        public Player Owner { get; set; } // neposilan
        public Brush BorderBrush { get; set; } // neposilan
        public Brush FillBrush { get; set; } // neposilan

        public SingularityMine(SceneMgr mgr, long id)
            : base(mgr, id)
        {
            BorderBrush = Brushes.Black;
            FillBrush = Brushes.Black;
            Category = DrawingCategory.PROJECTILES;
        }

        protected override void UpdateGeometricState()
        {
            /*
            EllipseGeometry elipse = (geometryElement.Children[0] as GeometryDrawing).Geometry as EllipseGeometry;
            elipse.RadiusX = Radius;
            elipse.RadiusY = Radius;
            (geometryElement.Children[0] as GeometryDrawing).Pen = new Pen(BorderBrush, 1);
            (geometryElement.Children[0] as GeometryDrawing).Brush = FillBrush;
             */
        }

        public void WriteObject(NetOutgoingMessage msg)
        {
            msg.Write((int)PacketType.NEW_SINGULARITY_MINE);
            msg.WriteObjectSingularityMine(this);
            msg.WriteControls(GetControlsCopy());
        }

        public void ReadObject(NetIncomingMessage msg)
        {
            msg.ReadObjectSingularityMine(this);

            SphereCollisionShape cs = new SphereCollisionShape();
            cs.Center = Center;
            cs.Radius = Radius;
            CollisionShape = cs;

            IList<IControl> controls = msg.ReadControls();
            foreach (Control c in controls)
                AddControl(c);
        }
    }
}
