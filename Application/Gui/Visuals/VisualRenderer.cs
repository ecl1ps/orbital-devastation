using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Media;
using System.Windows;

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
            ExtraDrawingVisual curr;
            for (int i = 0; i < area.GetChildrenCount(); ++i)
            {
                curr = area.GetChild(i) as ExtraDrawingVisual;
                DrawAllDrawings(curr, dc);
                //dc.DrawDrawing(curr.Drawing);
            }
        }

        private void DrawAllDrawings(ExtraDrawingVisual curr, DrawingContext dc)
        {
            foreach (Drawing drawing in curr.DrawingChildren)
                RenderDrawing(drawing, dc);
        }

        private void RenderDrawing(Drawing drawing, DrawingContext dc)
        {
            if (drawing.GetType() != typeof(DrawingGroup))
            {
                dc.DrawDrawing(drawing);
            }
            else
                Logger.Warn("Rendering DrawingGroup object is not supported yet");
        }
    }
}
