using System;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Effects;

namespace Orbit
{
    public class ColorReplaceEffect : ShaderEffect
    {
        #region Constructors

        static ColorReplaceEffect()
        {
            _pixelShader.UriSource = new Uri("pack://application:,,,/resources/shaders/colorReplaceEffect.ps");
        }

        public ColorReplaceEffect()
        {
            this.PixelShader = _pixelShader;

            // Update each DependencyProperty that's registered with a shader register.  This
            // is needed to ensure the shader gets sent the proper default value.
            UpdateShaderValue(InputProperty);
            UpdateShaderValue(ColorOverrideProperty);
            UpdateShaderValue(ColorReplaceProperty);
        }

        #endregion

        #region Dependency Properties

        public Brush Input
        {
            get { return (Brush)GetValue(InputProperty); }
            set { SetValue(InputProperty, value); }
        }

        // Brush-valued properties turn into sampler-property in the shader.
        // This helper sets "ImplicitInput" as the default, meaning the default
        // sampler is whatever the rendering of the element it's being applied to is.
        public static readonly DependencyProperty InputProperty =
            ShaderEffect.RegisterPixelShaderSamplerProperty("Input", typeof(ColorReplaceEffect), 0);



        public Color ColorToOverride
        {
            get { return (Color)GetValue(ColorOverrideProperty); }
            set { SetValue(ColorOverrideProperty, value); }
        }

        // Scalar-valued properties turn into shader constants with the register
        // number sent into PixelShaderConstantCallback().
        public static readonly DependencyProperty ColorOverrideProperty =
            DependencyProperty.Register("colorToOverride", typeof(Color), typeof(ColorReplaceEffect),
                    new UIPropertyMetadata(Colors.Yellow, PixelShaderConstantCallback(0)));

        public Color ColorReplace
        {
            get { return (Color)GetValue(ColorReplaceProperty); }
            set { SetValue(ColorReplaceProperty, value); }
        }

        // Scalar-valued properties turn into shader constants with the register
        // number sent into PixelShaderConstantCallback().
        public static readonly DependencyProperty ColorReplaceProperty =
            DependencyProperty.Register("colorToWrite", typeof(Color), typeof(ColorReplaceEffect),
                    new UIPropertyMetadata(Colors.Yellow, PixelShaderConstantCallback(1)));

        #endregion

        #region Member Data

        private static PixelShader _pixelShader = new PixelShader();

        #endregion

    }
}
