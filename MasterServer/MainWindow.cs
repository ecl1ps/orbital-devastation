using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Orbit.Core.Server;
using log4net.Appender;
using System.Runtime.CompilerServices;

namespace MasterServer
{
    public partial class MainWindow : Form, IAppender, INotifyPropertyChanged
    {
        private static readonly log4net.ILog Logger = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        private MasterServerMgr server;

        private int players = 0;
        private int totalPlayers = 0;
        private int quickGames = 0;
        private int tournaments = 0;
        private int totalQuickGames = 0;
        private int totalTournaments = 0;
        private int runningGames = 0;
        private int playedGames = 0;

        public int Players { get { return players; } set { players = value; NotifyPropertyChanged(); } }
        public int TotalPlayers { get { return totalPlayers; } set { totalPlayers = value; NotifyPropertyChanged(); } }

        public int QuickGames { get { return quickGames; } set { quickGames = value; RunningGames = QuickGames + Tournaments; NotifyPropertyChanged(); } }
        public int Tournaments { get { return tournaments; } set { tournaments = value; RunningGames = QuickGames + Tournaments; NotifyPropertyChanged(); } }

        public int TotalQuickGames { get { return totalQuickGames; } set { totalQuickGames = value; PlayedGames = TotalQuickGames + TotalTournaments; NotifyPropertyChanged(); } }
        public int TotalTournaments { get { return totalTournaments; } set { totalTournaments = value; PlayedGames = TotalQuickGames + TotalTournaments; NotifyPropertyChanged(); } }

        public int RunningGames { get { return runningGames; } set { runningGames = value; NotifyPropertyChanged(); } }
        public int PlayedGames { get { return playedGames; } set { playedGames = value; NotifyPropertyChanged(); } }

        public MainWindow()
        {
            InitializeComponent();
           
            // iniciace "pack"
            string s = System.IO.Packaging.PackUriHelper.UriSchemePack;
        }

        private void btnExit_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        private void Form1_FormClosed(object sender, FormClosedEventArgs e)
        {
            server.Shutdown();
            ((log4net.Repository.Hierarchy.Hierarchy)log4net.LogManager.GetRepository()).Root.RemoveAppender(this);
        }

        public void DoAppend(log4net.Core.LoggingEvent loggingEvent)
        {
            BeginInvoke(new Action(() => 
            {
                lbOut.Items.Add(String.Format("{0}: {1}", loggingEvent.Level.Name, loggingEvent.MessageObject.ToString()));
                lbOut.SelectedIndex = lbOut.Items.Count - 1;
            }));
        }

        private void MainWindow_Load(object sender, EventArgs e)
        {
            ((log4net.Repository.Hierarchy.Hierarchy)log4net.LogManager.GetRepository()).Root.AddAppender(this);
            server = new MasterServerMgr();
            server.PlayerConnectedCallback = PlayerConnected;
            server.PlayerDisconnectedCallback = PlayerDisconnected;
            server.GameStartedCallback = GameStarted;
            server.GameEndedCallback = GameEnded;
            lblStartTime.Text = DateTime.Now.ToString();
        }

        private void PlayerConnected()
        {
            Players++;
            TotalPlayers++;
        }

        private void PlayerDisconnected()
        {
            Players--;
        }

        private void GameStarted(bool tournament)
        {
            if (tournament)
            {
                Tournaments++;
                TotalTournaments++;
            }
            else
            {
                QuickGames++;
                TotalQuickGames++;
            }
        }

        private void GameEnded(bool tournament)
        {
            if (tournament)
                Tournaments--;
            else
                QuickGames--;
        }

        public event PropertyChangedEventHandler PropertyChanged;

        private void NotifyPropertyChanged([CallerMemberName] String propertyName = "")
        {
            BeginInvoke(new Action(() =>
            {
                if (PropertyChanged != null)
                    PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }));
        }
    }
}
