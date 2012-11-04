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
    class LevelTestEmpty : IGameLevel
    {
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

        public bool IsBotAllowed()
        {
            return false;
        }
    }
}
