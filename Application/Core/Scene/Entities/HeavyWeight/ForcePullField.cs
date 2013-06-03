using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Media;
using Orbit.Core.Client;
using System.Windows;
using System.Windows.Shapes;
using System.Windows.Controls;

namespace Orbit.Core.Scene.Entities.Implementations.HeavyWeight
{
    public class ForcePullField : HeavyweightSphere
    {
        public ForcePullField(SceneMgr mgr, long id)
            : base(mgr, id)
        {
        }

        public override bool IsOnScreen(Size screenSize)
        {
            return true;
        }

        protected override void UpdateGeometricState()
        {
            if (Radius <= 0)
                return;

            (HeavyWeightGeometry as Image).Width = Radius * 2;
            (HeavyWeightGeometry as Image).Height = Radius * 2;
        }
    }
}
