using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Orbit.Core.SpecialActions;
using Orbit.Core.Players;

namespace Orbit.Core.Client.GameStates
{
    public class StatisticsManager : IGameState
    {
        public float BulletFired { get; set; }
        public float BulletHit { get; set; }
        public float MineFired { get; set; }
        public float MineHit { get; set; }
        public float HookFired { get; set; }
        public float HookHit { get; set; }
        public float Healed { get; set; }
        public float GoldEarned { get; set; }

        public List<ISpecialAction> Actions { get; set; }
        public List<Stat> Stats { get; set; }

        private float time = 0;
        public float Time {get {return time;}}

        public void Update(float tpf)
        {
            time += tpf;
        }
    }
}
