#region File Description
/*
 * Author: Hristo Hristov
 * Date: 10.02.12
 * Revision: 1 (10.02.12)
 *
 * **********************************
 * License: Microsoft Public License (Ms-PL)
 * -----------------------------------------
 * This license governs use of the accompanying software. If you use the software, you accept this license. If you do not accept the license, do not use the software.
 *
 * 1. Definitions
 *
 * The terms "reproduce," "reproduction," "derivative works," and "distribution" have the same meaning here as under U.S. copyright law.
 *
 * A "contribution" is the original software, or any additions or changes to the software.
 *
 * A "contributor" is any person that distributes its contribution under this license.
 *
 * "Licensed patents" are a contributor's patent claims that read directly on its contribution.
 *
 * 2. Grant of Rights
 *
 * (A) Copyright Grant- Subject to the terms of this license, including the license conditions and limitations in section 3, each contributor grants you a non-exclusive, worldwide, royalty-free copyright license to reproduce its contribution, prepare derivative works of its contribution, and distribute its contribution or any derivative works that you create.
 *
 * (B) Patent Grant- Subject to the terms of this license, including the license conditions and limitations in section 3, each contributor grants you a non-exclusive, worldwide, royalty-free license under its licensed patents to make, have made, use, sell, offer for sale, import, and/or otherwise dispose of its contribution in the software or derivative works of the contribution in the software.
 *
 * 3. Conditions and Limitations
 *
 * (A) No Trademark License- This license does not grant you rights to use any contributors' name, logo, or trademarks.
 *
 * (B) If you bring a patent claim against any contributor over patents that you claim are infringed by the software, your patent license from such contributor to the software ends automatically.
 *
 * (C) If you distribute any portion of the software, you must retain all copyright, patent, trademark, and attribution notices that are present in the software.
 * 
 * (D) If you distribute any portion of the software in source code form, you may do so only under this license by including a complete copy of this license with your distribution. If you distribute any portion of the software in compiled or object code form, you may only do so under a license that complies with this license.
 *
 * (E) The software is licensed "as-is." You bear the risk of using it. The contributors give no express warranties, guarantees or conditions. You may have additional consumer rights under your local laws which this license cannot change. To the extent permitted under your local laws, the contributors exclude the implied warranties of merchantability, fitness for a particular purpose and non-infringement. 
 */
#endregion

using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;
using XNAControl;

namespace XNAControlGame
{
    /// <summary>
    /// This is the main type for your game
    /// </summary>
    public class Game1 : XNAControl.XNAControlGame
    {
        SpriteBatch m_spriteBatch;
        SpriteFont m_spriteFont;
        Model m_modelTeapot;
        Matrix m_view;
        Matrix m_projection;
        TimeSpan m_duration;
        TimeSpan m_limit = TimeSpan.FromSeconds(5);
        double m_passedTime;
        int m_counter;
        int m_prevCounter;
               
        public bool Animation
        {
            get;
            set;
        }

        public Game1(IntPtr handle) : base(handle, "Content")
        {
            Animation = true;
        }

        /// <summary>
        /// Allows the game to perform any initialization it needs to before starting to run.
        /// This is where it can query for any required services and load any non-graphic
        /// related content.  Calling base.Initialize will enumerate through any components
        /// and initialize them as well.
        /// </summary>
        protected override void Initialize()
        {
            m_view = Matrix.CreateLookAt(new Vector3(0, 2, 8), new Vector3(0, 1, 0), Vector3.UnitY);
            m_projection = Matrix.CreatePerspectiveFieldOfView(MathHelper.ToRadians(45f), 4f / 3f, 0.1f, 100f);
                        
            base.Initialize();
        }

        /// <summary>
        /// LoadContent will be called once per game and is the place to load
        /// all of your content.
        /// </summary>
        protected override void LoadContent()
        {
            // Create a new SpriteBatch, which can be used to draw textures.
            m_spriteBatch = new SpriteBatch(GraphicsDevice);

            m_spriteFont = Content.Load<SpriteFont>("DebugFont");

            m_modelTeapot = Content.Load<Model>("teapot");
        }
        
        /// <summary>
        /// Allows the game to run logic such as updating the world,
        /// checking for collisions, gathering input, and playing audio.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Update(GameTime gameTime)
        {
            base.Update(gameTime);
        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);
            
            foreach (ModelMesh mesh in m_modelTeapot.Meshes)
            {
                // This is where the mesh orientation is set, as well 
                // as our camera and projection.
                foreach (BasicEffect effect in mesh.Effects)
                {
                    effect.EnableDefaultLighting();
                    effect.World = Animation ? CreateRotationMatrix(gameTime.ElapsedGameTime) : Matrix.Identity;
                    effect.View = m_view;
                    effect.Projection = m_projection;

                    effect.CurrentTechnique.Passes[0].Apply();
                }


                mesh.Draw();
            }

            DrawFPS(gameTime.ElapsedGameTime.TotalMilliseconds);

            base.Draw(gameTime);
        }

        Matrix CreateRotationMatrix(TimeSpan timeSpan)
        {
            m_duration += timeSpan;

            if (m_duration > m_limit)
                m_duration = TimeSpan.Zero;

            float amount = (float)(m_duration.TotalMilliseconds / m_limit.TotalMilliseconds);
            
            return Matrix.CreateRotationX(MathHelper.Lerp(0f, 2f * (float)Math.PI, amount)) *
                   Matrix.CreateRotationY(MathHelper.Lerp(0f, 2f * (float)Math.PI, amount)) *
                   Matrix.CreateRotationZ(MathHelper.Lerp(0f, 2f * (float)Math.PI, amount));
        }

        void DrawFPS(double elapsedTime)
        {
            m_passedTime += elapsedTime;
            if (m_passedTime >= 1000)
            {
                m_passedTime = 0;
                m_prevCounter = m_counter;
                m_counter = 1;
            }
            else if (elapsedTime != 0)
                m_counter++;

            m_spriteBatch.Begin();
            m_spriteBatch.DrawString(m_spriteFont, m_prevCounter.ToString() + " FPS", new Vector2(10,5), Color.White);
            m_spriteBatch.End();
        }
    }
}
