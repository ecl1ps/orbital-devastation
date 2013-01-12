using System;
using Orbit.Core.Players;
using System.Windows.Media;
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

namespace Orbit.Core.Scene.Entities.Implementations
{
    public class Base : Square
    {
        public Player Owner { get; set; }
        public PlayerPosition BasePosition { get; set; }
        public Color Color { get; set; }
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

        private DrawingGroup image100;
        public DrawingGroup Image100 { get { return image100; } }
        private DrawingGroup image75;
        public DrawingGroup Image75 { get { return image75; } }
        private DrawingGroup image50;
        public DrawingGroup Image50 { get { return image50; } }
        private DrawingGroup image25;
        public DrawingGroup Image25 { get { return image25; } }
        private DrawingGroup background;
        public DrawingGroup BackgroundImage { get { return background; } }

        public Base(SceneMgr mgr, long id)
            : base(mgr, id)
        {
            Category = DrawingCategory.ASTEROIDS;
        }

        public void LoadImages()
        {
            image100 = SceneGeometryFactory.CreateBaseImage(this, "pack://application:,,,/resources/images/base/base_bw_shaded.png");
            image75 = SceneGeometryFactory.CreateBaseImage(this, "pack://application:,,,/resources/images/base/base_bw_shaded_75.png");
            image50 = SceneGeometryFactory.CreateBaseImage(this, "pack://application:,,,/resources/images/base/base_bw_shaded_50.png");
            image25 = SceneGeometryFactory.CreateBaseImage(this, "pack://application:,,,/resources/images/base/base_bw_shaded_25.png");
            background = SceneGeometryFactory.CreateBaseImage(this, "pack://application:,,,/resources/images/base/base_background_city.png", false);
        }

        public override bool IsOnScreen(Size screenSize)
        {
            return true;
        }

        public void OnIntegrityChange()
        {
            float integrityPct = Owner.GetBaseIntegrityPct();
            if (integrityPct <= 0.25)
                ChangeGeometry(Image25);
            else if (integrityPct <= 0.50)
                ChangeGeometry(Image50);
            else if (integrityPct <= 0.75)
                ChangeGeometry(Image75);
            else
                ChangeGeometry(Image100);
        }

        private void ChangeGeometry(DrawingGroup geometry)
        {
            SceneMgr.RemoveGraphicalObjectFromScene(GetGeometry(), Category);
            SceneMgr.AttachGraphicalObjectToScene(geometry, Category);
            SetGeometry(geometry);

            //VisualiseBase();
        }

        private void VisualiseBase()
        {
            Square sq = new SimpleSquare(SceneMgr, SceneMgr.GetCurrentPlayer().GetId());
            sq.Position = Position;
            sq.Size = Size;
            SceneMgr.AttachGraphicalObjectToScene(SceneGeometryFactory.CreateConstantColorRectangleGeometry(sq), DrawingCategory.PROJECTILE_BACKGROUND);

            Sphere s = new SimpleSphere(SceneMgr, SceneMgr.GetCurrentPlayer().GetId());
            s.Position = new Vector(Center.X, Position.Y + 2.5 * Size.Height);
            s.Radius = (int)(Size.Width / 1.6);
            SceneMgr.AttachGraphicalObjectToScene(SceneGeometryFactory.CreateConstantColorEllipseGeometry(s), DrawingCategory.PROJECTILE_BACKGROUND);
        }

        public override void  OnRemove()
        {
            base.OnRemove();
            SceneMgr.RemoveGraphicalObjectFromScene(GetGeometry(), Category);
            SceneMgr.RemoveGraphicalObjectFromScene(background, Category);
        }

        public override void OnAttach()
        {
            SceneMgr.AttachGraphicalObjectToScene(background, Category);
            //VisualiseBase();
        }
    }

}
