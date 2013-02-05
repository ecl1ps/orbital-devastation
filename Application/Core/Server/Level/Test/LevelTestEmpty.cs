using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Orbit.Core.Scene.Entities.Implementations;
using Orbit.Core.Weapons;
using System.Windows;
using Orbit.Core.Players;
using Orbit.Core.Scene.Entities;

namespace Orbit.Core.Server.Level
{
    public class LevelTestEmpty : IGameLevel
    {
        public static readonly LevelInfo Info = new LevelInfo(true, "[TEST] Empty map");

        protected ServerMgr mgr;

        public LevelTestEmpty(ServerMgr serverMgr)
        {
            mgr = serverMgr;
        }

        public virtual void CreateLevelObjects()
        {
        }

        public virtual void Update(float tpf)
        {
        }

        public virtual void OnStart()
        {
        }

        public virtual void CreateBots(List<Player> players, int suggestedCount, BotType type)
        {
            for (int i = 0; i < suggestedCount; ++i)
            {
                players.Add(GameLevelManager.CreateBot(type, players));
            }
        }

        public virtual void OnObjectDestroyed(ISceneObject obj)
        {
        }
    }
}
