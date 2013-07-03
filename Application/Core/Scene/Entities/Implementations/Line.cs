using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using Orbit.Core.Client;
using Orbit.Core.Helpers;
using Orbit.Core.Players;
using Microsoft.Xna.Framework;
using C3.XNA;

namespace Orbit.Core.Scene.Entities
{
    public class Line : SceneObject
    {
        public Vector2 Start {get; set; }
        public Vector2 End { get; set; }
        public override Vector2 Center
        {
            get
            {
                return Start + ((Start - End) / 2);
            }
        }

        public Line(SceneMgr mgr, long id)
            : base(mgr, id)
        {
        }

        public Line(SceneMgr mgr, long id, Vector2 start, Vector2 end, Color color, int width)
            : base(mgr, id)
        {
            SceneMgr = mgr;
            Start = start;
            End = end;
        }

        public override bool IsOnScreen(Rectangle screenSize)
        {
            // always on screen
            return true;
        }

        public override void UpdateGeometric(Microsoft.Xna.Framework.Graphics.SpriteBatch spriteBatch)
        {
            base.UpdateGeometric(spriteBatch);
            spriteBatch.DrawLine(Start, End, Color);
        }

    }
}
