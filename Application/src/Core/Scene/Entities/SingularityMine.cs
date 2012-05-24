using System;
using System.Windows;
using Orbit.Core.Scene.Controls;
using System.Windows.Threading;
using System.Windows.Media;
using System.Windows.Shapes;
using Lidgren.Network;
using System.Collections.Generic;
using Orbit.Core.Players;
using System.Windows.Controls;

namespace Orbit.Core.Scene.Entities
{

    public class SingularityMine : SceneObject, ICollidable, ISendable, IMovable
    {
        public float Radius { get; set; }
        public bool IsVisible { get; set; }
        public Player Owner { get; set; }
        public Vector Direction { get; set; }
        public Brush BorderBrush { get; set; } // neposilan
        public Brush FillBrush { get; set; } // neposilan

        public SingularityMine() : base()
        {
            IsVisible = false;
            BorderBrush = Brushes.Black;
            FillBrush = Brushes.Black;
        }

        public override bool IsOnScreen(Size screenSize)
        {
            return true;
        }

        public bool CollideWith(ICollidable other)
        {
            if (!IsVisible)
                return false;

            if (other is Meteor)
                return CollisionHelper.intersectsCircleAndCircle(Position, (int)Radius, (other as Meteor).Position, (other as Meteor).Radius);

            return false;
        }

        public void DoCollideWith(ICollidable other)
        {
            if (other is IMovable)
            {
                //SingularityControl c = GetControlOfType(typeof(SingularityControl)) as SingularityControl;
                DroppingSingularityControl c = GetControlOfType(typeof(DroppingSingularityControl)) as DroppingSingularityControl;
                if (c != null)
                    c.CollidedWith(other as IMovable);
            }
        }

        public override void UpdateGeometric()
        {
            if (!IsVisible)
                return;

            geometryElement.Dispatcher.Invoke(DispatcherPriority.DataBind, new Action(() =>
            {
                Canvas.SetLeft(geometryElement, Position.X);
                Canvas.SetTop(geometryElement, Position.Y);
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

            IList<IControl> controls = GetControlsCopy();
            msg.Write(controls.Count);
            foreach (IControl c in controls)
            {
                if (c is SingularityControl)
                {
                    msg.Write(typeof(SingularityControl).GUID.GetHashCode());
                    msg.WriteObjectSingularityControl(c as SingularityControl);
                }
                else if (c is DroppingSingularityControl)
                {
                    msg.Write(typeof(DroppingSingularityControl).GUID.GetHashCode());
                    msg.WriteObjectDroppingSingularityControl(c as DroppingSingularityControl);
                }
                else if (c is LinearMovementControl)
                {
                    msg.Write(typeof(LinearMovementControl).GUID.GetHashCode());
                    msg.WriteObjectLinearMovementControl(c as LinearMovementControl);
                }
                else
                    Console.Error.WriteLine("Sending Singularity Mine with unspported control!");
            }
        }

        public void ReadObject(NetIncomingMessage msg)
        {
            msg.ReadObjectSingularityMine(this);

            int controls = msg.ReadInt32();
            for (int i = 0; i < controls; ++i)
            {
                int hash = msg.ReadInt32();
                if (hash == typeof(SingularityControl).GUID.GetHashCode())
                {
                    SingularityControl c = new SingularityControl();
                    msg.ReadObjectSingularityControl(c);
                    AddControl(c);
                }
                else if (hash == typeof(DroppingSingularityControl).GUID.GetHashCode())
                {
                    DroppingSingularityControl c = new DroppingSingularityControl();
                    msg.ReadObjectDroppingSingularityControl(c);
                    AddControl(c);
                }
                else if (hash == typeof(LinearMovementControl).GUID.GetHashCode())
                {
                    LinearMovementControl c = new LinearMovementControl();
                    msg.ReadObjectLinearMovementControl(c);
                    AddControl(c);
                }
                else
                    Console.Error.WriteLine("Reading Singularity Mine with unspported control guid!");
            }
        }
    }
}
