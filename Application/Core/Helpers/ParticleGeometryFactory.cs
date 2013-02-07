using Orbit.Core.Client.Shaders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace Orbit.Core.Helpers
{
    class ParticleGeometryFactory
    {
        public static Visual CreateConstantColorCircleGeometry(double radius, Color color)
        {
            Ellipse e = new Ellipse();
            e.Width = radius;
            e.Height = radius;
            SolidColorBrush b = new SolidColorBrush(color);
            e.Fill = b;
            e.Measure(new System.Windows.Size(radius, radius));
            e.Arrange(new Rect(0, 0, radius, radius));

            return e;
        }

        public static RenderTargetBitmap CreateImageParticle(double radius, Color color, Uri source)
        {
            BitmapImage bi = new BitmapImage();
            bi.BeginInit();
            bi.UriSource = source;
            bi.EndInit();

            AlphaMaskEffect effect = new AlphaMaskEffect();
            effect.Color = Colors.Green;

            RenderTargetBitmap rtb = new RenderTargetBitmap((int)radius * 2, (int)radius * 2, 96, 96, PixelFormats.Pbgra32);

            System.Windows.Shapes.Rectangle visual = new System.Windows.Shapes.Rectangle();
            visual.Fill = new ImageBrush(bi);
            visual.Effect = effect;

            Size sz = new Size(radius * 2, radius * 2);
            visual.Measure(sz);
            visual.Arrange(new Rect(sz));

            rtb.Render(visual);
            return rtb;
        }
    }
}
