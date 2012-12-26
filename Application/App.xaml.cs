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

#if DEBUG
[assembly: log4net.Config.XmlConfigurator(ConfigFile = "logger.config")]
#else
[assembly: log4net.Config.XmlConfigurator(ConfigFile = "logger.config")]
#endif

namespace Orbit
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        private SceneMgr sceneMgr;
        private ServerMgr server;
        private static GameWindow mainWindow;
        private string lastServerAddress;
        private Gametype lastGameType;
        private bool hostedLastgame = false;
        private TournamentSettings lastSoloTournamentSettings;

        public string PlayerName { get; set; }
        public string PlayerHashId { get; set; }

        [STAThread]
        public static void Main(string[] args)
        {
            App app = new App();
            app.InitializeComponent();
            app.lastServerAddress = "127.0.0.1";
            app.lastGameType = Gametype.NONE;
            mainWindow = new GameWindow();
            app.Run(mainWindow);
        }

        protected override void OnStartup(StartupEventArgs e)
        {
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
                return false;
            }

            Thread serverThread = new Thread(new ThreadStart(server.Run));
            serverThread.IsBackground = false;
            serverThread.Name = "Server Thread";
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

        public void CreateGameGui(bool setCanvas = true)
        {
            mainWindow.mainGrid.Children.Clear();
            GameUC gameW = new GameUC();
            mainWindow.mainGrid.Children.Add(gameW);
            sceneMgr.GameWindowState = Orbit.Core.WindowState.IN_GAME;
            if (setCanvas)
                sceneMgr.Enqueue(new Action(() =>
                {
                    sceneMgr.SetCanvas(gameW.mainCanvas);
                }));
        }

        public Canvas GetCanvas()
        {
            return LogicalTreeHelper.FindLogicalNode(MainWindow, "mainCanvas") as Canvas;
        }

        public void CreateLobbyGui(bool asLeader, bool settingsLocked = false)
        {
            mainWindow.mainGrid.Children.Clear();
            sceneMgr.Enqueue(new Action(() =>
            {
                sceneMgr.GetCurrentPlayer().Data.LobbyLeader = asLeader;
            }));
            mainWindow.mainGrid.Children.Add(new LobbyUC(asLeader, settingsLocked));
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

            mainWindow.mainGrid.Children.Clear();
            mainWindow.mainGrid.Children.Add(new MainUC());
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
            mainWindow.mainGrid.Children.Clear();
            FindServerUC f = new FindServerUC();
            f.LastAddress = lastServerAddress;
            mainWindow.mainGrid.Children.Add(f);
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

        public void PlayerReady()
        {
            sceneMgr.Enqueue(new Action(() =>
            {
                sceneMgr.GetCurrentPlayer().Data.LobbyReady = true;
                sceneMgr.SendChatMessage("I am ready");
                sceneMgr.SendPlayerReadyMessage();
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
            mainWindow.mainGrid.Children.Clear();
            mainWindow.mainGrid.Children.Add(new ScoreboardUC(winnerData, data));
        }

        public void ShowStatisticsGui()
        {
            mainWindow.mainGrid.Children.Clear();
            mainWindow.mainGrid.Children.Add(new StatisticsUC());
        }

        public void ShowBotSelectionGui()
        {
            UIElement uc = LogicalTreeHelper.FindLogicalNode(mainWindow.mainGrid, "botSelection") as UIElement;
            if (uc != null)
                mainWindow.mainGrid.Children.Remove(uc);
            else
                mainWindow.mainGrid.Children.Add(new BotSelection());
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
            mainWindow.mainGrid.Children.Add(new GameOverviewUC(data));
        }

        public bool IsGameStarted() 
        { 
            return mainWindow.GameRunning; 
        }
    }
}
