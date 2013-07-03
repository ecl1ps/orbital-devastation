using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using Microsoft.Xna.Framework;
using Lidgren.Network;
using Orbit.Core.Helpers;
using Orbit.Core.Scene.CollisionShapes;
using System.Globalization;
using Orbit.Gui.Visuals;

namespace Orbit.Core.Client.GameStates
{
    public class FloatingTextManager : IGameState
    {
        private SceneMgr mgr;
        private List<FloatingText> floatingTexts         = new List<FloatingText>();
        private List<FloatingText> newFloatingTexts      = new List<FloatingText>();

        public static bool CollisionShapeVisualization   = false;

        public const float TIME_LENGTH_1                 = 0.5f;
        public const float TIME_LENGTH_2                 = 1f;
        public const float TIME_LENGTH_3                 = 1.5f;
        public const float TIME_LENGTH_4                 = 2f;
        public const float TIME_LENGTH_5                 = 3f;
        public const float TIME_LENGTH_6                 = 6f;

        public const float SIZE_SMALL                    = 12;
        public const float SIZE_MEDIUM                   = 16;
        public const float SIZE_BIG                      = 22;
        public const float SIZE_BIGGER                   = 26;

        public FloatingTextManager(SceneMgr sceneMgr)
        {
            mgr = sceneMgr;
        }

        public void Update(float tpf)
        {
            for (int i = 0; i < floatingTexts.Count; ++i )
            {
                floatingTexts[i].RemainingTime -= tpf;
                if (floatingTexts[i].RemainingTime <= 0) // < 0%
                    RemoveFloatingText(floatingTexts[i]);
                else if (floatingTexts[i].RemainingTime <= 0.4 * floatingTexts[i].TotalTime) // 40% - 0%
                    FadeFloatingText(floatingTexts[i]);
                else if (floatingTexts[i].RemainingTime >= 0.7 * floatingTexts[i].TotalTime) // 100% - 70% 
                    GrowFloatingText(floatingTexts[i]);
            }

            for (int i = 0; i < newFloatingTexts.Count; ++i)
                CreateFloatingText(newFloatingTexts[i]);

            newFloatingTexts.Clear();
        }

        private void GrowFloatingText(FloatingText ft)
        {
            /*
             *TODO predelat render 
            double newScale = GetNewFontScale(ft.TotalTime - ft.RemainingTime, ft.TotalTime * 0.3f, ft.FontSize);
            mgr.BeginInvoke(new Action(() =>
            {
                (ft.GUIObject.Transform as TransformGroup).Children.Clear();
                (ft.GUIObject.Transform as TransformGroup).Children.Add(new ScaleTransform(newScale, newScale));
                (ft.GUIObject.Transform as TransformGroup).Children.Add(new TranslateTransform(ft.Position.X - ft.CS.Rectangle.Width * newScale / 2, ft.Position.Y - ft.CS.Rectangle.Height * newScale / 2));
            }));
             */
        }

        private double GetNewFontScale(float timeLeft, float timeTotal, float finalSize)
        {
            return FastMath.LinearInterpolate(10, finalSize, timeLeft / timeTotal) / finalSize;
        }

        private void FadeFloatingText(FloatingText ft)
        {
            float newOpacity = GetNewOpacity(ft.RemainingTime, ft.TotalTime * 0.4f);
        }

        private float GetNewOpacity(float remaining, float full)
        {
            return remaining / full;
        }

