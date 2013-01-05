using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Media;
using Orbit.Core.Client;
using System.Windows;
using System.Windows.Controls;
using Orbit.Core.Scene.Entities.HeavyWeight;

namespace Orbit.Core.Scene.Entities.Implementations.HeavyWeight
{
    public class SphereField : HeavyWeightSceneObject, ISpheric
    {
        public Color Color { get; set; }
        public int Radius { get; set; }
        public bool HasPositionInCenter { get; set; }
        public override Vector Center
        {
            get
            {
                if (!HasPositionInCenter) // image ma Position v levem hornim rohu
                    return new Vector(Position.X + Radius, Position.Y + Radius);
                else
                    return Position; // EllipseGeometry ma Position ve stredu
            }
        }

        public SphereField(SceneMgr mgr, long id)
            : base(mgr, id)
        {
        }

        public override bool IsOnScreen(System.Windows.Size screenSize)
        {
            return true;
        }

        public override void UpdateGeometric()
        {
            Canvas.SetLeft(Path, Position.X);
            Canvas.SetTop(Path, Position.Y);
        }
    }
}
