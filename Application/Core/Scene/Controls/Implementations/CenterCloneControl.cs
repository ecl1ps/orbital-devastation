using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using Orbit.Core.Scene.Entities;

namespace Orbit.Core.Scene.Controls.Implementations
{
    public class CenterCloneControl : Control
    {
        public Vector Offset { get; set; }

        private ISceneObject toFollow;

        public CenterCloneControl(ISceneObject toFollow)
        {
            Offset = new Vector(0, 0);
            this.toFollow = toFollow;
        }

        protected override void InitControl(ISceneObject me)
        {
            me.Position = toFollow.Position;
        }

        protected override void UpdateControl(float tpf)
        {
            // stredy objektu jsou na sobe - pri dalsim posunuti se pouzije offset
            me.Position = toFollow.Center - (me.Center - me.Position) + Offset;
        }
    }
}
