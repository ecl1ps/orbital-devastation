using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Orbit.Core.Client;
using System.Windows.Media;
using Orbit.Gui.Visuals;

namespace Orbit.Core.Scene.Entities.Implementations
{
    public class IceShard : IceSquare
    {
        public int TextureId { get; set; }

        public IceShard(SceneMgr mgr, long id)
            : base(mgr, id)
        {
            Category = DrawingCategory.LOOTABLES;
        }
    }
}
