using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Forms;
using System.Runtime.InteropServices;
using System.Windows.Media.Imaging;
using System.Windows.Media;

namespace Orbit.Core.Client.GameStates
{
    public class StaticMouse : IGameState
    {
        private static StaticMouse instance;
        public static StaticMouse Instance { get { return instance; } }
        public static bool ALLOWED = true;
        public static float SENSITIVITY = 1;

        private Point position;
        private Point center;
        private Point nativePosition;

        private FrameworkElement cursor;
        private bool enabled;
        private Canvas canvas;
        private SceneMgr sceneMgr;

        public static void Init(SceneMgr mgr)
        {
            if (instance == null)
                instance = new StaticMouse(mgr);
        }

        private StaticMouse(SceneMgr mgr)
        {
            sceneMgr = mgr;
            SENSITIVITY = float.Parse(GameProperties.Props.Get(PropertyKey.STATIC_MOUSE_SENSITIVITY));
            cursor = initCursorImage(new Uri(GameProperties.Props.Get(PropertyKey.STATIC_MOUSE_CURSOR)));
            this.canvas = mgr.GetCanvas();
        }

        private Image initCursorImage(Uri url)
        {
            Image img = null;
            sceneMgr.Invoke(new Action(() =>
            {
                BitmapImage image = new BitmapImage();
                image.BeginInit();
                image.UriSource = url;
                image.EndInit();

                img = new Image();
                img.RenderTransform = new ScaleTransform(0.5, 0.5);
                img.RenderTransformOrigin = new Point(0.5, 0.5);
                img.Source = image;
            }));

            return img;
        }

        public void InitCursorImage(Uri url)
        {
            cursor = initCursorImage(url);
        }

        public static void Enable(bool enable)
        {
            if (instance != null)
                instance.EnableMouse(enable);
        }

        public static Point GetPosition()
        {
            if (instance != null && instance.enabled)
                return instance.position;
            else if (instance != null && !instance.enabled)
                return instance.nativePosition;
            else
                return new Point(Cursor.Position.X, Cursor.Position.Y);
        }

        private void EnableMouse(bool enable)
        {
            if (!ALLOWED)
                return;

            if (sceneMgr == null || sceneMgr.GetCanvas() == null || !sceneMgr.GetCanvas().IsVisible)
            {
                enabled = false;
                return;
            }

            if (enabled == enable)
                return;

            enabled = enable;
            if (enable)
            {
                AttachCustomCursor();
                CenterNativeCursor();
            }
            else
            {
                DettachCustomCursor();
            }
        }

        private void AttachCustomCursor()
        {
            //sceneMgr.AttachGraphicalObjectToScene(cursor);
            sceneMgr.Invoke(new Action(() =>
            {
                Canvas.SetZIndex(cursor, 500);
                Cursor.Hide();
                Point p = sceneMgr.GetCanvas().PointFromScreen(new Point(Cursor.Position.X, Cursor.Position.Y));
                PositionCursor(new System.Drawing.Point((int)p.X, (int)p.Y));
            }));
            
        }

        private void PositionCursor(System.Drawing.Point point)
        {
            sceneMgr.Invoke(new Action(() =>
            {
                position = new Point(point.X, point.Y);
                Canvas.SetLeft(cursor, point.X);
                Canvas.SetTop(cursor, point.Y);
            }));
        }

        private void DettachCustomCursor()
        {
            sceneMgr.Invoke(new Action(() =>
            {
                //sceneMgr.RemoveGraphicalObjectFromScene(cursor);
                PositionNativeCursor();
                Cursor.Show();
            }));
        }

        private void PositionNativeCursor()
        {
            sceneMgr.Invoke(new Action(() =>
            {
                if (sceneMgr.GetCanvas().IsVisible)
                {
                    Point p = sceneMgr.GetCanvas().PointToScreen(position);
                    Cursor.Position = new System.Drawing.Point((int)p.X, (int)p.Y);
                }
            }));
        }

        public void Update(float tpf)
        {
            if (!enabled)
            {
                ComputeNativePosition();
                return;
            }

            if (!sceneMgr.GetCanvas().IsVisible)
                return;

            sceneMgr.Invoke(new Action(() =>
            {
                if (!enabled)
                    return;

                position.X += (Cursor.Position.X - center.X) * SENSITIVITY;
                position.Y += (Cursor.Position.Y - center.Y) * SENSITIVITY;
                CenterNativeCursor();

                if (position.X > SharedDef.CANVAS_SIZE.Width)
                    position.X = SharedDef.CANVAS_SIZE.Width;
                else if (position.X < 0)
                    position.X = 0;

                if (position.Y > SharedDef.CANVAS_SIZE.Height)
                    position.Y = SharedDef.CANVAS_SIZE.Height;
                else if (position.Y < 0)
                    position.Y = 0;

                if (cursor.IsVisible)
                    UpdateCustomCursor(position);
            }));
        }

        private void ComputeNativePosition()
        {
            sceneMgr.Invoke(new Action(() =>
            {
                if (sceneMgr.GetCanvas().IsVisible)
                    nativePosition = sceneMgr.GetCanvas().PointFromScreen(new Point(Cursor.Position.X, Cursor.Position.Y));
                else
                    nativePosition = new Point(Cursor.Position.X, Cursor.Position.Y);
            }));
        }

        private void CenterNativeCursor()
        {
            sceneMgr.Invoke(new Action(() =>
            {
                if (sceneMgr.GetCanvas().IsVisible)
                {
                    Point p = sceneMgr.GetCanvas().PointToScreen(new Point(SharedDef.CANVAS_SIZE.Width / 2, SharedDef.CANVAS_SIZE.Height / 2));
                    Cursor.Position = new System.Drawing.Point((int)p.X, (int)p.Y);
                    center = new Point(Cursor.Position.X, Cursor.Position.Y);
                    if (position == null)
                        position = center;
                }
            }));
        }

        private void UpdateCustomCursor(Point p)
        {
            Canvas.SetLeft(cursor, p.X - (cursor.ActualWidth / 2));
            Canvas.SetTop(cursor, p.Y - (cursor.ActualHeight / 2));
        }
    }
}
