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
        public static Brush CreateConstantColorCircleGeometry(double radius, Color color)
        {
            UIElement elem = HeavyweightGeometryFactory.CreateConstantColorCircleGeometry(radius, color);

            RenderTargetBitmap renderTarget = PrepareRenderTarget(radius);
            renderTarget.Render(elem);
            renderTarget.Freeze();

            return new ImageBrush(renderTarget);
        }

        public static Brush CreateImageParticle(double radius, Color color, Uri source)
        {
            BitmapImage bi = new BitmapImage();
            bi.BeginInit();
            bi.UriSource = source;
            bi.EndInit();

            AlphaMaskEffect effect = new AlphaMaskEffect();
            effect.Color = color;

            RenderTargetBitmap rtb = PrepareRenderTarget(radius);

            System.Windows.Shapes.Rectangle visual = new System.Windows.Shapes.Rectangle();
            visual.Fill = new ImageBrush(bi);
            visual.Effect = effect;

            Size sz = new Size(radius * 2, radius * 2);
            visual.Measure(sz);
            visual.Arrange(new Rect(sz));

            rtb.Render(visual);

            return new ImageBrush(rtb);
        }

        private static RenderTargetBitmap PrepareRenderTarget(double radius) 
        {
            return new RenderTargetBitmap((int) (radius * 12) , (int) (radius * 12), 96, 96, PixelFormats.Pbgra32);
        }
    }
}
