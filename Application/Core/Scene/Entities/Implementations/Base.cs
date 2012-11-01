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

        private UIElement image100;
        public UIElement Image100 { get { return image100; } }
        private UIElement image75; 
        public UIElement Image75 { get { return image75; } }
        private UIElement image50;
        public UIElement Image50 { get { return image50; } }
        private UIElement image25;
        public UIElement Image25 { get { return image25; } }
        private UIElement background;
        public UIElement BackgroundImage { get { return background; } }
        
        public Base(SceneMgr mgr) : base(mgr)
        {
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

        public override void DoCollideWith(ICollidable other, float tpf)
        {
            if (other is Asteroid && (other as Asteroid).Enabled)
            {
                int damage = (other as Asteroid).Radius / 2;

                // score
                Player otherPlayer = SceneMgr.GetOtherActivePlayer(Owner.GetId());
                if (otherPlayer.IsCurrentPlayer())
                {
                    Vector textPos = new Vector(otherPlayer.GetBaseLocation().X + (otherPlayer.GetBaseLocation().Width / 2), otherPlayer.GetBaseLocation().Y - 20);
                    SceneMgr.FloatingTextMgr.AddFloatingText(damage * ScoreDefines.DAMAGE_DEALT, textPos, FloatingTextManager.TIME_LENGTH_2,
                        FloatingTextType.SCORE, FloatingTextManager.SIZE_MEDIUM);
                }

                if (otherPlayer.IsCurrentPlayerOrBot())
                    otherPlayer.AddScoreAndShow(damage * ScoreDefines.DAMAGE_DEALT);

                // damage
                SceneMgr.FloatingTextMgr.AddFloatingText(damage, new Vector((other as Asteroid).Center.X, (other as Asteroid).Center.Y - 10), 
                    FloatingTextManager.TIME_LENGTH_1, FloatingTextType.DAMAGE, FloatingTextManager.SIZE_MEDIUM);

                SoundManager.Instance.StartPlayingOnce(SharedDef.MUSIC_DAMAGE_TO_BASE);

                Integrity -= damage;
                (other as Asteroid).DoRemoveMe();
            }
        }

        public void OnIntegrityChange()
        {
            if (Integrity <= 25)
                ChangeGeometry(Image25);
            else if (Integrity <= 50)
                ChangeGeometry(Image50);
            else if (Integrity <= 75)
                ChangeGeometry(Image75);
            else
                ChangeGeometry(Image100);
        }

        private void ChangeGeometry(UIElement geometry)
        {
            SceneMgr.RemoveGraphicalObjectFromScene(GetGeometry());
            SceneMgr.AttachGraphicalObjectToScene(geometry);
            SetGeometry(geometry);
        }

        public override void  OnRemove()
        {
            base.OnRemove();
            SceneMgr.RemoveGraphicalObjectFromScene(background);
        }

        public override void OnAttach()
        {
            SceneMgr.AttachGraphicalObjectToScene(background);
        }
    }

}
