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
        public static readonly LevelInfo Info = new LevelInfo(true, "[TEST] Powerups");

        private ServerMgr mgr;
        private List<ISceneObject> objects;
        private EventProcessor events;

        public LevelTestPoweUp(ServerMgr serverMgr, List<ISceneObject> objs)
        {
            mgr = serverMgr;
            objects = objs;
            events = new EventProcessor();

            events.AddEvent(1, new Event(0.2f, EventType.REPEATABLE, new Action(() => GameLevelManager.CreateAndSendNewStatPowerup(mgr))));
        }

        public void CreateLevelObjects()
        {
        }

        public void Update(float tpf)
        {
            events.Update(tpf);
        }

        public void OnStart()
        {
        }

        public void CreateBots(List<Player> players, int suggestedCount, BotType type)
        {
        }

        public void OnObjectDestroyed(ISceneObject obj)
        {
        }
    }
}
