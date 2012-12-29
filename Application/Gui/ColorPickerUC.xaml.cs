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
using Orbit.Core;
using Orbit.Core.Players;

namespace Orbit.Gui
{
    /// <summary>
    /// Interaction logic for ColorPickerUC.xaml
    /// </summary>
    public partial class ColorPickerUC : UserControl
    {
        private Color selected;
        public Color SelectedColor 
        {
            get { return selected; }
            set { selected = value; CreateBaseImage(value); }
        }
        private bool fromEsc;

        public ColorPickerUC(bool fromEscMenu = false)
        {
            selected = Colors.Transparent;
            fromEsc = fromEscMenu;
            InitializeComponent();
            foreach (Color c in PlayerColorManager.GetAvailableColors())
                spColors.Children.Add(new ColorItem(c));
            CreateBaseImage(Player.GetChosenColor());
        }

        private void btnOk_Click(object sender, RoutedEventArgs e)
        {
            if (SelectedColor != null && SelectedColor != Colors.Transparent)
            {
                GameProperties.Props.SetAndSave(PropertyKey.CHOSEN_COLOR, SelectedColor.ToString());
                (Application.Current as App).GetSceneMgr().Enqueue(new Action(() =>
                {
                    (Application.Current as App).GetSceneMgr().PlayerColorChanged();
                }));
            }
            Close();
        }

        private void Close()
        {
            if (fromEsc)
                (Application.Current as App).AddMenu(new PlayerSettings());
            else
                (Application.Current as App).ClearMenus();
        }

        private void btncancel_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void CreateBaseImage(Color c)
        {
            BitmapImage bi = new BitmapImage();
            bi.BeginInit();
            bi.UriSource = new Uri("pack://application:,,,/resources/images/base/base_bw_shaded.png");
            bi.EndInit();

            Image img = new Image();
            img.Source = bi;
            img.Width = 240;
            img.Height = 60;

            ColorReplaceEffect effect = new ColorReplaceEffect();
            effect.ColorToOverride = Colors.White;
            effect.ColorReplace = c;

            img.Effect = effect;

            baze.Children.Clear();
            baze.Children.Add(img);
        }
    }
}
