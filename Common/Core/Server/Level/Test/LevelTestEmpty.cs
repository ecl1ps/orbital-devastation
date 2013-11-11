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
    public class LevelTestEmpty : AbstractGameLevel
    {
        public static LevelInfo Info = new LevelInfo(true, Strings.lvl_type_empty);

        public LevelTestEmpty(ServerMgr serverMgr) : base(serverMgr)
        {
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
