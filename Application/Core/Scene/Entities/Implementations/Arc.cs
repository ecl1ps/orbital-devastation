using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Orbit.Core.Client;
using Orbit.Core.Helpers;

namespace Orbit.Core.Scene.Entities.Implementations
{
    public class Arc : SceneObject
    {
        public float Radius { get; set; }
        public float StartAngle { get; set; }
        public float EndingAngle { get; set; }

        public Arc(SceneMgr mgr, long id)
            : base(mgr, id)
        {
        }

        public override bool IsOnScreen(Microsoft.Xna.Framework.Rectangle screenSize)
        {
            return true;
        }

        public override void UpdateGeometric(Microsoft.Xna.Framework.Graphics.SpriteBatch spriteBatch)
        {
            base.UpdateGeometric(spriteBatch);

            spriteBatch.DrawArc(Position, Radius, 24, StartAngle, EndingAngle, Color);
        }
    }
}
