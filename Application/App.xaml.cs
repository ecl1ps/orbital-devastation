using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Windows;
using System.Windows.Navigation;
using Orbit.Core.Scene;
using Orbit.Gui;
using System.Threading;
using System.ComponentModel;
using System.Windows.Controls;
using Orbit.Core;
using Orbit.Core.Client;
using Orbit.Core.Server;
using System.IO;
using Orbit.Core.Players;
using Orbit.Core.Client.GameStates;
using System.Net.Sockets;
using Orbit.Core.Server.Match;
using Orbit.Core.Server.Level;
using Orbit.Gui.Visuals;
using System.Reflection;
using System.Globalization;

namespace Orbit
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        private static readonly log4net.ILog Logger = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        private SceneMgr sceneMgr;
        private MasterServerMgr masterServer;
        private XNAControl.XNAControl gameControl;
        private static GameWindow mainWindow;

        public string PlayerName { get; set; }
        public string PlayerHashId { get; set; }
        public static App Instance
        {
            get
            {
                return Application.Current as App;
            }
        }
        public static GameWindow WindowInstance
        {
            get
            {
                return Application.Current.MainWindow as GameWindow;
            }
        }

        [STAThread]
        public static void Main(string[] args)
        {
            App app = new App();
            app.InitializeComponent();
            CommandExecutor.Instance.ParseCommands(args);
            mainWindow = new GameWindow();
#if !DEBUG
            try {
#endif
            app.Run(mainWindow);
#if !DEBUG
            } 
            catch (Exception e)
            {
                Logger.Fatal(e);
            }
#endif
        }

        public static void SetLocalization(CultureInfo locale)
        {
            WPFLocalizeExtension.Engine.LocalizeDictionary.Instance.Culture = locale;
            Strings.Culture = locale;
            Thread.CurrentThread.CurrentCulture = locale;
        }

        public static void SetLocalization(string locale)
        {
            SetLocalization(CultureInfo.GetCultureInfo(locale));
        }

        protected override void OnStartup(StartupEventArgs e)
        {
#if DEBUG
            log4net.Config.XmlConfigurator.Configure(Assembly.GetExecutingAssembly().GetManifestResourceStream("Orbit.logger.config"));
#else
            log4net.Config.XmlConfigurator.Configure(Assembly.GetExecutingAssembly().GetManifestResourceStream("Orbit.logger.config"));
#endif

            if (!File.Exists(SharedDef.CONFIG_FILE))
                GameProperties.Props.Save();

            WPFLocalizeExtension.Engine.LocalizeDictionary.Instance.MergedAvailableCultures.Clear();
            WPFLocalizeExtension.Engine.LocalizeDictionary.Instance.MergedAvailableCultures.Add(CultureInfo.GetCultureInfo("en"));
            WPFLocalizeExtension.Engine.LocalizeDictionary.Instance.MergedAvailableCultures.Add(CultureInfo.GetCultureInfo("cs"));
            SetLocalization(GameProperties.Get(PropertyKey.GAME_LANGUAGE));

            PlayerName = GameProperties.Props.Get(PropertyKey.PLAYER_NAME);
            PlayerHashId = GameProperties.Props.Get(PropertyKey.PLAYER_HASH_ID);
            if (!Boolean.TryParse(GameProperties.Props.Get(PropertyKey.STATIC_MOUSE_ENABLED), out StaticMouse.ALLOWED))
                StaticMouse.ALLOWED = false;

            gameControl = new XNAControl.XNAControl();
            gameControl.Width = SharedDef.VIEW_PORT_SIZE.Width;
            gameControl.Height = SharedDef.VIEW_PORT_SIZE.Height;

            sceneMgr = new SceneMgr(gameControl.Handle);

            SoundManager.Instance.StartPlayingInfinite(SharedDef.MUSIC_BACKGROUND_CALM);

            CommandExecutor.Instance.ExecuteCommands();
        }

        protected override void OnExit(ExitEventArgs e)
        {
            ShutdownServerIfExists();
        }

        public void StartSoloGame(TournamentSettings s)
        {
            /*if (!StartLocalServer(Gametype.SOLO_GAME))
                return;

            sceneMgr.SetRemoteServerAddress(System.Net.IPAddress.Loopback.ToString());

            StartGame(Gametype.SOLO_GAME);

            SendTournamentSettings(s);*/
        }

        private void SendTournamentSettings(TournamentSettings s)
        {
            sceneMgr.Enqueue(new Action(() =>
            {
                sceneMgr.SendNewTournamentSettings(s);
            }));
        }

        private bool StartLocalServer(Gametype type)
        {
            try
            {
                if (masterServer == null)
                    masterServer = new MasterServerMgr();
            }
            catch (SocketException)
            {
                ShowStartScreen();
                AddMenu(new InfoUC(Strings.ui_warning_port_unavailable));
                return false;
            }

            return true;
        }

        private void StartGame(Gametype type, int serverId = 0)
        {
            SoundManager.Instance.StopAllSounds();
            SoundManager.Instance.StartPlayingInfinite(SharedDef.MUSIC_BACKGROUND_ACTION);

            StartGameThread();

            if (type != Gametype.MULTIPLAYER_GAME)
                mainWindow.GameRunning = true;

            sceneMgr.Enqueue(new Action(() =>
            {
                sceneMgr.RemoteServerId = serverId;
                sceneMgr.Init(type);
            }));
        }

        public void StartQuickGame(string address = SharedDef.MASTER_SERVER_ADDRESS)
        {
            sceneMgr.SetRemoteServerAddress(address);
            StartGame(Gametype.MULTIPLAYER_GAME);
        }

        public void CreateGameGui(bool setGameArea = true)
        {
            GameUC gameW = new GameUC(gameControl);
            AddWindow(gameW);
            /*sceneMgr.GameWindowState = Orbit.Core.WindowState.IN_GAME;
            if (setGameArea)
            {
                GameVisualArea gva = GetGameArea();
                sceneMgr.Enqueue(new Action(() =>
                {
                    sceneMgr.SetGameVisualArea(gva);
                }));
            }*/
        }

        public void StartTournamentFinder(string address = SharedDef.MASTER_SERVER_ADDRESS)
        {
            AddWindow(new TournamentFinderUC(address));
        }

        /************************************************************************/
        /* ostatni pripojujici se hraci                                                                     */
        /************************************************************************/
        public void StartTournamentGame(string address, int serverId)
        {
            sceneMgr.SetRemoteServerAddress(address);

            StartGame(Gametype.TOURNAMENT_GAME, serverId);
        }

        /************************************************************************/
        /* leader                                                                     */
        /************************************************************************/
        public void StartTournamentLobby(string address)
        {
            sceneMgr.SetRemoteServerAddress(address);

            StartGame(Gametype.TOURNAMENT_GAME, 0);

            CreateLobbyGui(true, true);
        }

        public void CreateLobbyGui(bool asLeader, bool firstGame = false)
        {
            sceneMgr.Enqueue(new Action(() =>
            {
                sceneMgr.GetCurrentPlayer().Data.LobbyLeader = asLeader;
            }));
            AddWindow(new LobbyUC(asLeader, firstGame));
            sceneMgr.GameWindowState = Orbit.Core.WindowState.IN_LOBBY;
        }

        public void SetGameStarted(bool started)
        {
            mainWindow.GameRunning = started;
        }

        public void FocusWindow()
        {
            MainWindow.WindowState = System.Windows.WindowState.Normal;
            MainWindow.Activate();
            MainWindow.Focus();
            MainWindow.BringIntoView();
        }

        public GameVisualArea GetGameArea()
        {
            return LogicalTreeHelper.FindLogicalNode(mainWindow.mainGrid, "gameArea") as GameVisualArea;
        }

        private void StartGameThread()
        {
            Thread gameThread = new Thread(new ThreadStart(sceneMgr.Run));
            gameThread.IsBackground = false;
            gameThread.Name = "Game Thread";
            gameThread.CurrentCulture = Thread.CurrentThread.CurrentCulture;
            gameThread.Start();
        }

        public SceneMgr GetSceneMgr()
        {
            return sceneMgr;
        }

        public void GameEnded()
        {
            ShutdownServerIfExists();
            StaticMouse.Enable(false);
            ShowStartScreen();
        }

        public void ShowStartScreen()
        {
            SoundManager.Instance.StopAllSounds();
            SoundManager.Instance.StartPlayingInfinite(SharedDef.MUSIC_BACKGROUND_CALM);

            AddWindow(new MainUC());
            sceneMgr.GameWindowState = Orbit.Core.WindowState.IN_MAIN_MENU;
        }

        public void ShutdownServerIfExists()
        {
            if (masterServer != null)
                masterServer.Shutdown();
            masterServer = null;
            mainWindow.GameRunning = false;
        }

        public void ShutdownSceneMgr()
        {
            if (sceneMgr != null)
            {
                sceneMgr.Enqueue(new Action(() =>
                {
                    if (sceneMgr != null)
                    {
                        sceneMgr.CloseGame();
                        sceneMgr = null;
                    }
                }));
            }
        }

        public void PlayerReady(bool ready)
        {
            sceneMgr.Enqueue(new Action(() =>
            {
                sceneMgr.GetCurrentPlayer().Data.LobbyReady = ready;
                sceneMgr.SendChatMessage(ready ? Strings.lobby_ready_msg : Strings.lobby_not_ready_msg);
                sceneMgr.SendPlayerReadyMessage(ready);
            }));
        }

        public void StartTournamentGame()
        {
            sceneMgr.Enqueue(new Action(() =>
            {
                sceneMgr.SendStartGameRequest();
            }));
        }

        public void SendChatMessage(string msg)
        {
            sceneMgr.Enqueue(new Action(() =>
            {
                sceneMgr.SendChatMessage(msg);
            }));
        }

        public void CreateScoreboardGui(List<LobbyPlayerData> data)
        {
            AddWindow(new ScoreboardUC(data));
        }

        public void ShowStatisticsGui()
        {
            AddWindow(new StatisticsUC());
        }

        public void ShowBotSelectionGui()
        {
            AddMenu(new BotSelection());
        }

        public void OnKeyEvent(System.Windows.Input.KeyEventArgs e)
        {
            sceneMgr.Enqueue(new Action(() =>
            {
                sceneMgr.OnKeyEvent(e);
            }));
        }

        public void ShowGameOverview(List<PlayerOverviewData> data)
        {
            AddMenu(new GameOverviewUC(data));
        }

        public bool IsGameStarted() 
        { 
            return mainWindow.GameRunning; 
        }

        public void ClearMenus()
        {
            mainWindow.ClearMenus();
        }

        public void AddMenu(UserControl menu, bool removeOldMenus = true)
        {
            mainWindow.AddMenu(menu, removeOldMenus);
        }

        public void ClearWindows()
        {
            mainWindow.mainGrid.Children.Clear();
        }

        public void AddWindow(UserControl window)
        {
            ClearMenus();
            ClearWindows();
            mainWindow.mainGrid.Children.Add(window);
        }
    }
}
