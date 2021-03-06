﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Orbit.Core.Scene.Entities;
using Orbit.Core.Scene.Entities.Implementations;
using Orbit.Core.Weapons;
using Lidgren.Network;
using Orbit.Core.Players;
using System.Windows.Media;
using Orbit.Core.AI;
using Orbit.Core.Server.Level.Test;

namespace Orbit.Core.Server.Level
{
    public enum GameLevel
    {
        BASIC_MAP,
        SURVIVAL_MAP,

        TEST_EMPTY,
        TEST_BASE_COLLISIONS,
        TEST_POWERUPS,
        TEST_STATIC_OBJ,
        TEST_PARTICLES,
        TEST_BURNING_ASTEROIDS
    }

    public static class GameLevelManager
    {
        private static readonly log4net.ILog Logger = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        public static IGameLevel CreateNewGameLevel(ServerMgr mgr, GameLevel lvl)
        {
            IGameLevel newLvl = null;
            switch (lvl)
            {
                case GameLevel.BASIC_MAP:
                    newLvl = new LevelBasic(mgr);
                    break;
                case GameLevel.SURVIVAL_MAP:
                    newLvl = new LevelSurvival(mgr);
                    break;

                // testovaci
                case GameLevel.TEST_EMPTY:
                    newLvl = new LevelTestEmpty(mgr);
                    break;
                case GameLevel.TEST_BASE_COLLISIONS:
                    newLvl = new LevelTestBaseCollisions(mgr);
                    break;
                case GameLevel.TEST_POWERUPS:
                    newLvl = new LevelTestPoweUp(mgr);
                    break;
                case GameLevel.TEST_STATIC_OBJ:
                    newLvl = new LevelTestStaticObjects(mgr);
                    break;
                case GameLevel.TEST_PARTICLES:
                    newLvl = new LevelTestParticles(mgr);
                    break;
                case GameLevel.TEST_BURNING_ASTEROIDS:
                    newLvl = new LevelTestBurningAsteroids(mgr);
                    break;
            }

            return newLvl;
        }

        public static void CreateAndSendNewStatPowerup(ServerMgr serverMgr)
        {
            StatPowerUp p = ServerSceneObjectFactory.CreateStatPowerUp(serverMgr,
                (DeviceType)serverMgr.GetRandomGenerator().Next((int)DeviceType.DEVICE_FIRST + 1, (int)DeviceType.DEVICE_LAST));
            SendNewObject(serverMgr, p);
        }

        public static void SendNewObject(ServerMgr serverMgr, ISceneObject obj)
        {
            if (!(obj is ISendable))
            {
                Logger.Error("Trying to send " + obj.GetType().Name + " but it is not ISendable");
                return;
            }

            NetOutgoingMessage msg = serverMgr.CreateNetMessage();
            (obj as ISendable).WriteObject(msg);
            serverMgr.BroadcastMessage(msg);
        }

        public static Player CreateBot(BotType type, List<Player> players)
        {
            Player bot = new Player(null);
            bot.Data = new PlayerData();
            bot.Data.Id = IdMgr.GetNewPlayerId();
            String name = BotNameAccessor.GetBotName(type);
            if (players.Exists(p => p.Data.Name.Equals(name)))
            {
                Random rand = new Random(Environment.TickCount);
                bot.Data.PlayerColor = Color.FromRgb((byte)rand.Next(255), (byte)rand.Next(255), (byte)rand.Next(255));
                bot.Data.Name = name + " " + bot.GetId();
            }
            else
            {
                Color botColor = players[0].Data.PlayerColor;
                bot.Data.PlayerColor = Color.FromRgb((byte)(0xFF - botColor.R), (byte)(0xFF - botColor.G), (byte)(0xFF - botColor.B));
                bot.Data.Name = name;
            }
            bot.Data.HashId = bot.Data.Name;

            bot.Data.PlayerType = PlayerType.BOT;
            bot.Data.BotType = type;
            bot.Data.StartReady = true;

            return bot;
        }
    }
}
