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
            StartHostedGame();
            StartGameThread();
        }

        protected override void OnExit(ExitEventArgs e)
        {
            sceneMgr.RequestStop();
        }

        public void InitScene()
        {
            GameWindow wnd = MainWindow as GameWindow;
            sceneMgr = SceneMgr.GetInstance();
            sceneMgr.SetCanvas(wnd.mainCanvas, wnd.actionArea);
            sceneMgr.Init();
        }

        public void StartHostedGame()
        {
            InitScene();
            sceneMgr.SetAsServer(true);
        }

        public void ConnectToGame()
        {
            throw new Exception("Not implemented");
        }

        public void StartGameThread()
        {
            Thread gameThread = new Thread(new ThreadStart(sceneMgr.Run));
            gameThread.Start();
        }

        public SceneMgr GetSceneMgr()
        {
            return sceneMgr;
        }
    }
}
