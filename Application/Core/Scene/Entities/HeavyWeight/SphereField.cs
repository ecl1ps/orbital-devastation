using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Media;
using Orbit.Core.Client;
using System.Windows;
using System.Windows.Controls;

namespace Orbit.Core.Scene.Entities.Implementations.HeavyWeight
{
    public class SphereField : HeavyweightSphere
    {
        public SphereField(SceneMgr mgr, long id)
            : base(mgr, id)
        {
        }

        public override bool IsOnScreen(Size screenSize)
        {
            return true;
        }
    }
}
