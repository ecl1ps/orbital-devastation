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
            lblActive.Content = data.Active ? "Playing" : "Spectating";
            lblScore.Content = "Score: " + data.Score;
            lblGold.Content = "Gold: " + data.Gold;
            lblMine.Content = "Mine: " + GetCharsForLevel(data.MineLevel);
            lblCannon.Content = "Cannon: " + GetCharsForLevel(data.CannonLevel);
            lblHook.Content = "Hook: " + GetCharsForLevel(data.HookLevel);
            lblHealingKit.Content = "Repair: " + GetCharsForLevel(data.HealingKitLevel);
            lblWon.Content = "Won: " + data.Won;
            lblPlayed.Content = "Played: " + data.Played;
        }

        private string GetCharsForLevel(UpgradeLevel lvl)
        {
            string s = "";
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
        public UpgradeLevel HealingKitLevel { get; set; }

        public PlayerOverviewData(string name, int score, int gold, bool active, int played, int won, 
            UpgradeLevel mine, UpgradeLevel cannon, UpgradeLevel hook, UpgradeLevel heal)
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
            HealingKitLevel = heal;
        }
    }
}
