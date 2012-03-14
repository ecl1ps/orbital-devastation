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
            this.isServer = isServer;
            objects = new List<ISceneObject>();
            Random r = new Random();
            me = r.Next(2) == 0 ? PlayerPosition.LEFT : PlayerPosition.RIGHT;
            playerData = new Dictionary<PlayerPosition, IPlayerData>(2);
            //playerData.Add(me, );
        }

        public void Init()
        {
            objects.Add(new Base());
            objects.Add(new Base());

            for (int i = 0; i < 10; ++i)
            {
                objects.Add(new Sphere());
            }
        }

        public void Update(float tpf)
        {
            UpdateSceneObjects(tpf);
            CheckCollisions();
            RenderScene();
        }

        public void UpdateSceneObjects(float tpf)
        {
            foreach (ISceneObject obj in objects)
            {
                obj.Update(tpf);
                if (!obj.IsOnScreen())
                    objects.Remove(obj);
            }
        }

        public void CheckCollisions()
        {
            throw new Exception("Not implemented");
        }

        public void RenderScene()
        {
            foreach (ISceneObject obj in objects)
            {
                obj.Render();
            }
        }

        public IPlayerData GetPlayerData(PlayerPosition pos)
        {
            IPlayerData data;
            playerData.TryGetValue(pos, out data);
            return data;
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