        private void CreateFloatingText(FloatingText ft)
        {
            /*
            DrawingGroup gr = null;
            mgr.Invoke(new Action(() =>
            {
                gr = new DrawingGroup();

                FormattedText finalTxt = new FormattedText(ft.Text, CultureInfo.CurrentCulture, FlowDirection.LeftToRight,
                    new Typeface("arial"), ft.FontSize, Brushes.Black);
                finalTxt.SetFontWeight(FontWeights.Bold);

                ft.CS.Rectangle = new Rectangle(finalTxt.Width, finalTxt.Height * 0.8); // nepotrebujeme cely radek i s volnym mistem nahore a dole

                gr.Children.Add(new GeometryDrawing(GetColorForType(ft.Type), null, finalTxt.BuildGeometry(new Vector2())));
                gr.Transform = new TransformGroup();
                double scale = 10 / ft.FontSize;
                (gr.Transform as TransformGroup).Children.Add(new ScaleTransform(scale, scale));
                (gr.Transform as TransformGroup).Children.Add(new TranslateTransform(ft.Position.X - ft.CS.Rectangle.Width * scale / 2, ft.Position.Y - ft.CS.Rectangle.Height * scale / 2));
                mgr.GetGameVisualArea().Add(gr, DrawingCategory.TEXTS);
            }));

            */
            ft.FinalPosition = new Vector2(ft.Position.X - ft.CS.Rectangle.Width / 2, ft.Position.Y - ft.CS.Rectangle.Height / 2);
            ft.CS.Position = ft.FinalPosition;
            ft.CollisionArea = new Rectangle((int)ft.CS.Position.X, (int)ft.CS.Position.Y, ft.CS.Rectangle.Height, ft.CS.Rectangle.Width);
            
            if (CollisionShapeVisualization)
                ShowCA(ft);

            AdjustPositionDueToCollisions(ft);

            floatingTexts.Add(ft);

            if (ft.Send)
            {
                NetOutgoingMessage msg = mgr.CreateNetMessage();
                msg.Write((int)PacketType.FLOATING_TEXT);
                msg.Write(ft.Text);
                msg.Write(ft.Position);
                msg.Write(ft.TotalTime);
                msg.Write((byte)ft.Type);
                msg.Write(ft.FontSize);
                mgr.SendMessage(msg);
            }
        }

        private void AdjustPositionDueToCollisions(FloatingText currentFt)
        {
            bool positionFound = false;
            while (!positionFound)
            {
                positionFound = true;

                FloatingText t;
                for (int i = 0; i < floatingTexts.Count; ++i)
                {
                    t = floatingTexts[i];
                    if (t == currentFt)
                        continue;

                    if (currentFt.CS.CollideWith(t.CS))
                    {
                        UpdateCollisionArea(currentFt, t.CS);
                        FindNewPos(currentFt);
                        positionFound = false;
                        break;
                    }
                }
            }

            currentFt.Position = currentFt.CS.Position + new Vector2(currentFt.CS.Rectangle.Width / 2, currentFt.CS.Rectangle.Height / 2);

            /*
            mgr.Invoke(new Action(() =>
            {
                double scale = 10 / currentFt.FontSize;
                (currentFt.GUIObject.Transform as TransformGroup).Children.Clear();
                (currentFt.GUIObject.Transform as TransformGroup).Children.Add(new ScaleTransform(scale, scale));
                (currentFt.GUIObject.Transform as TransformGroup).Children.Add(
                    new TranslateTransform(currentFt.Position.X - currentFt.CS.Rectangle.Width * scale / 2, currentFt.Position.Y - currentFt.CS.Rectangle.Height * scale / 2));
            }));
             */ 
        }

        /// <summary>
        /// prepocitava velikost kolizniho obdelniku floating textu tak, ze k nemu pricte poskytnuty kolizni obdelnik
        /// kolizni obdelnik udava oblast, ve ktere doslo ke kolizi a uz v ni neni opakovane zkouseno hledat volne misto
        /// </summary>
        /// <param name="squareCollisionShape"></param>
        private void UpdateCollisionArea(FloatingText ft, SquareCollisionShape cs)
        {
            // leva
            int x1 = ft.CollisionArea.Left;
            if (cs.Position.X < ft.CollisionArea.Left)
                x1 = (int) cs.Position.X - 1;

            // prava
            int x2 = ft.CollisionArea.Right;
            if (cs.Position.X + cs.Rectangle.Width > ft.CollisionArea.Right)
                x2 = (int) (cs.Position.X + cs.Rectangle.Width + 1);

            // horni
            int y1 = ft.CollisionArea.Top;
            if (cs.Position.Y < ft.CollisionArea.Top)
                y1 = (int) cs.Position.Y - 1;

            // dolni
            int y2 = ft.CollisionArea.Bottom;
            if (cs.Position.Y + cs.Rectangle.Height > ft.CollisionArea.Bottom)
                y2 = (int) (cs.Position.Y + cs.Rectangle.Height + 1);

            ft.CollisionArea = new Rectangle(x1, y1, x2 - x1, y2 - y1);
            
            if (CollisionShapeVisualization)
                ShowCA(ft);
        }

