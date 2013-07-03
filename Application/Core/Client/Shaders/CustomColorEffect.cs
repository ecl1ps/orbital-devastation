    using System;
    using System.Windows;
    using System.Windows.Media.Effects;
    using System.Diagnostics;
using System.Windows.Media;

namespace Orbit.Core.Client.Shaders
{
    class CustomColorEffect : ShaderEffect
    {
         #region Dependency Properties

        /// <summary>
        /// Gets or sets the Input of the shader.
        /// </summary>
        public static readonly DependencyProperty InputProperty = ShaderEffect.RegisterPixelShaderSamplerProperty("Input", typeof(CustomColorEffect), 0);

        public static readonly DependencyProperty ColorProperty = DependencyProperty.Register("color", typeof(Color), typeof(CustomColorEffect), new UIPropertyMetadata(Colors.Red, PixelShaderConstantCallback(0)));

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
        static CustomColorEffect()
        {
            pixelShader = new PixelShader();
            pixelShader.UriSource = new Uri("pack://application:,,,/resources/shaders/customColorEffect.ps");
        }

        /// <summary>
        /// Creates an instance and updates the shader's variables to the default values.
        /// </summary>
        public CustomColorEffect()
        {
            this.PixelShader = pixelShader;

            UpdateShaderValue(InputProperty);
            UpdateShaderValue(ColorProperty);
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

        public Color Color
        {
            get { return (Color)GetValue(ColorProperty); }
            set { SetValue(ColorProperty, value); }
        }
    }
}
