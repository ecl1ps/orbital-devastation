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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Orbit.Gui
{
    /// <summary>
    /// Interaction logic for ColorItem.xaml
    /// </summary>
    public partial class ColorItem : UserControl
    {
        private Color color;

        public ColorItem(Color c)
        {
            color = c;
            InitializeComponent();
            grid.Background = new SolidColorBrush(color);
        }

        private void ColorItem_MouseDown(object sender, MouseButtonEventArgs e)
        {
            ColorPickerUC cp = LogicalTreeHelper.FindLogicalNode(Application.Current.MainWindow, "colorPicker") as ColorPickerUC;
            if (cp != null)
                cp.SelectedColor = color;

#if DEBUG
            foreach (System.Reflection.PropertyInfo prop in typeof(Colors).GetProperties())
            {
                if (prop.PropertyType.FullName == "System.Windows.Media.Color")
                {
                    if ((Color)prop.GetGetMethod().Invoke(null, null) == color)
                    {
                        Console.WriteLine(prop.Name + " " + color.ToString());
                        break;
                    }
                }
            }
#endif
        }
    }
}
