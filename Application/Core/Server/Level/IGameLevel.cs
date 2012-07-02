using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Orbit.Core.Server.Level
{
    public interface IGameLevel
    {
        void CreateLevelObjects();

        void Update(float tpf);

        void OnStart();

        bool IsBotAllowed();
    }
}
