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

namespace Orbit
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        private SceneMgr sceneMgr;
        private static GameWindow mainWindow;
        private string lastServerAddress;

        [STAThread]
        public static void Main(string[] args)
        {
            App app = new App();
            app.InitializeComponent();
            app.lastServerAddress = "127.0.0.1";
            mainWindow = new GameWindow();
            app.Run(mainWindow);
        }

        protected override void OnStartup(StartupEventArgs e)
        {
            sceneMgr = SceneMgr.GetInstance();
        }

        protected override void OnExit(ExitEventArgs e)
        {
            ExitGame();
        }

        public void StartGame(Gametype type)
        {
            mainWindow.mainGrid.Children.Clear();
            mainWindow.mainGrid.Children.Add(new GameUC());
            sceneMgr.SetCanvas((Canvas)LogicalTreeHelper.FindLogicalNode(MainWindow, "mainCanvas"));
            sceneMgr.Init(type);

            StartGameThread();
        }

        public void StartHostedGame()
        {
            sceneMgr.SetIsServer(true);
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
            gameThread.Start();
        }

        public SceneMgr GetSceneMgr()
        {
            return sceneMgr;
        }

        public void GameEnded()
        {
            ShowStartScreen();
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
        }

        public void LookForGame()
        {
            mainWindow.mainGrid.Children.Clear();
            FindServerUC f = new FindServerUC();
            f.LastAddress = lastServerAddress;
            mainWindow.mainGrid.Children.Add(f);
        }
    }
}
