using System;
using Orbit.Player;
using Orbit.Scene.Controls;
using Orbit.Scene.Entities;
using System.Collections.Generic;

namespace Orbit.Scene
{
    public class SceneMgr
    {
        private bool isServer;
        //private NetServer server;
        //private NetClient client;
        private IList<ISceneObject> objects;
        private Dictionary<PlayerPosition, IPlayerData> playerData;
        private PlayerPosition me;
        //private int isStarted;
        //http://msdn.microsoft.com/en-us/library/system.threading.interlocked.aspx

        public SceneMgr(bool isServer)
        {
            throw new Exception("Not implemented");
        }

        public void Init()
        {
            throw new Exception("Not implemented");
        }

        public void Update()
        {
            throw new Exception("Not implemented");
        }

        public void UpdateSceneObjects(float tpf)
        {
            throw new Exception("Not implemented");
        }

        public void CheckCollisions()
        {
            throw new Exception("Not implemented");
        }

        public void RenderScene()
        {
            throw new Exception("Not implemented");
        }

        public IPlayerData GetPlayerData(PlayerPosition pos)
        {
            throw new Exception("Not implemented");
        }

        public void ProcessUserInput()
        {
            throw new Exception("Not implemented");
        }

        public void ProcessMessages()
        {
            throw new Exception("Not implemented");
        }

    }

}
