using System;
using Orbit.Core.Players;
using Microsoft.Xna.Framework;
using System.Windows;
using System.Windows.Threading;
using System.Windows.Controls;
using Lidgren.Network;
using Orbit.Core.Client;
using System.Windows.Media.Imaging;
using Orbit.Core.Helpers;
using Orbit.Core.Client.GameStates;
using Orbit.Core.Scene.CollisionShapes;
using Orbit.Gui.Visuals;
using Microsoft.Xna.Framework.Graphics;

namespace Orbit.Core.Scene.Entities.Implementations
{
    public class Base : TexturedSquare
    {
        public Player Owner { get; set; }
        public PlayerPosition BasePosition { get; set; }
        public int Integrity 
        { 
            get
            {
                return Owner.GetBaseIntegrity();
            }
            set
            {
                if (Owner.IsCurrentPlayerOrBot())
                {
                    Owner.SetBaseIntegrity(value);

                    OnIntegrityChange();
                    if (value < 0)
                        Integrity = 0;

                    NetOutgoingMessage msg = SceneMgr.CreateNetMessage();
                    msg.Write((int)PacketType.BASE_INTEGRITY_CHANGE);
                    msg.Write(Owner.GetId());
                    msg.Write(value);
                    SceneMgr.SendMessage(msg);
                }
            }
        }

        public Base(SceneMgr mgr, long id)
            : base(mgr, id)
        {
            Category = DrawingCategory.ASTEROIDS;
        }

        public override bool IsOnScreen(Rectangle screenSize)
        {
            return true;
        }

        public void OnIntegrityChange()
        {
            float integrityPct = Owner.GetBaseIntegrityPct();
            if (integrityPct <= 0.25)
                ChangeGeometry(SceneGeometryFactory.Base_25);
            else if (integrityPct <= 0.50)
                ChangeGeometry(SceneGeometryFactory.Base_50);
            else if (integrityPct <= 0.75)
                ChangeGeometry(SceneGeometryFactory.Base_75);
            else
                ChangeGeometry(SceneGeometryFactory.Base_100);
        }

        private void ChangeGeometry(Texture2D geometry)
        {
            Texture = geometry;
            //VisualiseBase();
        }

        public override void UpdateGeometric(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(SceneGeometryFactory.Base_background, Rectangle, Color.White);
            spriteBatch.Draw(Texture, Rectangle, Color);
        }
    }

}
