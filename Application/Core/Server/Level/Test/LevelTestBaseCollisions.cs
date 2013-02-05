using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Orbit.Core.Scene.Entities;
using Orbit.Core.Players;
using System.Windows;
using Orbit.Core.Scene.Entities.Implementations;
using Orbit.Core.Weapons;
using Lidgren.Network;

namespace Orbit.Core.Server.Level
{
    public class LevelTestBaseCollisions : AbstractGameLevel
    {
        public static readonly LevelInfo Info = new LevelInfo(true, "[TEST] Base collisions");

        public LevelTestBaseCollisions(ServerMgr serverMgr) : base(serverMgr)
        {
        }

        public override void OnStart()
        {
            Rect baseLoc = PlayerBaseLocation.GetBaseLocation(PlayerPosition.LEFT);

            GameLevelManager.SendNewObject(mgr, ServerSceneObjectFactory.CreateCustomAsteroid(
                mgr, 10, new Vector(baseLoc.X - 10 * 2 - 1, 100), new Vector(0, 1), AsteroidType.NORMAL));
            GameLevelManager.SendNewObject(mgr, ServerSceneObjectFactory.CreateCustomAsteroid(
                mgr, 20, new Vector(baseLoc.X - 20 * 2, 200), new Vector(0, 1), AsteroidType.NORMAL));
            GameLevelManager.SendNewObject(mgr, ServerSceneObjectFactory.CreateCustomAsteroid(
                mgr, 10, new Vector(baseLoc.X + baseLoc.Width + 1, 100), new Vector(0, 1), AsteroidType.NORMAL));
            GameLevelManager.SendNewObject(mgr, ServerSceneObjectFactory.CreateCustomAsteroid(
                mgr, 20, new Vector(baseLoc.X + baseLoc.Width, 200), new Vector(0, 1), AsteroidType.NORMAL));

            baseLoc = PlayerBaseLocation.GetBaseLocation(PlayerPosition.RIGHT);
            GameLevelManager.SendNewObject(mgr, CreateAndSendNewStatPowerup(new Vector(baseLoc.X - 10 * 2 - 1, 100), new Vector(0, 1)));
            GameLevelManager.SendNewObject(mgr, CreateAndSendNewStatPowerup(new Vector(baseLoc.X - 20 * 2, 200), new Vector(0, 1)));
            GameLevelManager.SendNewObject(mgr, CreateAndSendNewStatPowerup(new Vector(baseLoc.X + baseLoc.Width + 1, 100), new Vector(0, 1)));
            GameLevelManager.SendNewObject(mgr, CreateAndSendNewStatPowerup(new Vector(baseLoc.X + baseLoc.Width, 200), new Vector(0, 1)));
        }

        private StatPowerUp CreateAndSendNewStatPowerup(Vector pos, Vector dir)
        {
            StatPowerUp p = ServerSceneObjectFactory.CreateStatPowerUp(mgr,
                (DeviceType)mgr.GetRandomGenerator().Next((int)DeviceType.DEVICE_FIRST + 1, (int)DeviceType.DEVICE_LAST));
            p.Position = pos;
            p.Direction = dir;
            return p;
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
