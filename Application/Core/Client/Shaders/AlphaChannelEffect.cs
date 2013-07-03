using System;
using System.Windows;
using System.Windows.Media.Effects;
using System.Diagnostics;
using System.Windows.Media;

namespace Orbit.Core.Client.Shaders
{
    class AlphaChannelEffect : ShaderEffect
    {
        #region Dependency Properties

        /// <summary>
        /// Gets or sets the Input of the shader.
        /// </summary>
        public static readonly DependencyProperty InputProperty = ShaderEffect.RegisterPixelShaderSamplerProperty("Input", typeof(AlphaChannelEffect), 0);
        public static readonly DependencyProperty AlphaProperty = DependencyProperty.Register("alfa", typeof(float), typeof(AlphaChannelEffect), new UIPropertyMetadata(1.0f, PixelShaderConstantCallback(0)));
        #endregion

        #region Member Data

        /// <summary>
        /// The shader instance.
        /// </summary>
        private static PixelShader pixelShader;

        #endregion

        #region Constructors

        /// <summary>
        /// Creates an instance of the shader from the included pixel shader.
        /// </summary>
        static AlphaChannelEffect()
        {
            pixelShader = new PixelShader();
            pixelShader.UriSource = new Uri("pack://application:,,,/resources/shaders/alphaChannelEffect.ps");
        }

        /// <summary>
        /// Creates an instance and updates the shader's variables to the default values.
        /// </summary>
        public AlphaChannelEffect()
        {
            this.PixelShader = pixelShader;

            UpdateShaderValue(InputProperty);
        }

        #endregion

        /// <summary>
        /// Gets or sets the input used in the shader.
        /// </summary>
        [System.ComponentModel.BrowsableAttribute(false)]
        public Brush Input
        {
            get { return (Brush)GetValue(InputProperty); }
            set { SetValue(InputProperty, value); }
        }
        public float Alpha
        {
            get { return (float)GetValue(AlphaProperty); }
            set { SetValue(AlphaProperty, value); }
        }
    }
}
