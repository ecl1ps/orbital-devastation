using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Orbit.Core.Scene.Entities;
using Orbit.Core.Scene.Entities.Implementations;
using Lidgren.Network;

namespace Orbit.Core.Server.Level
{
    public class LevelTestPoweUp : IGameLevel
    {
        private ServerMgr mgr;
        private List<ISceneObject> objects;
        private float newStatPowerupTimer;

        public LevelTestPoweUp(ServerMgr serverMgr, List<ISceneObject> objs)
        {
            mgr = serverMgr;
            objects = objs;
            newStatPowerupTimer = 1;
        }

        public void CreateLevelObjects()
        {
        }

        public void Update(float tpf)
        {
            if (newStatPowerupTimer <= tpf)
            {
                GameLevelManager.CreateAndSendNewStatPowerup(mgr);
                newStatPowerupTimer = 1;
            }
            else
                newStatPowerupTimer -= tpf;
        }

        public void OnStart()
        {
        }

        public bool IsBotAllowed()
        {
            return false;
        }
    }
}
