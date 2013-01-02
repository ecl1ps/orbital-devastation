using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Media;
using System.Windows.Threading;

namespace Orbit.Gui.Visuals
{
    public class GameVisualArea : FrameworkElement
    {
        private VisualCollection children;
        private VisualRenderer renderer;
        private Action invalidateAction;

        public GameVisualArea()
        {
            children = new VisualCollection(this);
            renderer = new VisualRenderer(this);
            invalidateAction = new Action(() =>
            {
                InvalidateVisual();
            });

            for (int i = 0; i < (int)DrawingCategory.MAX; ++i)
                children.Add(new ExtraDrawingVisual());
        }

        protected override void OnRender(DrawingContext dc)
        {
            renderer.Render(dc);
        }

        protected override int VisualChildrenCount
        {
            get { return children.Count; }
        }

        protected override Visual GetVisualChild(int index)
        {
            if (index < 0 || index >= children.Count)
            {
                throw new ArgumentOutOfRangeException();
            }

            return children[index];
        }

        public int GetChildrenCount()
        {
            return children.Count;
        }

        public Visual GetChild(int index)
        {
            if (index < 0 || index >= children.Count)
            {
                throw new ArgumentOutOfRangeException();
            }

            return children[index];
        }

        /// <summary>
        /// deprecated
        /// </summary>
        public void Add(Visual elem)
        {
            /*EllipseGeometry g = new EllipseGeometry(new Point((children[0] as ExtraDrawingVisual).DrawingChildren.Count * 5, (children[0] as ExtraDrawingVisual).DrawingChildren.Count * 5), 5, 5);
            Add(new GeometryDrawing(Brushes.Coral, new Pen(Brushes.CornflowerBlue, 1), g));*/
            //children.Add(elem);
        }

        /// <summary>
        /// deprecated
        /// </summary>
        public void Remove(Visual elem)
        {
            //children.Remove(elem);
        }

        public void Add(Drawing elem, DrawingCategory cat = DrawingCategory.BACKGROUND)
        {
            (children[(int)cat] as ExtraDrawingVisual).DrawingChildren.Add(elem);
        }

        public void Remove(Drawing elem, DrawingCategory cat = DrawingCategory.BACKGROUND)
        {
            (children[(int)cat] as ExtraDrawingVisual).DrawingChildren.Add(elem);
        }

        public void Clear()
        {
            children.Clear();
        }

        public void RunRender()
        {
            Dispatcher.Invoke(invalidateAction, DispatcherPriority.Send);
        }
    }

    public class ExtraDrawingVisual : DrawingVisual
    {
        public DrawingCollection DrawingChildren { get; set; }

        public ExtraDrawingVisual()
            : base()
        {
            DrawingChildren = new DrawingCollection();
        }
    }

    public enum DrawingCategory
    {
        BACKGROUND      = 0,
        ASTEROIDS       = 1,
        LOOTABLES       = 2,
        PLAYER_OBJECTS  = 3,
        PROJECTILES     = 4,
        TEXTS           = 5,

        MAX             = 6,
    }
}
