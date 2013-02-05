using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Orbit.Core.Scene.Entities;
using Orbit.Core.Scene.Entities.Implementations;
using Lidgren.Network;
using Orbit.Core.Players;
using System.Windows;
using Orbit.Core.Scene.CollisionShapes;

namespace Orbit.Core.Server.Level
{
    public class LevelSurvival : AbstractGameLevel
    {
        public static readonly LevelInfo Info = new LevelInfo(false, "Survival map");

        private float newAsteroidTimer;

        private enum Events
        {
            NEW_ASTEROID,
            NEW_STAT_POWERUP
        }

        public LevelSurvival(ServerMgr serverMgr) : base(serverMgr)
        {
            newAsteroidTimer = SharedDef.LEVEL_SURVIVAL_ASTEROID_TIMER;

            events.AddEvent((int)Events.NEW_ASTEROID, new Event(newAsteroidTimer, EventType.REPEATABLE, new Action(() => CreateAndSendNewAsteroid())));
            events.AddEvent((int)Events.NEW_STAT_POWERUP, new Event(1, EventType.REPEATABLE, new Action(() => CreateAndSendNewStatPowerup())));
        }

        protected override void CreateLevelObjects()
        {
            for (int i = 0; i < SharedDef.LEVEL_SURVIVAL_ASTEROID_COUNT; ++i)
                objects.Add(CreateNewAsteroidAbove());
        }

        private void CreateAndSendNewStatPowerup()
        {
            GameLevelManager.CreateAndSendNewStatPowerup(mgr);
            events.RescheduleEvent((int)Events.NEW_STAT_POWERUP, mgr.GetRandomGenerator().Next(SharedDef.NEW_STAT_POWERUP_TIMER_MIN, SharedDef.NEW_STAT_POWERUP_TIMER_MAX + 1));
        }

        private void CreateAndSendNewAsteroid()
        {
            Asteroid a = CreateNewAsteroidAbove();
            NetOutgoingMessage msg = mgr.CreateNetMessage();
            a.WriteObject(msg);
            mgr.BroadcastMessage(msg);

            if (newAsteroidTimer > SharedDef.LEVEL_SURVIVAL_ASTEROID_TIMER_CAP)
                newAsteroidTimer *= 0.999f;

            events.RescheduleEvent((int)Events.NEW_ASTEROID, newAsteroidTimer);
        }

        private Asteroid CreateNewAsteroidAbove()
        {
            Asteroid s = ServerSceneObjectFactory.CreateNewRandomAsteroid(mgr, true);

            s.Position = new Vector(mgr.GetRandomGenerator().Next((int)(SharedDef.VIEW_PORT_SIZE.Width - s.Radius * 2)), -s.Radius * 4);
            (s.CollisionShape as SphereCollisionShape).Center = s.Center;
            s.Direction = new Vector(0, 1).Rotate(mgr.GetRandomGenerator().Next(60) - 30, false); // -30° - +30°

            return s;
        }

        public override void CreateBots(List<Player> players, int suggestedCount, BotType type)
        {
            for (int i = 0; i < suggestedCount; ++i)
            {
                players.Add(GameLevelManager.CreateBot(type, players));
            }
        }
    }
}
