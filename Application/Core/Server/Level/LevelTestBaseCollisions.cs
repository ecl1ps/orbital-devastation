﻿using System;
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
    public class LevelTestBaseCollisions : IGameLevel
    {
        private ServerMgr mgr;

        public LevelTestBaseCollisions(ServerMgr serverMgr)
        {
            mgr = serverMgr;
        }

        public void CreateLevelObjects()
        {

        }

        public void Update(float tpf)
        {

        }

        public void OnStart()
        {
            Rect baseLoc = PlayerBaseLocation.GetBaseLocation(PlayerPosition.LEFT);

            GameLevelManager.SendNewObject(mgr, ServerSceneObjectFactory.CreateCustomAsteroid(mgr, 10, new Vector(baseLoc.X - 10 * 2 - 1, 100), new Vector(0, 1)));
            GameLevelManager.SendNewObject(mgr, ServerSceneObjectFactory.CreateCustomAsteroid(mgr, 20, new Vector(baseLoc.X - 20 * 2, 200), new Vector(0, 1)));
            GameLevelManager.SendNewObject(mgr, ServerSceneObjectFactory.CreateCustomAsteroid(mgr, 10, new Vector(baseLoc.X + baseLoc.Width + 1, 100), new Vector(0, 1)));
            GameLevelManager.SendNewObject(mgr, ServerSceneObjectFactory.CreateCustomAsteroid(mgr, 20, new Vector(baseLoc.X + baseLoc.Width, 200), new Vector(0, 1)));

            GameLevelManager.SendNewObject(mgr, CreateAndSendNewStatPowerup(new Vector(0, 1)));
            GameLevelManager.SendNewObject(mgr, CreateAndSendNewStatPowerup(new Vector(0, 1)));
            GameLevelManager.SendNewObject(mgr, CreateAndSendNewStatPowerup(new Vector(0, 1)));
            GameLevelManager.SendNewObject(mgr, CreateAndSendNewStatPowerup(new Vector(0, 1)));
        }

        private StatPowerUp CreateAndSendNewStatPowerup(Vector dir)
        {
            StatPowerUp p = ServerSceneObjectFactory.CreateStatPowerUp(mgr,
                (DeviceType)mgr.GetRandomGenerator().Next((int)DeviceType.WEAPON_FIRST + 1, (int)DeviceType.WEAPON_LAST));
            p.Direction = dir;
            return p;
        }

        public bool IsBotAllowed()
        {
            return false;
        }
    }
}