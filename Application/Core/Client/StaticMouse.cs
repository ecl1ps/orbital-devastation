﻿using System;
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

        public Point position;
        private Point center;

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
                centerNativeCursor();
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
                Cursor.Hide();
                //FIXME Point p = sceneMgr.GetCanvas().PointFromScreen(new Point(Cursor.Position.X, Cursor.Position.Y));
                //positionCursor(new System.Drawing.Point((int)p.X, (int)p.Y));
            }));
            
        }

        private void positionCursor(System.Drawing.Point point)
        {
            sceneMgr.Invoke(new Action(() =>
            {
                Canvas.SetLeft(cursor, point.X);
                Canvas.SetTop(cursor, point.Y);
            }));
        }

        public void DettachCustomCursor()
        {
            sceneMgr.Invoke(new Action(() =>
            {
                sceneMgr.RemoveGraphicalObjectFromScene(cursor);
                positionNativeCursor();
                Cursor.Show();
            }));
        }

        private void positionNativeCursor()
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
                centerNativeCursor();

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

        private void centerNativeCursor()
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