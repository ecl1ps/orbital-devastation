using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Media;
using System.Windows;
using MB.Tools;

namespace Orbit.Gui.Visuals
{
    public class VisualRenderer
    {
        private static readonly log4net.ILog Logger = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        private GameVisualArea area;

        public VisualRenderer(GameVisualArea area)
        {
            this.area = area;
        }

        public void Render(DrawingContext dc)
        {
            for (int i = 0; i < area.GetChildrenCount(); ++i)
            {
                DrawAllDrawings(area.GetChild(i) as ExtraDrawingVisual, dc);
            }
        }

        private void DrawAllDrawings(ExtraDrawingVisual curr, DrawingContext dc)
        {
            foreach (Drawing drawing in curr.DrawingChildren)
                if (!MetaPropertyExtender.HasMetaProperty(drawing, "IsVisible") || (bool)MetaPropertyExtender.GetMetaProperty(drawing, "IsVisible"))
                    dc.DrawDrawing(drawing);
        }
    }
}
