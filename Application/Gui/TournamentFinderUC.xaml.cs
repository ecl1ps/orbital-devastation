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
        private Timer requestTimer;

        private ObservableCollection<VisualizableTorunamentSettings> availableTournaments = new ObservableCollection<VisualizableTorunamentSettings>();
        private bool statusReceived;
        public ObservableCollection<VisualizableTorunamentSettings> AvailableTournaments { get { return availableTournaments; } }

        public delegate void ReceivedNewTournaments(List<TournamentSettings> tss, string serverAddress);
        public ReceivedNewTournaments ReceivedNewTournamentsCallback { get; set; }

        private enum OnlineStatus
        {
            CHECKING,
            ONLINE,
            OFFLINE
        }

        public TournamentFinderUC(string serverAddress)
        {
            InitializeComponent();
            CheckOnlineStatus();
            PostInit();
            this.serverAddress = serverAddress;
            StartClient();
            requestTimer = new Timer(RequestTournaments);
            requestTimer.Change(0, SharedDef.TOURNAMENT_LIST_REQUEST_INTERVAL);

            NetOutgoingMessage msg = client.CreateMessage();
            msg.Write((byte)PacketType.AVAILABLE_RECONNECT_REQUEST);
            msg.Write(App.Instance.PlayerHashId);
            client.SendUnconnectedMessage(msg, serverAddress, SharedDef.MASTER_SERVER_PORT);
        }

        private void CheckOnlineStatus()
        {
            try
            {
                NetUtility.ResolveAsync(SharedDef.MASTER_SERVER_ADDRESS, delegate(IPAddress adr)
                {
                    if (adr == null)
                        SetOnlineStatus(OnlineStatus.OFFLINE);
                    else
                        SetOnlineStatus(OnlineStatus.ONLINE);
                });
            }
            catch (Exception)
            {
                SetOnlineStatus(OnlineStatus.OFFLINE);
            }
        }

        private void SetOnlineStatus(OnlineStatus status)
        {
            switch (status)
            {
                case OnlineStatus.CHECKING:
                    Dispatcher.BeginInvoke(new Action(() =>
                    {
                        lblConnectionStatus.Content = "Checking online status. Please wait...";
                        lblConnectionStatus.Foreground = Brushes.Goldenrod;
                    }));
                    break;
                case OnlineStatus.ONLINE:
                    Dispatcher.BeginInvoke(new Action(() =>
                    {
                        lblConnectionStatus.Content = "Online";
                        lblConnectionStatus.Foreground = Brushes.Green;
                    }));
                    break;
                case OnlineStatus.OFFLINE:
                    Dispatcher.BeginInvoke(new Action(() =>
                    {
                        lblConnectionStatus.Content = "Offline";
                        lblConnectionStatus.Foreground = Brushes.Red;
                    }));
                    break;
            }
        }

        private void SetServerStatus(OnlineStatus status)
        {
            switch (status)
            {
                case OnlineStatus.CHECKING:
                    lblServerStatus.Content = "Checking server status. Please wait...";
                    lblServerStatus.Foreground = Brushes.Goldenrod;
                    btnCreateTournament.IsEnabled = false;
                    btnJoinTournament.IsEnabled = false;
                    break;
                case OnlineStatus.ONLINE:
                    lblServerStatus.Content = "Online";
                    lblServerStatus.Foreground = Brushes.Green;
                    btnCreateTournament.IsEnabled = true;
                    btnJoinTournament.IsEnabled = true;
                    break;
                case OnlineStatus.OFFLINE: // unused atm
                    lblServerStatus.Content = "Offline";
                    lblServerStatus.Foreground = Brushes.Red;
                    btnCreateTournament.IsEnabled = false;
                    btnJoinTournament.IsEnabled = false;
                    break;
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
                    PacketType type = (PacketType)msg.ReadByte();
                    if (type == PacketType.AVAILABLE_TOURNAMENTS_RESPONSE)
                        ReceivedTournaments(msg);
                    else if (type == PacketType.AVAILABLE_RECONNECT_RESPONSE)
                        ReceivedAvailableReconnect();
                    break;
                default:
                    break;
            }
            client.Recycle(msg);
        }

        private void ReceivedAvailableReconnect()
        {
            App.Instance.AddMenu(new ReconnectUC(serverAddress));
        }

        private void btnJoinTournament_Click(object sender, RoutedEventArgs e)
        {
            JoinTournament(lvTournaments.SelectedItem as VisualizableTorunamentSettings);

        }

        private void lvTournaments_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            JoinTournament(((FrameworkElement)e.OriginalSource).DataContext as VisualizableTorunamentSettings);
        }

        private void JoinTournament(VisualizableTorunamentSettings s)
        {
            if (s == null || s.Settings.Running || s.Settings.PlayedMatches > 0)
                return;

            App.Instance.StartTournamentGame(serverAddress, s.Settings.ServerId);
        }
        private void btnRefresh_Click(object sender, RoutedEventArgs e)
        {
            CheckOnlineStatus();
            RequestTournaments(null);
        }

        private void ScheduleTimeoutCheck()
        {
            Timer stateTimer = new Timer(ServerStatusTimeout);
            stateTimer.Change(3000, Timeout.Infinite);
        }

        private void ServerStatusTimeout(object status)
        {
            Dispatcher.Invoke(new Action(() =>
            {
                if (!statusReceived)
                    SetServerStatus(OnlineStatus.OFFLINE);
            }));
        }

        private void RequestTournaments(object status)
        {
            Dispatcher.BeginInvoke(new Action(() =>
            {
                ScheduleTimeoutCheck();
                SetServerStatus(OnlineStatus.CHECKING);
                statusReceived = false;

                NetOutgoingMessage msg = client.CreateMessage();
                msg.Write((byte)PacketType.AVAILABLE_TOURNAMENTS_REQUEST);
                client.SendUnconnectedMessage(msg, serverAddress, SharedDef.MASTER_SERVER_PORT);
            }));
        }

        private void ReceivedTournaments(NetIncomingMessage msg)
        {
            SetServerStatus(OnlineStatus.ONLINE);
            statusReceived = true;

            availableTournaments.Clear();
            List<TournamentSettings> tss = new List<TournamentSettings>();

            int count = msg.ReadInt32();
            for (int i = 0; i < count; ++i)
            {
                TournamentSettings ts = msg.ReadTournamentSettings();
                tss.Add(ts);
                availableTournaments.Add(new VisualizableTorunamentSettings(ts));
            }

            if (ReceivedNewTournamentsCallback != null)
                ReceivedNewTournamentsCallback(tss, serverAddress);

            lblTournamentCount.Content = count.ToString(Strings.Culture);
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
            tbName.Text = "Test";

            // pridani dostupnych botu pro testovani
            data = new List<ComboData>();
            data.Add(new ComboData { Id = BotType.LEVEL1, Name = BotNameAccessor.GetBotName(BotType.LEVEL1) });
            data.Add(new ComboData { Id = BotType.LEVEL2, Name = BotNameAccessor.GetBotName(BotType.LEVEL2) });
            data.Add(new ComboData { Id = BotType.LEVEL3, Name = BotNameAccessor.GetBotName(BotType.LEVEL3) });

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

        private void tournamentFinderUC_Unloaded(object sender, RoutedEventArgs e)
        {
            requestTimer.Dispose();
        }
    }

    public class StatusToColorConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            VisualizableTorunamentSettings vts = value as VisualizableTorunamentSettings;

            if (vts.Settings.Running || vts.Settings.PlayedMatches > 0)
                return Brushes.Orange;
            else
                return Brushes.Green;
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            return value;
        }
    }
}
