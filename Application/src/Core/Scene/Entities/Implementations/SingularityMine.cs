using System;
using System.Windows;
using Orbit.Core.Scene.Controls;
using System.Windows.Threading;
using System.Windows.Media;
using System.Windows.Shapes;
using Lidgren.Network;
using System.Collections.Generic;
using Orbit.Core.Players;

namespace Orbit.Core.Scene.Entities
{

    public class SingularityMine : Sphere, ISendable
    {
        public bool IsVisible { get; set; } // neposilan - pouzivano pouze controlem SingularityControl
        public Player Owner { get; set; } // neposilan
        public Brush BorderBrush { get; set; } // neposilan
        public Brush FillBrush { get; set; } // neposilan
        
        public SingularityMine(ISceneMgr mgr) : base(mgr)
        {
            BorderBrush = Brushes.Black;
            FillBrush = Brushes.Black;
        }

        public override void DoCollideWith(ICollidable other)
        {
            if (other is IMovable)
            {
                DroppingSingularityControl c = GetControlOfType(typeof(DroppingSingularityControl)) as DroppingSingularityControl;
                if (c != null)
                    c.CollidedWith(other as IMovable);
            }
        }

        protected override void UpdateGeometricState()
        {
            geometryElement.Dispatcher.Invoke(DispatcherPriority.DataBind, new Action(() =>
            {
                ((geometryElement as Path).Data as EllipseGeometry).RadiusX = Radius;
                ((geometryElement as Path).Data as EllipseGeometry).RadiusY = Radius;
                (geometryElement as Path).Stroke = BorderBrush;
                (geometryElement as Path).Fill = FillBrush;
            }));
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
            IList<IControl> controls = msg.ReadControls();
            foreach (Control c in controls)
                AddControl(c);
        }
    }
}
