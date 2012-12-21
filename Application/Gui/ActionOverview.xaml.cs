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

namespace Orbit.Gui
{
    /// <summary>
    /// Interaction logic for ActionOverview.xaml
    /// </summary>
    public partial class ActionOverview : UserControl, IActionOverview
    {
        public ActionOverview()
        {
            InitializeComponent();
        }

        public void RenderAction(ISpectatorAction action)
        {
            //if (action.ImageSource != null)
              //  Icon.SetImageUri(action.ImageSource);
        }

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {

        }
    }
}
