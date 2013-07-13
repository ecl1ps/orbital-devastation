using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using Microsoft.Xna.Framework;
using System.Windows.Threading;
using System.Windows.Media;

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

        public void Add(Drawing elem, DrawingCategory cat = DrawingCategory.BACKGROUND)
        {
            (children[(int)cat] as ExtraDrawingVisual).DrawingChildren.Add(elem);
        }

        public void Remove(Drawing elem, DrawingCategory cat = DrawingCategory.BACKGROUND)
        {
            (children[(int)cat] as ExtraDrawingVisual).DrawingChildren.Remove(elem);
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
        private DrawingCollection dc = new DrawingCollection();
        public DrawingCollection DrawingChildren { get { return dc; } }
    }

    /// <summary>
    /// kategorie objektu, prakticky urcuje Z-index objektu a pomaha optimalizovat scenu;
    /// BACKGROUND je nejnize a TEXTS nejvyse, MAX nesmi byt pouzito
    /// </summary>
    public enum DrawingCategory
    {
        BACKGROUND              = 0,
        ASTEROIDS               = 1,
        LOOTABLES               = 2,
        PLAYER_OBJECTS          = 3,
        PROJECTILE_BACKGROUND   = 4,
        PROJECTILES             = 5,
        TEXTS                   = 6,
        GUI                     = 7,

        MAX                     = 8,
    }
}
