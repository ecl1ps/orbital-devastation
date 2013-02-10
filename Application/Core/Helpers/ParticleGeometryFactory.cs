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
        public static Brush CreateConstantColorCircleGeometry(Color color, int renderSize)
        {
            UIElement elem = HeavyweightGeometryFactory.CreateConstantColorCircleGeometry(renderSize, color);

            RenderTargetBitmap renderTarget = PrepareRenderTarget(renderSize);
            renderTarget.Render(elem);
            renderTarget.Freeze();

            return new ImageBrush(renderTarget);
        }

        public static Brush CreateImageParticle(Color color, Uri source, int renderSize)
        {
            BitmapImage bi = new BitmapImage();
            bi.BeginInit();
            bi.UriSource = source;
            bi.EndInit();

            AlphaMaskEffect effect = new AlphaMaskEffect();
            effect.Color = color;

            RenderTargetBitmap rtb = PrepareRenderTarget(renderSize);

            System.Windows.Shapes.Rectangle visual = new System.Windows.Shapes.Rectangle();
            visual.Fill = new ImageBrush(bi);
            visual.Effect = effect;

            Size sz = new Size(renderSize, renderSize);
            visual.Measure(sz);
            visual.Arrange(new Rect(sz));

            rtb.Render(visual);

            return new ImageBrush(rtb);
        }

        private static RenderTargetBitmap PrepareRenderTarget(int size) 
        {
            return new RenderTargetBitmap(size , size, 96, 96, PixelFormats.Pbgra32);
        }
    }
}
