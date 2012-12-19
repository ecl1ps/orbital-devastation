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

        public LobbyPlayer()
        {
            InitializeComponent();
        }

        public LobbyPlayer(LobbyPlayerData data)
        {
            InitializeComponent();
            if (data.Leader)
                readyState.Fill = new SolidColorBrush(Color.FromRgb(0x90, 0x00, 0x90));
            else if (data.Ready)
                readyState.Fill = new SolidColorBrush(Color.FromRgb(0x00, 0xC0, 0x00));
            else
                readyState.Fill = new SolidColorBrush(Color.FromRgb(0xC0, 0x00, 0x00));

            PlayerId = data.Id;
            lblName.Content = data.Name;
            lblScore.Content = data.Score;
            lblWins.Content = "Won/Played: " + data.Won + "/" + data.Played;
            colorBox.Background = new SolidColorBrush(data.Color);
        }
    }

    public class LobbyPlayerData
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public int Score { get; set; }
        public bool Leader { get; set; }
        public bool Ready { get; set; }
        public int Played { get; set; }
        public int Won { get; set; }
        public Color Color { get; set; }

        public LobbyPlayerData(int id, string name, int score, bool leader, bool ready, int played, int won, Color c)
        {
            Id = id;
            Name = name;
            Score = score;
            Leader = leader;
            Ready = ready;
            Played = played;
            Won = won;
            Color = c;
        }
    }
}
