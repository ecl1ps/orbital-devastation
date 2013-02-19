using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using Lidgren.Network;
using Orbit.Core;
using Orbit.Core.AI;
using Orbit.Core.Players;
using Orbit.Core.Server.Level;
using Orbit.Core.Server.Match;
using Orbit.Core.Helpers;
using System.Net;
using System.Collections.ObjectModel;

namespace Orbit.Gui
{
    /// <summary>
    /// Interaction logic for TournamentFinderUC.xaml
    /// </summary>
    public partial class TournamentFinderUC : UserControl
    {
        private static readonly log4net.ILog Logger = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        private NetClient client;
        private string serverAddress;

        private ObservableCollection<VisualizableTorunamentSettings> availableTournaments = new ObservableCollection<VisualizableTorunamentSettings>();
        public ObservableCollection<VisualizableTorunamentSettings> AvailableTournaments { get { return availableTournaments; } }

        public TournamentFinderUC(string serverAddress)
        {
            InitializeComponent();
            CheckOnlineStatus();
            PostInit();
            this.serverAddress = serverAddress;
            StartClient();
            RequestTournaments();
        }

        private void CheckOnlineStatus()
        {
            // TODO: zkontrolovat jestli na adrese serveru opravdu bezi aplikace
            try
            {
                NetUtility.ResolveAsync(SharedDef.MASTER_SERVER_ADDRESS, delegate(IPAddress adr)
                {
                    if (adr == null)
                    {
                        lblStatus.Dispatcher.BeginInvoke(new Action(() =>
                        {
                            lblStatus.Content = "Connection to the server is unavailable.";
                            lblStatus.Foreground = Brushes.Red;
                        }));
                    }
                    else
                    {
                        lblStatus.Dispatcher.BeginInvoke(new Action(() =>
                        {
                            lblStatus.Content = "Connection to the server is online.";
                            lblStatus.Foreground = Brushes.Green;
                        }));
                    }
                });
            }
            catch (System.Exception /*e*/)
            {
                lblStatus.Dispatcher.BeginInvoke(new Action(() =>
                {
                    lblStatus.Content = "Connection is unavailable.";
                    lblStatus.Foreground = Brushes.Red;
                }));
            }
        }

        private void StartClient()
        {
            NetPeerConfiguration conf = new NetPeerConfiguration("Orbit");
            conf.EnableMessageType(NetIncomingMessageType.UnconnectedData);
            client = new NetClient(conf);
            client.RegisterReceivedCallback(new SendOrPostCallback(GotMessage)); 
            client.Start();
        }

        public void GotMessage(object peer)
        {
            client = peer as NetClient;
            NetIncomingMessage msg = client.ReadMessage();

            switch (msg.MessageType)
            {
                case NetIncomingMessageType.UnconnectedData:
                    if (msg.ReadByte() == (byte)PacketType.AVAILABLE_TOURNAMENTS_RESPONSE)
                        ReceivedTournaments(msg);
                    break;
                default:
                    break;
            }
            client.Recycle(msg);
        }

        private void ReceivedTournaments(NetIncomingMessage msg)
        {
            availableTournaments.Clear();

            int count = msg.ReadInt32();
            for (int i = 0; i < count; ++i)
                availableTournaments.Add(new VisualizableTorunamentSettings(msg.ReadTournamentSettings()));

            lblTournamentCount.Content = count.ToString(Strings.Culture);
        }

        private void btnJoinTournament_Click(object sender, RoutedEventArgs e)
        {
            VisualizableTorunamentSettings s = lvTournaments.SelectedItem as VisualizableTorunamentSettings;
            if (s == null)
                return;

            App.Instance.StartTournamentGame(serverAddress, s.Settings.ServerId);
        }

        private void btnRefresh_Click(object sender, RoutedEventArgs e)
        {
            RequestTournaments();
        }

        private void RequestTournaments()
        {
            NetOutgoingMessage msg = client.CreateMessage();
            msg.Write((byte)PacketType.AVAILABLE_TOURNAMENTS_REQUEST);
            client.SendUnconnectedMessage(msg, serverAddress, SharedDef.MASTER_SERVER_PORT);
        }

