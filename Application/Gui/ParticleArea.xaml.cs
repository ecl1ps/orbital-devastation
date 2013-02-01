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
using System.Windows.Media.Imaging;
using System.Windows.Media.Media3D;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Orbit.Gui
{
    /// <summary>
    /// Interaction logic for ParticleArea.xaml
    /// </summary>
    public partial class ParticleArea : UserControl
    {
        public Viewport3D ViewPort { get { return World; } }
        public Model3DGroup Models { get { return WorldModels; } }

        public ParticleArea()
        {
            InitializeComponent();
        }
    }
}
