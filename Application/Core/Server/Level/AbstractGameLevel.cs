using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Orbit.Core.Scene.Entities;

namespace Orbit.Core.Server.Level
{
    public abstract class AbstractGameLevel : IGameLevel
    {
        protected ServerMgr mgr;
        protected List<ISceneObject> objects;
        protected EventProcessor events;

        public AbstractGameLevel(ServerMgr serverMgr)
        {
            mgr = serverMgr;
            objects = new List<ISceneObject>();
            events = new EventProcessor();
            CreateLevelObjects();
        }

        public virtual void Update(float tpf)
        {
            events.Update(tpf);
        }

        protected virtual void CreateLevelObjects()
        {
        }

        public virtual void OnStart()
        {
        }

        public virtual void CreateBots(List<Players.Player> players, int suggestedCount, Players.BotType type)
        {
        }

        public void ObjectDestroyed(long id)
        {
            ISceneObject obj = objects.Find(o => o.Id == id);
            if (obj == null)
                return;

            objects.Remove(obj);
            OnObjectDestroyed(obj);
        }

        public virtual void OnObjectDestroyed(ISceneObject obj)
        {
        }

        public List<ISceneObject> GetLevelObjects()
        {
            return objects;
        }
    }
}
