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
    /// Interaction logic for PlayerStats.xaml
    /// </summary>
    public partial class PlayerStatsUC : UserControl
    {
        public PlayerStatsUC()
        {
            InitializeComponent();
        }

        public void setData(String mineStats, String bulletStats, String hookStats,
            String damageTaken, String healed, String goldEarned, String actionsUsed,
            String powerupPicked, String favAction, String favPowerup)
        {
            MineStats.Text = mineStats;
            BulletStats.Text = bulletStats;
            HookStats.Text = hookStats;
            Damage.Text = damageTaken;
            Heal.Text = healed;
            Gold.Text = goldEarned;
            Actions.Text = actionsUsed;
            Powerup.Text = powerupPicked;
            FavAction.Text = favAction;
            FavPowerup.Text = favPowerup;

        }
    }
}
