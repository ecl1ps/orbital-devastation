using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Orbit.Core.Scene.Entities;
using Orbit.Core.Scene.Entities.Implementations;
using Orbit.Core.Weapons;
using Lidgren.Network;

namespace Orbit.Core.Server.Level
{
    public static class GameLevelManager
    {
        public static IGameLevel CreateNewGameLevel(ServerMgr mgr, GameLevel lvl, List<ISceneObject> objects)
        {
            IGameLevel newLvl = null;
            switch (lvl)
            {
                case GameLevel.NORMAL1:
                    newLvl = new LevelNormal1(mgr, objects);
                    break;
                case GameLevel.TEST_BASE_COLLISIONS:
                    newLvl = new LevelTestBaseCollisions(mgr);
                    break;
                case GameLevel.TEST_POWERUPS:
                    newLvl = new LevelTestPoweUp(mgr, objects);
                    break;
            }

            return newLvl;
        }

        public static void CreateAndSendNewStatPowerup(ServerMgr serverMgr)
        {
            StatPowerUp p = ServerSceneObjectFactory.CreateStatPowerUp(serverMgr,
                (WeaponType)serverMgr.GetRandomGenerator().Next((int)WeaponType.WEAPON_FIRST + 1, (int)WeaponType.WEAPON_LAST));
            NetOutgoingMessage powerupMsg = serverMgr.CreateNetMessage();
            p.WriteObject(powerupMsg);
            serverMgr.BroadcastMessage(powerupMsg);
        }

        public static void SendNewObject(ServerMgr serverMgr, ISceneObject obj)
        {
            if (!(obj is ISendable))
                return;

            NetOutgoingMessage msg = serverMgr.CreateNetMessage();
            (obj as ISendable).WriteObject(msg);
            serverMgr.BroadcastMessage(msg);
        }
    }
}
