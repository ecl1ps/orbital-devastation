using Lidgren.Network;
using Orbit.Core.Scene.Entities;
using Orbit.Gui;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Windows;
using System.Windows.Media;

namespace Orbit.Core.Client.Cache
{
    public struct CachedObject
    {
        public bool IsPermanent { get; set; }
        public Object Cache { get; set; }
    }

    public abstract class CacheManager : IGameState
    {
        public bool Loaded { get { return loaded; } }
        private bool loaded = false;
        private int step = 0;
        private int total = 0;
        private float waiting = 0;
        private bool loadingPerm = true;
        private LoadingScreen loadingScreen;

        private Dictionary<String, CachedObject> map = new Dictionary<string, CachedObject>();

        public SceneMgr SceneMgr { get; set; }

        private List<MethodInfo> permM = new List<MethodInfo>();
        private List<MethodInfo> tempM = new List<MethodInfo>();

        public CacheManager(SceneMgr mgr, LoadingScreen loadingScreen)
        {
            SceneMgr = mgr;
            this.loadingScreen = loadingScreen;
            Prepare();
        }

        private void Prepare()
        {
            MethodInfo[] methods =  GetType().GetMethods(BindingFlags.Instance | BindingFlags.Public);

            foreach (MethodInfo info in methods)
            {
                if (info.Name.StartsWith("RegisterTemp"))
                    tempM.Add(info);
                else if (info.Name.StartsWith("RegisterPerm"))
                    permM.Add(info);
            }

            total = permM.Count + tempM.Count;
            step = 0;
        }

        public void Clear()
        {
            foreach (KeyValuePair<String,  CachedObject> pair in map)
            {
                if (!pair.Value.IsPermanent)
                    map.Remove(pair.Key);
            }

            loaded = false;
            //kdyz uz je permanentni nacitani complete
            if(!loadingPerm)
                total = tempM.Count;

        }

        public void Register(Object geometry, bool permanent, Type type, int id = 0)
        {
            Register(geometry, permanent, type.Name + id);
        }

        public void Register(Object geometry, bool permanent, String s)
        {
            if (map.ContainsKey(s))
                throw new Exception("Registering with duplicate key " + s);

            CachedObject obj = new CachedObject();
            obj.IsPermanent = permanent;
            obj.Cache = geometry;

            map.Add(s, obj);
        }

        public Object Get(Type type, int id = 0) 
        {
            return Get(type.Name + id);
        }

        public Object Get(String s) 
        {
            CachedObject obj;
            if (map.TryGetValue(s, out obj))
            {
                if (obj.Cache is ICloneable)
                    return (obj.Cache as ICloneable).Clone();
                else if (obj.Cache is Freezable)
                    return (obj.Cache as Freezable).Clone();
                else
                    throw new Exception("Trying to cache uncloneable class");
            }
            else
                throw new Exception("Trying to get undefined cache: " + s); 
        }

        public void Update(float tpf)
        {
            if (loaded)
            {
                return;
            }

            if (waiting <= 0)
            {
                if (loadingPerm)
                {
                    if (step < permM.Count)
                        permM[step].Invoke(this, new object[] { });
                    else if (step < permM.Count + tempM.Count)
                        tempM[step - permM.Count].Invoke(this, new object[] { });
                }
                else
                    tempM[step].Invoke(this, new object[] { });

                step++;
                UpdateProgress(step / total);
            }
            else
            {
                waiting -= tpf;
            }
        }

        public bool IsLoadingCompleted()
        {
            return loaded;
        }

        protected void LoadingCompleted()
        {
            loaded = true;
            loadingPerm = false;

            SendLoadingCompletedMsg();
        }

        protected void SendLoadingProgressMsg(float progress)
        {
            NetOutgoingMessage msg = SceneMgr.CreateNetMessage();
            msg.Write((int)PacketType.PLAYER_LOAD_PROGRESS);
            msg.Write(SceneMgr.GetCurrentPlayer().GetId());
            msg.Write(progress);

            SceneMgr.SendMessage(msg);
        }

        protected void SendLoadingCompletedMsg()
        {
            NetOutgoingMessage msg = SceneMgr.CreateNetMessage();
            msg.Write((int)PacketType.PLAYER_LOADING_COMPLETED);
            msg.Write(SceneMgr.GetCurrentPlayer().GetId());

            SceneMgr.SendMessage(msg);
        }

        protected void UpdateProgress(float percentage)
        {
            loadingScreen.UpdatePlayer(SceneMgr.GetCurrentPlayer(), percentage);
            SendLoadingProgressMsg(percentage);

            if (percentage >= 1)
                LoadingCompleted();
        }


    }
}
