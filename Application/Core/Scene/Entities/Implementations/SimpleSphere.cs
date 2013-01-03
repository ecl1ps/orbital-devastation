using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Orbit.Core.Client;
using Orbit.Gui.Visuals;

namespace Orbit.Core.Scene.Entities.Implementations
{
    /// <summary>
    /// SimpleSphere nic neumi, je totiz trochu hloupa... takze by nebylo dobre, kdyby mela potomky - s genomem by to nedopadlo dobre
    /// </summary>
    public sealed class SimpleSphere : Sphere
    {
        public SimpleSphere(SceneMgr mgr, long id)
            : base(mgr, id)
        {
            Category = DrawingCategory.GUI;
        }
    }
}
