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
using Orbit.Core.Weapons;

namespace Orbit.Gui
{
    /// <summary>
    /// Interaction logic for PlayerOverview.xaml
    /// </summary>
    public partial class PlayerOverview : UserControl
    {
        public PlayerOverview()
        {
            InitializeComponent();
        }

        public PlayerOverview(PlayerOverviewData data)
        {
            InitializeComponent();
            lblName.Content = data.Name;
            lblActive.Content = data.Active ? Strings.ui_playing : Strings.ui_spectating;
            lblScore.Content = String.Format(Strings.Culture, Strings.ui_match_points, data.Score);
            
            if (data.Active)
            {
                lblGold.Content = String.Format(Strings.Culture, Strings.ui_gold, data.Gold);
                lblMine.Content = String.Format(Strings.Culture, Strings.ui_mine_level, GetCharsForLevel(data.MineLevel));
                lblCannon.Content = String.Format(Strings.Culture, Strings.ui_cannon_level, GetCharsForLevel(data.CannonLevel));
                lblHook.Content = String.Format(Strings.Culture, Strings.ui_hook_level, GetCharsForLevel(data.HookLevel));
            }
            else
            {
                lblGold.Content = string.Empty;
                lblMine.Content = string.Empty;
                lblCannon.Content = string.Empty;
                lblHook.Content = string.Empty;
            }

            lblWon.Content = String.Format(Strings.Culture, Strings.ui_won, data.Won);
            lblPlayed.Content = String.Format(Strings.Culture, Strings.ui_played, data.Played);
        }

        private string GetCharsForLevel(UpgradeLevel lvl)
        {
            string s = string.Empty;
            for (int i = 0; i < (int)lvl; ++i)
                s += "I";
            return s;
        }
    }

    public class PlayerOverviewData
    {
        public string Name { get; set; }
        public int Score { get; set; }
        public int Gold { get; set; }
        public bool Active { get; set; }
        public int Played { get; set; }
        public int Won { get; set; }
        public UpgradeLevel MineLevel { get; set; }
        public UpgradeLevel CannonLevel { get; set; }
        public UpgradeLevel HookLevel { get; set; }

        public PlayerOverviewData(string name, int score, int gold, bool active, int played, int won, 
            UpgradeLevel mine, UpgradeLevel cannon, UpgradeLevel hook)
        {
            Name = name;
            Score = score;
            Gold = gold;
            Active = active;
            Played = played;
            Won = won;
            MineLevel = mine;
            CannonLevel = cannon;
            HookLevel = hook;
        }
    }
}
