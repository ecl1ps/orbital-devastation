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

    public class SingularityMine : SceneObject, ICollidable, ISendable
    {
        public int Radius { get; set; }
        public bool IsVisible { get; set; }
        public Player Owner { get; set; }

        public SingularityMine() : base()
        {
            IsVisible = false;
        }

        public override bool IsOnScreen(Size screenSize)
        {
            return true;
        }

        public bool CollideWith(ICollidable other)
        {
            if (!IsVisible)
                return false;

            if (other is Sphere)
                return CollisionHelper.intersectsCircleAndCircle(Position, Radius, (other as Sphere).Position, (other as Sphere).Radius);

            return false;
        }

        public void DoCollideWith(ICollidable other)
        {
            if (other is IMovable)
            {
                SingularityControl c = GetControlOfType(typeof(SingularityControl)) as SingularityControl;
                if (c != null)
                    c.CollidedWith(other as IMovable);
            }
        }

        public override void UpdateGeometric()
        {
            if (!IsVisible)
                return;

            geometryElement.Dispatcher.BeginInvoke(DispatcherPriority.DataBind, new Action(() =>
            {
                ((geometryElement as Path).Data as EllipseGeometry).RadiusX = Radius;
                ((geometryElement as Path).Data as EllipseGeometry).RadiusY = Radius;
                (geometryElement as Path).Stroke = Brushes.Black;
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
                else
                    Console.Error.WriteLine("Reading Singularity Mine with unspported control guid!");
            }
        }
    }
}
