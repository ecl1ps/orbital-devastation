using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using Lidgren.Network;
using Orbit.Core.Helpers;

namespace Orbit.Core.Client
{
    public class FloatingTextManager : IGameState
    {
        private SceneMgr mgr;
        private List<FloatingText> floatingTexts = new List<FloatingText>();
        private List<FloatingText> newFloatingTexts = new List<FloatingText>();

        public const float TIME_LENGTH_1                 = 0.5f;
        public const float TIME_LENGTH_2                 = 1f;
        public const float TIME_LENGTH_3                 = 1.5f;
        public const float TIME_LENGTH_4                 = 2f;
        public const float TIME_LENGTH_5                 = 3f;

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

            foreach (FloatingText ft in newFloatingTexts)
                CreateFloatingText(ft);

            newFloatingTexts.Clear();
        }

        private void GrowFloatingText(FloatingText ft)
        {
            float newSize = GetNewFontSize(ft.TotalTime - ft.RemainingTime, ft.TotalTime * 0.3f, ft.FontSize);
            mgr.BeginInvoke(new Action(() =>
            {
                ft.GUIObject.FontSize = newSize;
                Canvas.SetLeft(ft.GUIObject, ft.Position.X - ft.GUIObject.ActualWidth / 2);
                Canvas.SetTop(ft.GUIObject, ft.Position.Y - ft.GUIObject.ActualHeight / 2);
            }));
        }

        private float GetNewFontSize(float timeLeft, float timeTotal, float finalSize)
        {
            return FastMath.LinearInterpolate(5, finalSize, timeLeft / timeTotal);
        }

        private void FadeFloatingText(FloatingText ft)
        {
            float newOpacity = GetNewOpacity(ft.RemainingTime, ft.TotalTime * 0.4f);
            mgr.BeginInvoke(new Action(() =>
            {
                ft.GUIObject.Opacity = newOpacity;
            }));
        }

        private float GetNewOpacity(float remaining, float full)
        {
            return remaining / full;
        }

        private void CreateFloatingText(FloatingText ft)
        {
            TextBlock tb = null;
            mgr.Invoke(new Action(() =>
            {
                tb = new TextBlock();
                tb.Text = ft.Text;
                tb.Foreground = GetColorForType(ft.Type);
                tb.FontWeight = FontWeights.Bold;
                tb.FontSize = 5;
                Canvas.SetLeft(tb, ft.Position.X);
                Canvas.SetTop(tb, ft.Position.Y);
                mgr.GetCanvas().Children.Add(tb);
            }));
            ft.GUIObject = tb;

            floatingTexts.Add(ft);
        }

        private void RemoveFloatingText(FloatingText ft)
        {
            mgr.BeginInvoke(new Action(() =>
            {
                mgr.GetCanvas().Children.Remove(ft.GUIObject);
            }));
            floatingTexts.Remove(ft);
        }

        public void AddFloatingText(int value, Vector position, float time, FloatingTextType type, float fontSize = SIZE_SMALL, bool disableRandomPos = false, bool send = false)
        {
            AddFloatingText(value.ToString(), position, time, type, fontSize, disableRandomPos, send);
        }

        public void AddFloatingText(string text, Vector position, float time, FloatingTextType type, float fontSize = SIZE_SMALL, bool disableRandomPos = false, bool send = false)
        {
            if (!disableRandomPos)
                position = new Vector(position.X + mgr.GetRandomGenerator().Next(-5, 5), position.Y + mgr.GetRandomGenerator().Next(-5, 5));
            newFloatingTexts.Add(new FloatingText(text, position, time, type, fontSize));

            if (send)
            {
                NetOutgoingMessage msg = mgr.CreateNetMessage();
                msg.Write((int)PacketType.FLOATING_TEXT);
                msg.Write(text);
                msg.Write(position);
                msg.Write(time);
                msg.Write((byte)type);
                msg.Write(fontSize);
                mgr.SendMessage(msg);
            }
        }

        private Brush GetColorForType(FloatingTextType type)
        {
            switch (type)
            {
                case FloatingTextType.SCORE:
                    return Brushes.Goldenrod;
                case FloatingTextType.DAMAGE:
                    return Brushes.Red;
                case FloatingTextType.CHAT:
                    return Brushes.Blue;
                case FloatingTextType.HEAL:
                    return Brushes.Green;
                case FloatingTextType.SYSTEM:
                    return Brushes.DarkMagenta;
                default:
                    return Brushes.Black;
            }
        }

        private class FloatingText
        {
            public string Text { get; set; }
            public Vector Position { get; set; }
            public float TotalTime { get; set; }
            public float RemainingTime { get; set; }
            public FloatingTextType Type { get; set; }
            public float FontSize { get; set; }
            public TextBlock GUIObject { get; set; }

            public FloatingText(string text, Vector position, float time, FloatingTextType type, float fontSize)
            {
                Text = text;
                Position = position;
                TotalTime = time;
                RemainingTime = time;
                Type = type;
                FontSize = fontSize;
            }
        }
    }

    public enum FloatingTextType 
    {
        DAMAGE,
        HEAL,
        SCORE,
        SYSTEM,
        CHAT
    }
}
