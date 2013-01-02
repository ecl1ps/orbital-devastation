using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Orbit.Core.Client;
using Orbit.Gui.Visuals;

namespace Orbit.Core.Scene.Entities.Implementations
{
    /// <summary>
    /// Se SimpleSquare to taky moc daleko nedotahnete... mozna znate jeho kamaradku, SimpleSphere... oni jsou... proste jednodusi
    /// </summary>
    public sealed class SimpleSquare : Square
    {
        public SimpleSquare(SceneMgr mgr, long id)
            : base(mgr, id)
        {
            Category = DrawingCategory.GUI;
        }
    }
}
