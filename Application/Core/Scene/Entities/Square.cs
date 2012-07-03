using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Media;
using System.Windows.Threading;
using System.Windows.Controls;
using Orbit.Core.Client;
using Orbit.Core.Helpers;
using Orbit.Core.Scene.Entities.Implementations;

namespace Orbit.Core.Scene.Entities
{
    public abstract class Square : SceneObject, ICollidable
    {
        public Size Size { get; set; }
        public Vector Center
        {
            get
            {
                return new Vector(Position.X + Size.Width / 2, Position.Y + Size.Height / 2);
            }
        }
        
        public Square(SceneMgr mgr) : base(mgr)
        {
        }

        public bool CollideWith(ICollidable other)
        {
            if (other is SpherePoint)
                return CollisionHelper.IntersectsPointAndSquare(((SpherePoint)other).Center, Position, Size);

            if (other is Sphere)
                return CollisionHelper.IntersectsCircleAndSquare((other as Sphere).Center, (other as Sphere).Radius, Position, Size);

            if (other is Square)
                return CollisionHelper.IntersectsSquareAndSquare(Position, Size, (other as Square).Position, (other as Square).Size);

            if (other is SolidLine)
                return CollisionHelper.IntersectLineAndSquare((other as SolidLine).Start, (other as SolidLine).End, Position.ToPoint(), Size);

            return false;
        }

        public abstract void DoCollideWith(ICollidable other, float tpf);

        public override bool IsOnScreen(Size screenSize)
        {
            if (Position.X + 2 * Size.Width + 5 < 0 || Position.Y + Size.Height + 5 < 0)
                return false;

            if ((Position.X - Size.Width - 5 > screenSize.Width) || (Position.Y + 5 > screenSize.Height))
                return false;

            return true;
        }

    }
}
