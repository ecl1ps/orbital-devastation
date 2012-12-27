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
using Orbit.Gui.InteractivePanel;
using Orbit.Core.SpecialActions.Spectator;
using Orbit.Core.SpecialActions;

namespace Orbit.Gui
{
    /// <summary>
    /// Interaction logic for ActionOverview.xaml
    /// </summary>
    public partial class ActionOverview : UserControl, IActionOverview
    {
        private ISpectatorAction action = null;

        public ActionOverview()
        {
            InitializeComponent();
        }

        public void RenderAction(ISpectatorAction action)
        {
            if (action == null)
            {
                Visibility = System.Windows.Visibility.Collapsed;
                return;
            }
            else
                Visibility = System.Windows.Visibility.Visible;

            this.action = action;

            Dispatcher.BeginInvoke(new Action(() => {
                if (action.ImageSource != null)
                {
                    BitmapImage image = new BitmapImage();
                    image.BeginInit();
                    image.UriSource = new Uri(action.ImageSource);
                    image.EndInit();
                    Icon.Source = image;
                }

                CountNormal.Text = action.ComputeMissing(action.Normal).ToString();
                CountGold.Text = action.ComputeMissing(action.Gold).ToString();
            }));
        }
    }
}
