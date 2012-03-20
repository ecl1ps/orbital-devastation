using System;
using Orbit.Scene;

namespace Orbit
{
    class Application
    {
        private SceneMgr sceneMgr;

        static void Main(string[] args)
        {
            Application app = new Application();
            //app.InitGui();
            app.StartHostedGame();

        }

        public void InitGui()
        {
            throw new Exception("Not implemented");
        }

        public void InitScene()
        {
            sceneMgr = new SceneMgr(true);
            sceneMgr.Init();
        }

        public void StartHostedGame()
        {
            InitScene();
        }

        public void ConnectToGame()
        {
            throw new Exception("Not implemented");
        }

        public void StartGameThread()
        {
            throw new Exception("Not implemented");
        }
    }
}
