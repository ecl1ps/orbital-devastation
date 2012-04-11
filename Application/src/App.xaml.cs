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

namespace Orbit
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        private SceneMgr sceneMgr;

        [STAThread]
        public static void Main(string[] args)
        {
            App app = new App();
            app.InitializeComponent();
            app.Run(new GameWindow());
        }

        protected override void OnStartup(StartupEventArgs e)
        {
            //MainWindow.Content = new GameUC();
            StartHostedGame();
        }

        protected override void OnExit(ExitEventArgs e)
        {
            if (sceneMgr != null)
                sceneMgr.RequestStop();
        }

        private void InitScene()
        {
            MainWindow.Content = new GameUC();
            sceneMgr = SceneMgr.GetInstance();
            sceneMgr.SetCanvas((Canvas)LogicalTreeHelper.FindLogicalNode(MainWindow, "mainCanvas"));
            sceneMgr.Init();

            StartGameThread();
        }

        public void StartHostedGame()
        {
            InitScene();
            sceneMgr.SetIsServer(true);
        }

        public void ConnectToGame()
        {
            throw new Exception("Not implemented");
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
            sceneMgr.Init();
            StartGameThread();
        }
    }
}
