using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Orbit.Core.Scene.Entities.Implementations;
using Orbit.Core.Weapons;
using System.Windows;
using Orbit.Core.Players;

namespace Orbit.Core.Server.Level
{
    public class LevelTestEmpty : IGameLevel
    {
        public static readonly LevelInfo Info = new LevelInfo(true, "[TEST] Empty map");

        private ServerMgr mgr;

        public LevelTestEmpty(ServerMgr serverMgr)
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
        }

        public void CreateBots(List<Player> players, int suggestedCount, BotType type)
        {
        }
    }
}
