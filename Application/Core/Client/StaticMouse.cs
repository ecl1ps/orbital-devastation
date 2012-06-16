using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Forms;

namespace Orbit.Core.Client
{
    public class StaticMouse : IAppState
    {
        private UIElement cursor;
        private bool enabled;
        private Canvas canvas;
        private SceneMgr sceneMgr;
        private System.Drawing.Point center;
        private System.Drawing.Point position;

        public System.Drawing.Point Position { get { return position; } set { position = value; } }
        public bool Enabled
        {
            get { return enabled; }

            set
            {
                enableMouse(value);
            }
        }

        public float sensitivity { get; set; }

        public StaticMouse(UIElement cursor, SceneMgr mgr)
        {
            if (cursor == null)
                throw new Exception("cursor cannot be null");

            this.cursor = cursor;
            sceneMgr = mgr;
            this.canvas = mgr.GetCanvas();
        }

        private void enableMouse(bool enable)
        {
            this.enabled = enable;
            if (enable)
            {
                centerNativeCursor();
                attachCustomCursor();
            }
            else
            {
                positionCursor(Position);
                dettachCustomCursor();
            }
        }

        private void centerNativeCursor()
        {
            System.Windows.Forms.Cursor.Position = new System.Drawing.Point((int) sceneMgr.ViewPortSizeOriginal.Width / 2, (int) sceneMgr.ViewPortSizeOriginal.Height / 2);
            center = System.Windows.Forms.Cursor.Position;
        }
        public void positionCursor(System.Drawing.Point position) 
        {
            System.Windows.Forms.Cursor.Position = position;
        }

        public void attachCustomCursor()
        {
            sceneMgr.AttachGraphicalObjectToScene(cursor);
        }

        public void dettachCustomCursor()
        {
            sceneMgr.RemoveGraphicalObjectFromScene(cursor);
        }

        public void update(float tpf)
        {
            if (!enabled)
                return;

            position.X += center.X - Cursor.Position.X;
            position.Y += center.Y - Cursor.Position.Y;
            centerNativeCursor();
            if(cursor.IsVisible)
                updateCustomCursor();
        }

        private void updateCustomCursor()
        {
            sceneMgr.Invoke(new Action(() =>
            {
                Canvas.SetLeft(cursor, Position.X);
                Canvas.SetTop(cursor, Position.Y);
            }));
        }
    }
}
