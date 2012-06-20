using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Forms;
using System.Runtime.InteropServices;
using System.Windows.Media.Imaging;

namespace Orbit.Core.Client
{
    public class StaticMouse : IGameState
    {
        private static StaticMouse instance;
        public static StaticMouse Instance { get { return instance; } }

        private Point position;
        private Point center;

        private FrameworkElement cursor;
        private bool enabled;
        private Canvas canvas;
        private SceneMgr sceneMgr;

        public float Sensitivity { get; set; }

        public static void Init(SceneMgr mgr)
        {
            if(instance == null)
                instance = new StaticMouse(mgr);
        }

        private StaticMouse(SceneMgr mgr)
        {
            Image img = null;
            mgr.Invoke(new Action(() =>
            {
                BitmapImage image = new BitmapImage();
                image.BeginInit();
                image.UriSource = new Uri("pack://application:,,,/resources/images/mouse/targeting_icon.png");
                image.EndInit();

                img = new Image();
                img.Source = image;
            }));

            cursor = img;
            sceneMgr = mgr;
            this.canvas = mgr.GetCanvas();
        }

        public static void Enable(bool enable)
        {
            if (instance != null)
                instance.EnableMouse(enable);
        }

        public static Point GetPosition()
        {
            if (instance != null)
                return instance.position;
            return new Point();
        }

        private void EnableMouse(bool enable)
        {
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
            sceneMgr.AttachGraphicalObjectToScene(cursor);
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
                sceneMgr.RemoveGraphicalObjectFromScene(cursor);
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
                return;

            if (!sceneMgr.GetCanvas().IsVisible)
                return;

            sceneMgr.Invoke(new Action(() =>
            {
                if (!enabled)
                    return;

                position.X += Cursor.Position.X - center.X;
                position.Y += Cursor.Position.Y - center.Y;
                CenterNativeCursor();

                if (position.X > sceneMgr.ViewPortSizeOriginal.Width)
                    position.X = sceneMgr.ViewPortSizeOriginal.Width;
                else if (position.X < 0)
                    position.X = 0;

                if (position.Y > sceneMgr.ViewPortSizeOriginal.Height)
                    position.Y = sceneMgr.ViewPortSizeOriginal.Height;
                else if (position.Y < 0)
                    position.Y = 0;

                if (cursor.IsVisible)
                    UpdateCustomCursor(position);
            }));
        }

        private void CenterNativeCursor()
        {
            sceneMgr.Invoke(new Action(() =>
            {
                if (sceneMgr.GetCanvas().IsVisible)
                {
                    Point p = sceneMgr.GetCanvas().PointToScreen(new Point(sceneMgr.ViewPortSizeOriginal.Width / 2, sceneMgr.ViewPortSizeOriginal.Height / 2));
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
