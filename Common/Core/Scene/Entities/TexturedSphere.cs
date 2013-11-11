using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Orbit.Core.Client;
using Orbit.Core.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Orbit.Core.Scene.Entities
{
    public class TexturedSphere : TexturedSceneObject, ISpheric
    {
        public int Radius { get; set; }

        public TexturedSphere(SceneMgr mgr, long id)
            : base(mgr, id)
        {
        }

        public override void Update(float tpf)
        {
            base.Update(tpf);
            scale = Radius / (float) Texture.Width;
        }

        public override bool IsOnScreen(Microsoft.Xna.Framework.Rectangle rectangle)
        {
            // objekt se odstrani az je jakoby dvakrat mimo obrazovku (dvakrat jeho sirka)
            if (Position.X <= (-Radius * 4) || Position.Y <= (-Radius * 4))
                return false;

            if (Position.X >= rectangle.Width + Radius * 2 || Position.Y >= rectangle.Height + Radius * 2)
                return false;

            return true;
        }
    }
}
