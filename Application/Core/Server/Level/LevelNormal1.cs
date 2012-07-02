using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Orbit.Core.Scene.Entities;
using Orbit.Core.Scene.Entities.Implementations;
using Lidgren.Network;

namespace Orbit.Core.Server.Level
{
    public class LevelNormal1 : IGameLevel
    {
        private ServerMgr mgr;
        private List<ISceneObject> objects;
        private float newStatPowerupTimer;
        private float newAsteroidTimer;

        public LevelNormal1(ServerMgr serverMgr, List<ISceneObject> objs)
        {
            mgr = serverMgr;
            objects = objs;
            newStatPowerupTimer = 1;
            newAsteroidTimer = SharedDef.NEW_ASTEROID_TIMER;
        }

        public void CreateLevelObjects()
        {
            for (int i = 0; i < SharedDef.ASTEROID_COUNT; ++i)
                objects.Add(ServerSceneObjectFactory.CreateNewRandomAsteroid(mgr, i % 2 == 0));
        }

        public void Update(float tpf)
        {
            if (newAsteroidTimer <= tpf)
            {
                Asteroid a = ServerSceneObjectFactory.CreateNewAsteroidOnEdge(mgr, objects.Count % 2 == 0);
                objects.Add(a);
                NetOutgoingMessage msg = mgr.CreateNetMessage();
                a.WriteObject(msg);
                mgr.BroadcastMessage(msg);

                newAsteroidTimer = SharedDef.NEW_ASTEROID_TIMER;
            }
            else
                newAsteroidTimer -= tpf;

            if (newStatPowerupTimer <= tpf)
            {
                GameLevelManager.CreateAndSendNewStatPowerup(mgr);
                newStatPowerupTimer = mgr.GetRandomGenerator().Next(SharedDef.NEW_STAT_POWERUP_TIMER_MIN, SharedDef.NEW_STAT_POWERUP_TIMER_MAX + 1);
            }
            else
                newStatPowerupTimer -= tpf;
        }

        public void OnStart()
        {

        }

        public bool IsBotAllowed()
        {
            return true;
        }
    }
}
