using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Orbit.Core.Players;

namespace Orbit.Core.Server.Level
{
    public interface IGameLevel
    {
        void CreateLevelObjects();

        void Update(float tpf);

        void OnStart();

        void CreateBots(List<Player> players, int suggestedCount, BotType type);
    }
}
