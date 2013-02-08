using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Orbit.Core.Scene.Entities;
using Orbit.Core.Scene.Entities.Implementations;
using Lidgren.Network;
using Orbit.Core.Players;

namespace Orbit.Core.Server.Level
{
    public class LevelTestPoweUp : AbstractGameLevel
    {
        public static LevelInfo Info = new LevelInfo(true, Strings.lvl_type_powerup);

        public LevelTestPoweUp(ServerMgr serverMgr) : base(serverMgr)
        {
            events.AddEvent(1, new Event(0.2f, EventType.REPEATABLE, new Action(() => GameLevelManager.CreateAndSendNewStatPowerup(mgr))));
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
