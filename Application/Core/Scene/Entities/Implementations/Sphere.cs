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
using Microsoft.Xna.Framework.Graphics;
using Orbit.Core.Helpers;

namespace Orbit.Core.Scene.Entities
{
    public class Sphere : SceneObject, ISpheric
    {
        public int Radius { get; set; }

        public Sphere(SceneMgr mgr, long id)
            : base(mgr, id)
        {
        }

        public override bool IsOnScreen(Rectangle screenSize)
        {
            // objekt se odstrani az je jakoby dvakrat mimo obrazovku (dvakrat jeho sirka)
            if (Position.X <= (-Radius * 4) || Position.Y <= (-Radius * 4))
                return false;

            if (Position.X >= screenSize.Width + Radius * 2 || Position.Y >= screenSize.Height + Radius * 2)
                return false;

            return true;
        }

        public override void UpdateGeometric(SpriteBatch spriteBatch)
        {
            base.UpdateGeometric(spriteBatch);

            spriteBatch.DrawCircle(Position, Radius, 24, Color);
        }
    }
}
