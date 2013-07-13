using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Orbit.Core.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Orbit.Core.Scene.Entities
{
    public abstract class TexturedSceneObject : SceneObject
    {
        public Rectangle Rectangle { get; set; }
        public Texture2D Texture { get; set; }

        public TexturedSceneObject(SceneMgr mgr, long id)
            : base(mgr, id)
        {
            Color = Color.White;
        }

        public override void Update(float tpf)
        {
            base.Update(tpf);
        }

        public override void UpdateGeometric(SpriteBatch spriteBatch)
        {
            base.UpdateGeometric(spriteBatch);
            spriteBatch.Draw(Texture, Rectangle, null, new Color(Color.R, Color.G, Color.B, Opacity) , Rotation, new Vector2(0, 0), SpriteEffects.None, 0);
        }
    }
}
