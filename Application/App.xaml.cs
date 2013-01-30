﻿using System;
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
        private ServerMgr server;
        private static GameWindow mainWindow;
        private string lastServerAddress;
        private Gametype lastGameType;
        private bool hostedLastgame = false;
        private TournamentSettings lastSoloTournamentSettings;

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
            SetLocalization("en");

            Thread.CurrentThread.CurrentCulture = CultureInfo.GetCultureInfo("en");

            App app = new App();
            app.InitializeComponent();
            app.lastServerAddress = "127.0.0.1";
            app.lastGameType = Gametype.NONE;
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

        public static void SetLocalization(string locale)
        {
            WPFLocalizeExtension.Engine.LocalizeDictionary.Instance.Culture = CultureInfo.GetCultureInfo(locale);
            Strings.Culture = CultureInfo.GetCultureInfo(locale);
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

            PlayerName = GameProperties.Props.Get(PropertyKey.PLAYER_NAME);
            PlayerHashId = GameProperties.Props.Get(PropertyKey.PLAYER_HASH_ID);
            Boolean.TryParse(GameProperties.Props.Get(PropertyKey.STATIC_MOUSE_ENABLED), out StaticMouse.ALLOWED);

            sceneMgr = new SceneMgr();
            SoundManager.Instance.StartPlayingInfinite(SharedDef.MUSIC_BACKGROUND_CALM);
        }

        protected override void OnExit(ExitEventArgs e)
        {
            ShutdownServerIfExists();
        }

        public void StartSoloGame(TournamentSettings s)
        {
            if (!StartLocalServer(Gametype.SOLO_GAME))
                return;

            sceneMgr.SetRemoteServerAddress("127.0.0.1");

            StartGame(Gametype.SOLO_GAME);

            SendTournamentSettings(s);
            lastSoloTournamentSettings = s;
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
            server = new ServerMgr();
            try
            {
                server.Init(type);
            }
            catch (SocketException)
            {
                ShowStartScreen();
                AddMenu(new InfoUC("Your port is already in use. Server is probably already running on your machine."));
                return false;
            }

            Thread serverThread = new Thread(new ThreadStart(server.Run));
            serverThread.IsBackground = false;
            serverThread.Name = "Server Thread";
            serverThread.CurrentCulture = Thread.CurrentThread.CurrentCulture;
            serverThread.Start();
            return true;
        }

        private void StartGame(Gametype type)
        {
            SoundManager.Instance.StopAllSounds();
            SoundManager.Instance.StartPlayingInfinite(SharedDef.MUSIC_BACKGROUND_ACTION);

            StartGameThread();

            lastGameType = type;

            if (type != Gametype.MULTIPLAYER_GAME)
                mainWindow.GameRunning = true;

            sceneMgr.Enqueue(new Action(() =>
            {
                sceneMgr.Init(type);
            }));
        }

        public void StartHostedGame()
        {
            if (!StartLocalServer(Gametype.MULTIPLAYER_GAME))
                return;

            hostedLastgame = true;

            sceneMgr.SetRemoteServerAddress("127.0.0.1");

            StartGame(Gametype.MULTIPLAYER_GAME);

            TournamentSettings s = new TournamentSettings();
            s.MMType = MatchManagerType.QUICK_GAME;
            s.Level = GameLevel.BASIC_MAP;
            s.RoundCount = 1;
            s.BotCount = 0;

            SendTournamentSettings(s);
        }

        public void StartTournamentLobby()
        {
            if (!StartLocalServer(Gametype.TOURNAMENT_GAME))
                return;

            sceneMgr.SetRemoteServerAddress("127.0.0.1");

            StartGame(Gametype.TOURNAMENT_GAME);

            CreateLobbyGui(true);
        }

        public void SetGameStarted(bool started)
        {
            mainWindow.GameRunning = started;
        }

        public void CreateGameGui(bool setGameArea = true)
        {
            GameUC gameW = new GameUC();
            AddWindow(gameW);
            sceneMgr.GameWindowState = Orbit.Core.WindowState.IN_GAME;
            if (setGameArea)
            {
                GameVisualArea gva = GetGameArea();
                sceneMgr.Enqueue(new Action(() =>
                {
                    sceneMgr.SetGameVisualArea(gva);
                }));
            }
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

        public void CreateLobbyGui(bool asLeader, bool settingsLocked = false)
        {
            sceneMgr.Enqueue(new Action(() =>
            {
                sceneMgr.GetCurrentPlayer().Data.LobbyLeader = asLeader;
            }));
            AddWindow(new LobbyUC(asLeader, settingsLocked));
            sceneMgr.GameWindowState = Orbit.Core.WindowState.IN_LOBBY;
        }

        public void ConnectToGame(string serverAddress)
        {
            hostedLastgame = false;
            lastServerAddress = serverAddress;
            sceneMgr.SetRemoteServerAddress(serverAddress);
            StartGame(Gametype.MULTIPLAYER_GAME);
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
            StaticMouse.Enable(false);
            ShowStartScreen();
            if (lastGameType != Gametype.NONE)
            {
                Button btnRepeatGame = LogicalTreeHelper.FindLogicalNode(mainWindow.mainGrid, "btnRepeatGame") as Button;
                if (btnRepeatGame != null)
                    btnRepeatGame.Visibility = Visibility.Visible;
            }
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
            if (server != null)
            {
                server.Enqueue(new Action(() =>
                {
                    if (server != null)
                    {
                        server.Shutdown();
                        server = null;
                    }
                }));
            }
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

        public void LookForGame()
        {
            FindServerUC f = new FindServerUC();
            f.LastAddress = lastServerAddress;
            AddWindow(f);
        }

        public void RepeatGame()
        {
            switch (lastGameType)
            {
                case Gametype.SOLO_GAME:
                    CreateGameGui();
                    StartSoloGame(lastSoloTournamentSettings);
                    break;
                case Gametype.MULTIPLAYER_GAME:
                    CreateGameGui();
                    if (hostedLastgame)
                        StartHostedGame();
                    else
                        ConnectToGame(lastServerAddress);
                    break;
                case Gametype.TOURNAMENT_GAME:
                    StartTournamentLobby();
                    break;
                case Gametype.NONE:
                default:
                    CreateGameGui();
                    StartSoloGame(lastSoloTournamentSettings);
                    break;
            }
        }

        public void PlayerReady(bool ready)
        {
            sceneMgr.Enqueue(new Action(() =>
            {
                sceneMgr.GetCurrentPlayer().Data.LobbyReady = ready;
                sceneMgr.SendChatMessage(ready ? "I am ready" : "Wait, I am not ready");
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

        public void CreateScoreboardGui(LobbyPlayerData winnerData, List<LobbyPlayerData> data)
        {
            AddWindow(new ScoreboardUC(winnerData, data));
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
