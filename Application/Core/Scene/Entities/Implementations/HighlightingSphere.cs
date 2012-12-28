using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Orbit.Core.Client;

namespace Orbit.Core.Scene.Entities.Implementations
{
    public class HighlightingSphere : Sphere
    {
        public HighlightingSphere(SceneMgr mgr, long id)
            : base(mgr, id)
        {
        }
    }
}
