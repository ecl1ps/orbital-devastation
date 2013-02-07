using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Effects;

namespace Orbit.Core.Client.Shaders
{
    class AlphaMaskEffect : ShaderEffect
    {
        #region Dependency Properties

        /// <summary>
        /// Gets or sets the Input of the shader.
        /// </summary>
        public static readonly DependencyProperty InputProperty = ShaderEffect.RegisterPixelShaderSamplerProperty("Input", typeof(AlphaMaskEffect), 0);
        public static readonly DependencyProperty ColorProperty = DependencyProperty.Register("colorToWrite", typeof(Color), typeof(AlphaMaskEffect), new UIPropertyMetadata(Colors.White, PixelShaderConstantCallback(0)));
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
        static AlphaMaskEffect()
        {
            pixelShader = new PixelShader();
            pixelShader.UriSource = new Uri("pack://application:,,,/resources/shaders/alphaMaskEffect.ps");
        }

        /// <summary>
        /// Creates an instance and updates the shader's variables to the default values.
        /// </summary>
        public AlphaMaskEffect()
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
        public Color Color
        {
            get { return (Color)GetValue(ColorProperty); }
            set { SetValue(ColorProperty, value); }
        }
    }
}
