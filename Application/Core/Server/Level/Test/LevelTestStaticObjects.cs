using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Orbit.Core.Scene.Entities.Implementations;
using Orbit.Core.Weapons;
using System.Windows;
using Orbit.Core.Players;
using Orbit.Core.Scene.Entities;
using Orbit.Core.Scene.Controls.Implementations;

namespace Orbit.Core.Server.Level
{
    public class LevelTestStaticObjects : AbstractGameLevel
    {
        public static LevelInfo Info = new LevelInfo(true, Strings.lvl_type_static);

        public LevelTestStaticObjects(ServerMgr serverMgr) : base(serverMgr)
        {
        }

        public override void OnStart()
        {
            Rect baseLoc = PlayerBaseLocation.GetBaseLocation(PlayerPosition.LEFT);

            GameLevelManager.SendNewObject(mgr, MakeObjectStatic(ServerSceneObjectFactory.CreateCustomAsteroid(
                mgr, 10, new Vector(baseLoc.X - 10 * 2 - 1, 100), new Vector(0, 0), AsteroidType.NORMAL)));
            GameLevelManager.SendNewObject(mgr, MakeObjectStatic(ServerSceneObjectFactory.CreateCustomAsteroid(
                mgr, 20, new Vector(baseLoc.X - 20 * 2, 200), new Vector(0, 0), AsteroidType.NORMAL)));
            GameLevelManager.SendNewObject(mgr, MakeObjectStatic(ServerSceneObjectFactory.CreateCustomAsteroid(
                mgr, 10, new Vector(baseLoc.X + baseLoc.Width + 1, 100), new Vector(0, 0), AsteroidType.NORMAL)));
            GameLevelManager.SendNewObject(mgr, MakeObjectStatic(ServerSceneObjectFactory.CreateCustomAsteroid(
                mgr, 20, new Vector(baseLoc.X + baseLoc.Width, 200), new Vector(0, 0), AsteroidType.NORMAL)));


            GameLevelManager.SendNewObject(mgr, MakeObjectStatic(ServerSceneObjectFactory.CreateCustomAsteroid(
                mgr, 20, new Vector(300, 200), new Vector(0, 0), AsteroidType.GOLDEN)));
            GameLevelManager.SendNewObject(mgr, MakeObjectStatic(ServerSceneObjectFactory.CreateCustomAsteroid(
                mgr, 20, new Vector(500, 200), new Vector(0, 0), AsteroidType.GOLDEN)));


            StatPowerUp sp = CreateNewStatPowerup();
            sp.Position = new Vector(100, 100);
            GameLevelManager.SendNewObject(mgr, MakeObjectStatic(sp));
            sp = CreateNewStatPowerup();
            sp.Position = new Vector(100, 200);
            GameLevelManager.SendNewObject(mgr, MakeObjectStatic(sp));
            sp = CreateNewStatPowerup();
            sp.Position = new Vector(200, 100);
            GameLevelManager.SendNewObject(mgr, MakeObjectStatic(sp));
            sp = CreateNewStatPowerup();
            sp.Position = new Vector(200, 200);
            GameLevelManager.SendNewObject(mgr, MakeObjectStatic(sp));
        }

        private ISceneObject MakeObjectStatic(ISceneObject obj)
        {
            NewtonianMovementControl nmc = obj.GetControlOfType<NewtonianMovementControl>();
            nmc.Speed = 0;
            nmc.Enabled = false;

            return obj;
        }

        private StatPowerUp CreateNewStatPowerup()
        {
            StatPowerUp p = ServerSceneObjectFactory.CreateStatPowerUp(mgr,(DeviceType)mgr.GetRandomGenerator().Next((int)DeviceType.DEVICE_FIRST + 1, (int)DeviceType.DEVICE_LAST));
            p.Direction = new Vector(0, 0);
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