        private void PostInit()
        {
            List<ComboData> data = new List<ComboData>();
            foreach (MatchManagerType t in Enum.GetValues(typeof(MatchManagerType)))
            {
                MatchManagerInfo i = MatchManagerInfoAccessor.GetInfo(t);
#if DEBUG   // v debugu pridat vsechny managery
                data.Add(new ComboData { Id = t, Name = i.Text });
#else
                if (!i.IsDebug)
                    data.Add(new ComboData { Id = t, Name = i.Text });
#endif
            }

            cbType.ItemsSource = data;
            cbType.DisplayMemberPath = "Name";
            cbType.SelectedValuePath = "Id";
            cbType.SelectedValue = MatchManagerType.ONLY_SCORE;

            data = new List<ComboData>();
            foreach (GameLevel l in Enum.GetValues(typeof(GameLevel)))
            {
                LevelInfo i = LevelInfoAccessor.GetInfo(l);
#if DEBUG   // v debugu pridat vsechny mapy
                data.Add(new ComboData { Id = l, Name = i.Text });
#else
                if (!i.IsDebug)
                    data.Add(new ComboData { Id = l, Name = i.Text });
#endif
            }

            cbMap.ItemsSource = data;
            cbMap.DisplayMemberPath = "Name";
            cbMap.SelectedValuePath = "Id";
            cbMap.SelectedValue = GameLevel.BASIC_MAP;

#if DEBUG
            // pridani dostupnych botu pro testovani
            data = new List<ComboData>();
            data.Add(new ComboData { Id = BotType.LEVEL1, Name = BotNameAccessor.GetBotName(BotType.LEVEL1) });
            data.Add(new ComboData { Id = BotType.LEVEL2, Name = BotNameAccessor.GetBotName(BotType.LEVEL2) });

            cbBot.ItemsSource = data;
            cbBot.DisplayMemberPath = "Name";
            cbBot.SelectedValuePath = "Id";
            cbBot.SelectedValue = BotType.LEVEL1;
#else
            tbBotCount.Visibility = Visibility.Hidden;
            cbBot.Visibility = Visibility.Hidden;
            lblBot.Visibility = Visibility.Hidden;
            lblBotCount.Visibility = Visibility.Hidden;
#endif
        }

        [System.Reflection.ObfuscationAttribute(Feature = "properties renaming")]
        class ComboData
        {
            public object Id { get; set; }
            public string Name { get; set; }
        }

        private void btnCreateTournament_Click(object sender, RoutedEventArgs e)
        {
            if (String.IsNullOrWhiteSpace(tbName.Text))
                return;

            TournamentSettings s = new TournamentSettings();
            s.Name = tbName.Text;
            s.Leader = App.Instance.PlayerName;
            s.MMType = (MatchManagerType)cbType.SelectedValue;
            s.Level = (GameLevel)cbMap.SelectedValue;
            try
            {
                s.RoundCount = int.Parse(tbRounds.Text, CultureInfo.InvariantCulture);
                if (s.RoundCount < 1)
                    throw new Exception();
            }
            catch (Exception)
            {
                s.RoundCount = 1;
                tbRounds.Text = 1.ToString(Strings.Culture);
            }

#if !DEBUG
            s.BotCount = 0;
            s.BotType = SharedDef.DEFAULT_BOT;
#else
            try
            {
                s.BotCount = int.Parse(tbBotCount.Text, CultureInfo.InvariantCulture);
            }
            catch (Exception)
            {
                s.BotCount = 0;
            }

            s.BotType = (BotType)cbBot.SelectedValue;
#endif

            App.Instance.StartTournamentLobby(serverAddress);

            App.Instance.GetSceneMgr().Enqueue(new Action(() =>
            {
                App.Instance.GetSceneMgr().ProcessNewTournamentSettings(s);
            }));

            LobbyUC lobby = LogicalTreeHelper.FindLogicalNode(Application.Current.MainWindow, "lobbyWindow") as LobbyUC;
            if (lobby != null)
                lobby.UpdateTournamentSettings(s);
        }

        private void btnClose_Click(object sender, RoutedEventArgs e)
        {
            App.Instance.ShowStartScreen();
        }
    }
}
