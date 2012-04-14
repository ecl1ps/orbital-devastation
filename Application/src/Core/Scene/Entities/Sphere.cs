using System;
using System.Windows.Media;
using System.Windows;
using System.Windows.Threading;
using System.Windows.Controls;
using System.Windows.Shapes;
using Lidgren.Network;
using Orbit.Core.Scene.Controls;

namespace Orbit.Core.Scene.Entities
{

    public class Sphere : SceneObject, IMovable, ICollidable, IRotable
    {
        public Color Color { get; set; }
        public Vector Direction { get; set; }
        public int Radius { get; set; }
        public bool IsHeadingRight { get; set; }
        public float Rotation { get; set; }
        public int TextureId { get; set; }

        public bool CollideWith(ICollidable other)
        {
            if (other is Sphere)
                return CollisionHelper.intersectsCircleAndCircle(Position, Radius, (other as Sphere).Position, (other as Sphere).Radius);

            if (other is Base)
                return CollisionHelper.intersectsCircleAndSquare(Position, Radius, (other as Base).Position, (other as Base).Size);

            return false;
        }

        public void DoCollideWith(ICollidable other)
        {
            if (!(other is Sphere))
                DoRemoveMe();
        }

        public override void UpdateGeometric()
        {
            geometryElement.Dispatcher.BeginInvoke(DispatcherPriority.DataBind, (Action)(delegate 
            {
                Canvas.SetLeft(geometryElement, Position.X - Radius);
                Canvas.SetTop(geometryElement, Position.Y - Radius);
                (geometryElement as Image).Width = Radius * 2;
                (geometryElement as Image).RenderTransform = new RotateTransform(Rotation);
            }));

        }

        public override bool IsOnScreen(Size screenSize)
        {
            if (Position.X <= -Radius || Position.Y <= -Radius)
                return false;

            if (Position.X >= screenSize.Width + Radius || Position.Y >= screenSize.Height + Radius)
                return false;

            return true;
        }

        public override void OnRemove()
        {
            if (SceneMgr.GetInstance().GameType == Gametype.SOLO_GAME)
            {
                SceneMgr.GetInstance().AttachToScene(SceneObjectFactory.CreateNewSphereOnEdge(this));
                return;
            }

            if (SceneMgr.GetInstance().IsServer())
            {
                Sphere s = SceneObjectFactory.CreateNewSphereOnEdge(this);
                SceneMgr.GetInstance().AttachToScene(s);
                NetOutgoingMessage msg = SceneMgr.CreatNetMessage();
                msg.Write((int)PacketType.NEW_ASTEROID);
                msg.WriteObjectSphere(s);

                msg.Write((s.GetControlOfType(typeof(LinearMovementControl)) as LinearMovementControl).Speed);
                msg.Write((s.GetControlOfType(typeof(LinearRotationControl)) as LinearRotationControl).RotationSpeed);

                SceneMgr.SendMessage(msg);
            }
        }
    }

}
