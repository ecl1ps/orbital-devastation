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
        public bool GameEnded { get; set; }

        public float BulletFired { get; set; }
        public float BulletHit { get; set; }
        public float MineFired { get; set; }
        public float MineHit { get; set; }
        public float HookFired { get; set; }
        public float HookHit { get; set; }

        public float Healed { get; set; }
        public float GoldEarned { get; set; }
        public float DamageTaken { get; set; }
        public float DeadTime { get; set; }

        public List<ISpecialAction> Actions { get; set; }
        public List<Stat> Stats { get; set; }

        private float time = 0;
        public float Time {get {return time;}}

        public StatisticsManager()
        {
            GameEnded = false;

            BulletFired = 0;
            BulletHit = 0;
            MineFired = 0;
            MineHit = 0;
            HookFired = 0;
            HookHit = 0;

            Healed = 0;
            GoldEarned = 0;
            DamageTaken = 0;
            DeadTime = 0;

            Actions = new List<ISpecialAction>();
            Stats = new List<Stat>();
        }

        public void Update(float tpf)
        {
            if(!GameEnded)
                time += tpf;
        }
    }
}
