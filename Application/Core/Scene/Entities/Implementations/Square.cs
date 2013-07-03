using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using Microsoft.Xna.Framework;
using System.Windows.Threading;
using System.Windows.Controls;
using Orbit.Core.Client;
using Orbit.Core.Helpers;
using Orbit.Core.Scene.Entities.Implementations;
using C3.XNA;

namespace Orbit.Core.Scene.Entities
{
    public abstract class Square : SceneObject
    {
        public Rectangle Rectangle { get; set; }
        public override Vector2 Center { get { return new Vector2(Position.X + Rectangle.Width / 2, Position.Y + Rectangle.Height / 2); } }

        public Square(SceneMgr mgr, long id)
            : base(mgr, id)
        {
        }

        public override bool IsOnScreen(Rectangle screenSize)
        {
            // objekt se odstrani az je jakoby dvakrat mimo obrazovku (dvakrat jeho sirka resp vyska)
            if (Position.X <= (- Rectangle.Width * 2) || Position.Y <= (- Rectangle.Height * 2))
                return false;

            if (Position.X >= screenSize.Width + Rectangle.Width || Position.Y >= screenSize.Height + Rectangle.Height)
                return false;

            return true;
        }

        public override void Update(float tpf)
        {
            base.Update(tpf);
            Rectangle.Offset((int) Position.X, (int) Position.Y);
        }

        public override void UpdateGeometric(Microsoft.Xna.Framework.Graphics.SpriteBatch spriteBatch)
        {
            base.UpdateGeometric(spriteBatch);

            spriteBatch.DrawRectangle(Rectangle, Color);
        }
    }
}
