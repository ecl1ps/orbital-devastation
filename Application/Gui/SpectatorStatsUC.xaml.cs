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
    /// Interaction logic for SpectatorStatsUC.xaml
    /// </summary>
    public partial class SpectatorStatsUC : UserControl
    {
        public SpectatorStatsUC()
        {
            InitializeComponent();
        }

        public void SetData(String gameTime, String deadTime, String actions, String favAction, String damage) 
        {
            GameTime.Text = gameTime;
            DeadTime.Text = deadTime;
            ActionsUsed.Text = actions;
            FavAction.Text = favAction;
            Damage.Text = damage;
        }
    }
}
