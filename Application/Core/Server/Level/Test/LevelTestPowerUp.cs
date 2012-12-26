using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Orbit.Core.Scene.Entities;
using Orbit.Core.Scene.Entities.Implementations;
using Lidgren.Network;
using Orbit.Core.Players;

namespace Orbit.Core.Server.Level
{
    public class LevelTestPoweUp : IGameLevel
    {
        public static LevelInfo Info = new LevelInfo(true, "[TEST] Powerups");

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

        public void CreateBots(List<Player> players, int suggestedCount, BotType type)
        {
        }
    }
}
