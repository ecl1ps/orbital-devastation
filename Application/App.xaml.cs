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

            // TODO: ukladat jmeno pomoci esc -> options -> neco
            PlayerName = "Player1";
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
            lastGameType = type;
            mainWindow.mainGrid.Children.Clear();
            mainWindow.mainGrid.Children.Add(new GameUC());
            sceneMgr.SetCanvas((Canvas)LogicalTreeHelper.FindLogicalNode(MainWindow, "mainCanvas"));
            sceneMgr.Init(type);

            StartGameThread();
        }

        public void StartHostedGame()
        {
            StartLocalServer(Gametype.SERVER_GAME);

            sceneMgr.SetRemoteServerAddress("127.0.0.1");

            StartGame(Gametype.SERVER_GAME);
        }

        public void ConnectToGame(string serverAddress)
        {
            lastServerAddress = serverAddress;
            sceneMgr.SetRemoteServerAddress(serverAddress);
            StartGame(Gametype.CLIENT_GAME);
        }

        private void StartGameThread()
        {
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
            ExitGame();
            mainWindow.mainGrid.Children.Clear();
            mainWindow.mainGrid.Children.Add(new OptionsUC());
        }

        public void ExitGame()
        {
            if (sceneMgr != null)
                sceneMgr.CloseGame();

            if (server != null)
            {
                server.Shutdown();
                server = null;
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
                    StartSoloGame();
                    break;
                case Gametype.SERVER_GAME:
                    StartHostedGame();
                    break;
                case Gametype.CLIENT_GAME:
                    ConnectToGame(lastServerAddress);
                    break;
                case Gametype.NONE:
                default:
                    break;
            }
        }

        public string GetPlayerName()
        {
            return PlayerName;
        }
    }
}