        private void ShowCA(FloatingText ft)
        {
            //TODO predelat rendering
        }

        /// <summary>
        /// pocita novou pozici pro floating text - jednou nad koliznim obdelnikem, jednou pod nim
        /// </summary>
        /// <param name="ft"></param>
        private void FindNewPos(FloatingText ft)
        {
            // TODO pripadne posunovat do dalsich stran

            if (!ft.LastCollisionMovedUp)
                // posunuti nahoru
                ft.CS.Position = new Vector2(ft.CS.Position.X, ft.CollisionArea.Top - ft.CS.Rectangle.Height - 2);
            else
                // posunuti dolu
                ft.CS.Position = new Vector2(ft.CS.Position.X, ft.CollisionArea.Bottom + 2);

            ft.LastCollisionMovedUp = !ft.LastCollisionMovedUp;
        }

        private void RemoveFloatingText(FloatingText ft)
        {
            //mgr.RemoveGraphicalObjectFromScene(ft.GUIObject, Gui.Visuals.DrawingCategory.TEXTS);
            floatingTexts.Remove(ft);
        }

        public void AddFloatingText(int value, Vector2 position, float time, FloatingTextType type, float fontSize = SIZE_SMALL, bool disableRandomPos = false, bool send = false)
        {
            AddFloatingText(value.ToString(Strings.Culture), position, time, type, fontSize, disableRandomPos, send);
        }

        public void AddFloatingText(string text, Vector2 position, float time, FloatingTextType type, float fontSize = SIZE_SMALL, bool disableRandomPos = false, bool send = false)
        {
            if (!disableRandomPos)
                position = new Vector2(position.X + mgr.GetRandomGenerator().Next(-5, 5), position.Y + mgr.GetRandomGenerator().Next(-5, 5));
            newFloatingTexts.Add(new FloatingText(text, position, time, type, fontSize, send));
        }

        private Color GetColorForType(FloatingTextType type)
        {
            switch (type)
            {
                case FloatingTextType.SCORE:
                    return Color.RoyalBlue;
                case FloatingTextType.BONUS_SCORE:
                    return Color.Goldenrod;
                case FloatingTextType.DAMAGE:
                    return Color.Red;
                case FloatingTextType.CHAT:
                    return Color.Blue;
                case FloatingTextType.HEAL:
                    return Color.Green;
                case FloatingTextType.SYSTEM:
                    return Color.DarkMagenta;
                case FloatingTextType.GOLD:
                    return Color.Goldenrod;
                default:
                    return Color.Black;
            }
        }

        private class FloatingText
        {
            public string Text { get; set; }
            public Vector2 Position { get; set; }
            public float TotalTime { get; set; }
            public float RemainingTime { get; set; }
            public FloatingTextType Type { get; set; }
            public float FontSize { get; set; }
            public bool Send { get; set; }
            public SquareCollisionShape CS { get; set; }
            public bool LastCollisionMovedUp { get; set; }
            public Vector2 FinalPosition { get; set; }
            public Rectangle CollisionArea { get; set; }

            public FloatingText(string text, Vector2 position, float time, FloatingTextType type, float fontSize, bool send)
            {
                Text = text;
                Position = position;
                TotalTime = time;
                RemainingTime = time;
                Type = type;
                FontSize = fontSize;
                Send = send;
                CS = new SquareCollisionShape();
                LastCollisionMovedUp = false;
                CollisionArea = new Rectangle();
            }
        }
    }

    public enum FloatingTextType 
    {
        DAMAGE,
        HEAL,
        SCORE,
        BONUS_SCORE,
        SYSTEM,
        CHAT,
        GOLD
    }
}
