using Microsoft.Xna.Framework;
using Orbit.Core.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Orbit.Core.Scene.Entities
{
    public class TexturedSquare : TexturedSceneObject
    {

        public TexturedSquare(SceneMgr mgr, long id)
            : base(mgr, id)
        {

        }

        public override bool IsOnScreen(Rectangle screenSize)
        {
            // objekt se odstrani az je jakoby dvakrat mimo obrazovku (dvakrat jeho sirka resp vyska)
            if (Position.X <= (-Rectangle.Width * 2) || Position.Y <= (-Rectangle.Height * 2))
                return false;

            if (Position.X >= screenSize.Width + Rectangle.Width || Position.Y >= screenSize.Height + Rectangle.Height)
                return false;

            return true;
        }

        public override void Update(float tpf)
        {
            base.Update(tpf);
            Rectangle.Offset((int)Position.X, (int)Position.Y);
        }
    }
}
