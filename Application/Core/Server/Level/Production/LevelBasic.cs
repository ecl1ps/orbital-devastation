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
    public class LevelBasic : IGameLevel
    {
        public static readonly LevelInfo Info = new LevelInfo(false, "Basic map");

        protected ServerMgr mgr;
        protected List<ISceneObject> objects;
        protected EventProcessor events;

        private enum Events
        {
            ADDITIONAL_ASTEROID,
            NEW_STAT_POWERUP
        }

        public LevelBasic(ServerMgr serverMgr, List<ISceneObject> objs)
        {
            mgr = serverMgr;
            objects = objs;
            events = new EventProcessor();

            events.AddEvent((int)Events.ADDITIONAL_ASTEROID, new Event(SharedDef.NEW_ASTEROID_TIMER, EventType.REPEATABLE, new Action(() => CreateAndSendAdditionalAsteroid())));
            events.AddEvent((int)Events.NEW_STAT_POWERUP, new Event(1, EventType.REPEATABLE, new Action(() => CreateAndSendNewStatPowerup())));
        }

        public void CreateLevelObjects()
        {
            for (int i = 0; i < SharedDef.ASTEROID_COUNT; ++i)
                objects.Add(ServerSceneObjectFactory.CreateNewRandomAsteroid(mgr, i % 2 == 0));
        }

        public void Update(float tpf)
        {
            events.Update(tpf);
        }

        private void CreateAndSendNewStatPowerup()
        {
            GameLevelManager.CreateAndSendNewStatPowerup(mgr);
            events.RescheduleEvent((int)Events.NEW_STAT_POWERUP, mgr.GetRandomGenerator().Next(SharedDef.NEW_STAT_POWERUP_TIMER_MIN, SharedDef.NEW_STAT_POWERUP_TIMER_MAX + 1));
        }

        private void CreateAndSendAdditionalAsteroid()
        {
            Asteroid a = ServerSceneObjectFactory.CreateNewAsteroidOnEdge(mgr, objects.Count % 2 == 0);
            objects.Add(a);
            NetOutgoingMessage msg = mgr.CreateNetMessage();
            a.WriteObject(msg);
            mgr.BroadcastMessage(msg);
        }

        public virtual void OnStart()
        {
        }

        public void CreateBots(List<Player> players, int suggestedCount, BotType type)
        {
            for (int i = 0; i < suggestedCount; ++i)
            {
                players.Add(GameLevelManager.CreateBot(type, players));
            }
        }
    }
}
