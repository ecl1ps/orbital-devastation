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
    public class StaticMouse : IGameState
    {
        private static StaticMouse instance;
        public static StaticMouse Instance { get { return instance; } }

        private FrameworkElement cursor;
        private bool enabled;
        private Canvas canvas;
        private SceneMgr sceneMgr;

        public bool Enabled
        {
            get 
            { 
                return enabled; 
            }
            set
            {
                EnableMouse(value);
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

        private void EnableMouse(bool enable)
        {
            if (Enabled == enable)
                return;

            this.enabled = enable;
            if (enable)
            {
                AttachCustomCursor();
            }
            else
            {
                DettachCustomCursor();
            }
        }

        public void PositionCursor(System.Drawing.Point position) 
        {
            System.Windows.Forms.Cursor.Position = position;
        }

        public void AttachCustomCursor()
        {
            sceneMgr.AttachGraphicalObjectToScene(cursor);
            sceneMgr.Invoke(new Action(() =>
            {
                Canvas.SetZIndex(cursor, 500);
            }));
            Cursor.Hide();
        }

        public void DettachCustomCursor()
        {
            sceneMgr.RemoveGraphicalObjectFromScene(cursor);
            Cursor.Show();
        }

        public void Update(float tpf)
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
                    UpdateCustomCursor(p);

                p = canvas.PointToScreen(p);
                Cursor.Position = new System.Drawing.Point((int)p.X, (int)p.Y);
            }));
        }

        private void UpdateCustomCursor(Point p)
        {
            Canvas.SetLeft(cursor, p.X - (cursor.ActualWidth / 2));
            Canvas.SetTop(cursor, p.Y - (cursor.ActualHeight / 2));
        }
    }
}
