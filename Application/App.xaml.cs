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
            sceneMgr = new SceneMgr();

            PlayerName = "Player";

            if (!File.Exists("player"))
            {
                PlayerHashId = Player.GenerateNewHashId(PlayerName);
                using (StreamWriter writer = new StreamWriter("player", false))
                {
                    writer.WriteLine("name=" + PlayerName);
                    writer.WriteLine("hash=" + PlayerHashId);
                }
                return;
            }

            // TODO: je mozne aby hrac upravil soubor tak, ze nebude obsahovat hash, to by pak byla celkem chyba
            using (StreamReader reader = new StreamReader("player"))
            {
                string line;
                string[] items;
                while ((line = reader.ReadLine()) != null)
                {
                    items = line.Split('=');
                    if (items[0].Equals("name"))
                        PlayerName = items[1];
                    else if (items[0].Equals("hash"))
                        PlayerHashId = items[1];
                }
            }
        }

        protected override void OnExit(ExitEventArgs e)
        {
            ExitGame();
        }

        public void StartSoloGame()
        {
            StartLocalServer(Gametype.SOLO_GAME);

            sceneMgr.SetRemoteServerAddress("127.0.0.1");

            StartGame(Gametype.SOLO_GAME);
        }

        private void StartLocalServer(Gametype type)
        {
            server = new ServerMgr();
            server.Init(type);

            Thread serverThread = new Thread(new ThreadStart(server.Run));
            serverThread.Name = "Server Thread";
            serverThread.Start();
        }

        private void StartGame(Gametype type)
        {
            StartGameThread();

            lastGameType = type;

            sceneMgr.Enqueue(new Action(() =>
            {
                sceneMgr.Init(type);
            }));
        }

        public void StartHostedGame()
        {
            StartLocalServer(Gametype.SERVER_GAME);

            sceneMgr.SetRemoteServerAddress("127.0.0.1");

            StartGame(Gametype.SERVER_GAME);
        }

        public void StartTournamentLobby()
        {
            StartLocalServer(Gametype.TOURNAMENT_GAME);

            sceneMgr.SetRemoteServerAddress("127.0.0.1");

            StartGame(Gametype.TOURNAMENT_GAME);
        }

        public void CreateGameGui(bool setCanvas = true)
        {
            mainWindow.mainGrid.Children.Clear();
            GameUC gameW = new GameUC();
            mainWindow.mainGrid.Children.Add(gameW);
            if (setCanvas)
            {
                Size canvasSize = new Size(gameW.mainCanvas.Width, gameW.mainCanvas.Height);
                sceneMgr.Enqueue(new Action(() =>
                {
                    sceneMgr.SetCanvas(gameW.mainCanvas, canvasSize);
                }));
            }
        }

        public Canvas GetCanvas()
        {
            return LogicalTreeHelper.FindLogicalNode(MainWindow, "mainCanvas") as Canvas;
        }

        public void CreateLobbyGui(bool asLeader)
        {
            mainWindow.mainGrid.Children.Clear();
            sceneMgr.Enqueue(new Action(() =>
            {
                sceneMgr.GetCurrentPlayer().Data.LobbyLeader = asLeader;
            }));
            mainWindow.mainGrid.Children.Add(new LobbyUC(asLeader));
        }

        public void ConnectToGame(string serverAddress)
        {
            lastServerAddress = serverAddress;
            sceneMgr.SetRemoteServerAddress(serverAddress);
            StartGame(Gametype.CLIENT_GAME);
        }

        private void StartGameThread()
        {
            mainWindow.gameRunning = true;
            Thread gameThread = new Thread(new ThreadStart(sceneMgr.Run));
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
            mainWindow.mainGrid.Children.Clear();
            mainWindow.mainGrid.Children.Add(new MainUC());
        }

        public void ExitGame()
        {
            if (sceneMgr != null)
                sceneMgr.Enqueue(new Action(() =>
                {
                    sceneMgr.CloseGame();
                }));

            if (server != null)
            {
                server.Enqueue(new Action(() =>
                {
                    server.Shutdown();
                    server = null;
                }));
            }
            mainWindow.gameRunning = false;
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
                    StartSoloGame();
                    break;
                case Gametype.SERVER_GAME:
                    CreateGameGui();
                    StartHostedGame();
                    break;
                case Gametype.CLIENT_GAME:
                    CreateGameGui();
                    ConnectToGame(lastServerAddress);
                    break;
                case Gametype.TOURNAMENT_GAME:
                    StartTournamentLobby();
                    CreateLobbyGui(true);
                    break;
                case Gametype.NONE:
                default:
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
    }
}
