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
using Orbit.Core.Scene.Controls.Implementations;
using Orbit.Core.Scene.Controls;
using Microsoft.Xna.Framework;

namespace Orbit.Core.Server.Level
{
    public class LevelTestBurningAsteroids : AbstractGameLevel
    {
        public static LevelInfo Info = new LevelInfo(true, "[TEST] Burning asteroids");

        private int radius = 20;
        private int timer = 3;

        public LevelTestBurningAsteroids(ServerMgr serverMgr)
            : base(serverMgr)
        {
        }

        public override void OnStart()
        {
            events.AddEvent(1, new Event(timer, EventType.ONE_TIME, new Action(() => CreateAndSendNewAsteroid(1))));
        }

        private void CreateAndSendNewAsteroid(int step)
        {
            Asteroid a = null;
            switch (step)
            {
                case 1:
                    a = ServerSceneObjectFactory.CreateCustomAsteroid(mgr, radius, new Vector2(500, 200), new Vector2(0, 1), AsteroidType.NORMAL);
                    a.GetControlOfType<IMovementControl>().Speed = 100;
                    break;
                case 2:
                    a = ServerSceneObjectFactory.CreateCustomAsteroid(mgr, radius, new Vector2(500, 200), new Vector2(1, 1).NormalizeV(), AsteroidType.NORMAL);
                    a.GetControlOfType<IMovementControl>().Speed = 100;
                    break;
                case 3:
                    a = ServerSceneObjectFactory.CreateCustomAsteroid(mgr, radius, new Vector2(500, 300), new Vector2(1, 0), AsteroidType.NORMAL);
                    a.GetControlOfType<IMovementControl>().Speed = 100;
                    break;
                case 4:
                    a = ServerSceneObjectFactory.CreateCustomAsteroid(mgr, radius / 2, new Vector2(500, 300), new Vector2(-1, 0), AsteroidType.NORMAL);
                    a.GetControlOfType<IMovementControl>().Speed = 100;
                    break;
                case 5:
                    a = ServerSceneObjectFactory.CreateCustomAsteroid(mgr, radius, new Vector2(550, 400), new Vector2(0, -1), AsteroidType.NORMAL);
                    a.GetControlOfType<IMovementControl>().Speed = 60;
                    break;
                case 6:
                    a = ServerSceneObjectFactory.CreateCustomAsteroid(mgr, radius, new Vector2(500, 400), new Vector2(1, -1).NormalizeV(), AsteroidType.NORMAL);
                    a.GetControlOfType<IMovementControl>().Speed = 40;
                    break;
                default:
                    break;
            }

            a.TextureId = 4;
            a.RemoveControlsOfType<LinearRotationControl>();

            GameLevelManager.SendNewObject(mgr, a);

            step++;
            if (step > 6)
                step = 1;

            events.AddEvent(step, new Event(timer, EventType.ONE_TIME, new Action(() => CreateAndSendNewAsteroid(step))));
        }
    }
}
