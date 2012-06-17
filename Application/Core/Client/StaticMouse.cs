using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Forms;
using System.Runtime.InteropServices;

namespace Orbit.Core.Client
{
    public class StaticMouse : IAppState
    {
        private static StaticMouse instance;
        public static StaticMouse Instance { get { return instance; } }

        private FrameworkElement cursor;
        private bool enabled;
        private Canvas canvas;
        private SceneMgr sceneMgr;

        public bool Enabled
        {
            get { return enabled; }

            set
            {
                enableMouse(value);
            }
        }

        public float sensitivity { get; set; }

        public static void Init(FrameworkElement cursor, SceneMgr mgr)
        {
            instance = new StaticMouse(cursor, mgr);
        }

        private StaticMouse(FrameworkElement cursor, SceneMgr mgr)
        {
            if (cursor == null)
                throw new Exception("cursor cannot be null");

            this.cursor = cursor;
            sceneMgr = mgr;
            this.canvas = mgr.GetCanvas();
        }

        private void enableMouse(bool enable)
        {
            if (Enabled == enable)
                return;

            this.enabled = enable;
            if (enable)
            {
                attachCustomCursor();
            }
            else
            {
                dettachCustomCursor();
            }
        }

        public void positionCursor(System.Drawing.Point position) 
        {
            System.Windows.Forms.Cursor.Position = position;
        }

        public void attachCustomCursor()
        {
            sceneMgr.AttachGraphicalObjectToScene(cursor);
            sceneMgr.Invoke(new Action(() =>
            {
                Canvas.SetZIndex(cursor, 500);
            }));
            Cursor.Hide();
        }

        public void dettachCustomCursor()
        {
            sceneMgr.RemoveGraphicalObjectFromScene(cursor);
            Cursor.Show();
        }

        public void update(float tpf)
        {
            if (!enabled)
                return;

            sceneMgr.Invoke(new Action(() =>
            {
                Point p = sceneMgr.GetCanvas().PointFromScreen(new Point(Cursor.Position.X, Cursor.Position.Y));

                if (p.X > sceneMgr.ViewPortSizeOriginal.Width)
                    p.X = sceneMgr.ViewPortSizeOriginal.Width;
                else if (p.X < 0)
                    p.X = 0;

                if (p.Y > sceneMgr.ViewPortSizeOriginal.Height)
                    p.Y = sceneMgr.ViewPortSizeOriginal.Height;
                else if (p.Y < 0)
                    p.Y = 0;

                if (cursor.IsVisible)
                    updateCustomCursor(p);

                p = canvas.PointToScreen(p);
                Cursor.Position = new System.Drawing.Point((int)p.X, (int)p.Y);
            }));
        }

        private void updateCustomCursor(Point p)
        {
            Canvas.SetLeft(cursor, p.X - (cursor.ActualWidth / 2));
            Canvas.SetTop(cursor, p.Y - (cursor.ActualHeight / 2));
        }
    }
}
