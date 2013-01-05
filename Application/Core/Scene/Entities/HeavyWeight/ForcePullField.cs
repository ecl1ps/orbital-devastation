using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Media;
using Orbit.Core.Client;
using System.Windows;
using Orbit.Core.Scene.Entities.HeavyWeight;
using System.Windows.Shapes;
using System.Windows.Controls;

namespace Orbit.Core.Scene.Entities.Implementations.HeavyWeight
{
    public class ForcePullField : HeavyWeightSceneObject, ISpheric
    {
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

        public ForcePullField(SceneMgr mgr, long id)
            : base(mgr, id)
        {
        }

        public override bool IsOnScreen(Size screenSize)
        {
            return true;
        }

        public override void UpdateGeometric()
        {
            Canvas.SetLeft(HeavyWeightGeometry, Position.X);
            Canvas.SetTop(HeavyWeightGeometry, Position.Y);
            (HeavyWeightGeometry as Image).Width = Radius * 2;
            (HeavyWeightGeometry as Image).Height = Radius * 2;
        }
    }
}
