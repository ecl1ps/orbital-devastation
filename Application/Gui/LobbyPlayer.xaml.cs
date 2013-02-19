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
        public bool currentPlayer;

        public LobbyPlayer()
        {
            InitializeComponent();
        }

        public LobbyPlayer(LobbyPlayerData data, bool withoutReadyIndicator = false)
        {
            InitializeComponent();
            if (withoutReadyIndicator)
                readyState.Visibility = Visibility.Hidden;
            else if (data.Leader)
                readyState.Fill = new SolidColorBrush(Color.FromRgb(0x10, 0x00, 0xef));
            else if (data.Ready)
                readyState.Fill = new SolidColorBrush(Color.FromRgb(0x00, 0xC0, 0x00));
            else
                readyState.Fill = new SolidColorBrush(Color.FromRgb(0xC0, 0x00, 0x00));

            PlayerId = data.Id;
            lblName.Content = data.Name;
            lblScore.Content = data.Score.ToString(Strings.Culture);
            lblWins.Content = "Won/Played: " + data.Won + "/" + data.Played;
            ColorBox.Background = new SolidColorBrush(data.Color);
            currentPlayer = data.CurrentPlayer;
            btnKick.Visibility = Visibility.Collapsed;
        }

        private void ColorPick(object sender, MouseButtonEventArgs e)
        {
            if (currentPlayer)
                App.Instance.AddMenu(new ColorPickerUC());
        }

        public void EnableKickButton()
        {
            if (!currentPlayer)
                btnKick.Visibility = Visibility.Visible;
        }

        private void btnKick_MouseDown(object sender, MouseButtonEventArgs e)
        {
            App.Instance.GetSceneMgr().Enqueue(new Action(() => App.Instance.GetSceneMgr().RequestKickPlayer(PlayerId)));
        }
    }

    public class LobbyPlayerData
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public int Score { get; set; }
        public bool CurrentPlayer { get; set; }
        public bool Leader { get; set; }
        public bool Ready { get; set; }
        public int Played { get; set; }
        public int Won { get; set; }
        public Color Color { get; set; }

        public LobbyPlayerData(int id, string name, int score, bool currentPlayer, bool leader, bool ready, int played, int won, Color c)
        {
            Id = id;
            Name = name;
            Score = score;
            CurrentPlayer = currentPlayer;
            Leader = leader;
            Ready = ready;
            Played = played;
            Won = won;
            Color = c;
        }
    }
}
