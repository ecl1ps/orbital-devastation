using Orbit.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Effects;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Orbit.Gui
{
    /// <summary>
    /// Interaction logic for CooldownVisualiser.xaml
    /// </summary>
    public partial class CooldownVisualiser : UserControl
    {
        private ArcSegment arc;
        private Path path;
        private BlurEffect effect;

        private float grow = 0;
        public float Grow { get { return grow; } set { grow = value; effect.Radius = value; } }
        private bool growing = true;
        private float maxGrow = 8;
        private float growTime = 1;
        private float time = 0;

        public CooldownVisualiser()
        {
            InitializeComponent();

            arc = new ArcSegment();

            PathFigure figure = new PathFigure();
            figure.StartPoint = new Point(50, 25);

            arc = new ArcSegment();
            arc.SweepDirection = SweepDirection.Clockwise;
            arc.IsLargeArc = true;
            arc.Size = new Size(25, 25);
            arc.Point = ComputePointOnCircle(0);

            figure.Segments.Add(arc);

            PathGeometry geom = new PathGeometry();
            geom.Figures.Add(figure);

            path = new Path();
            path.Data = geom;
            path.Stroke = new SolidColorBrush(Colors.Blue);
            path.StrokeThickness = 2;

            effect = new BlurEffect();
            effect.KernelType = KernelType.Gaussian;
            effect.Radius = 0;
            path.Effect = effect;

            Panel.Children.Add(path);
        }

        public void SetPercentage(float percentage)
        {
            if (percentage < 0.5)
                arc.IsLargeArc = false;
            else
                arc.IsLargeArc = true;

            arc.Point = ComputePointOnCircle(FastMath.LinearInterpolate(0, Math.PI * 2, percentage));
        }

        private Point ComputePointOnCircle(double angle)
        {
            Point p = new Point();
            p.X = 25 * Math.Cos(angle) + 25;
            p.Y = 25 * Math.Sin(angle) + 24;

            return p;
        }


        public void Update(float tpf)
        {
            if (effect == null)
                return;

            if (growing)
            {
                time += tpf;
                grow = FastMath.LinearInterpolate(0, maxGrow, time / growTime);
            }
            else
            {
                time += tpf;
                grow = FastMath.LinearInterpolate(maxGrow, 0, time / growTime);
            }

            if (time >= growTime) 
            {
                growing = !growing;
                time = 0;
            }

            effect.Radius = Grow;
        }
    }
}
