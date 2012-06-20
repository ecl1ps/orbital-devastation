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
    /// Interaction logic for LobbyPlayer.xaml
    /// </summary>
    public partial class LobbyPlayer : UserControl
    {
        public int PlayerId { get; set; }

        public LobbyPlayer(LobbyPlayerData data)
        {
            InitializeComponent();
            PlayerId = data.Id;
            lblName.Content = data.Name;
            lblScore.Content = data.Score;
            lblWins.Content = "Won: " + data.Won + "/" + data.Played;
        }
    }

    public class LobbyPlayerData
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public int Score { get; set; }
        public bool Leader { get; set; }
        public int Played { get; set; }
        public int Won { get; set; }

        public LobbyPlayerData(int id, string name, int score, bool leader, int played, int won)
        {
            Id = id;
            Name = name;
            Score = score;
            Leader = leader;
            Played = played;
            Won = won;
        }
    }
}
