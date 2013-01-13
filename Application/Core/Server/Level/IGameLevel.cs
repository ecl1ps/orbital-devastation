using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Orbit.Core.Players;
using Orbit.Core.Scene.Entities;

namespace Orbit.Core.Server.Level
{
    public interface IGameLevel
    {
        void CreateLevelObjects();

        void Update(float tpf);

        void OnStart();

        void CreateBots(List<Player> players, int suggestedCount, BotType type);

        void OnObjectDestroyed(ISceneObject obj);
    }
}
