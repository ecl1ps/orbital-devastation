using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Orbit.Core.SpecialActions;
using Orbit.Core.Players;
using System.Runtime.Serialization;
using Orbit.Core.Scene.Entities;
using Orbit.Core.Helpers;

namespace Orbit.Core.Players
{
    public class StatisticsManager : ISendable
    {
        public Player Owner { get; set; }

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

        public int PoweredActions { get; set; }
        public List<ISpecialAction> Actions { get; set; }
        public List<Stat> Stats { get; set; }

        private float time = 0;
        public float Time { get { return time; } }

        public StatisticsManager()
        {
            Reset();
        }

        public void Reset()
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
            PoweredActions = 0;

            Actions = new List<ISpecialAction>();
            Stats = new List<Stat>();
        }

        public void Update(float tpf)
        {
            if(!GameEnded)
                time += tpf;
        }

        public void WriteObject(Lidgren.Network.NetOutgoingMessage msg)
        {
            msg.Write(BulletFired);
            msg.Write(BulletHit);
            msg.Write(MineFired);
            msg.Write(MineHit);
            msg.Write(HookFired);
            msg.Write(HookHit);
            msg.Write(Healed);
            msg.Write(GoldEarned);
            msg.Write(DamageTaken);
            msg.Write(DeadTime);
            msg.Write(Time);
            msg.Write(PoweredActions);

            msg.WriteSpecialActions(Actions);
            msg.WriteStats(Stats);
        }

        public void ReadObject(Lidgren.Network.NetIncomingMessage msg)
        {
            BulletFired = msg.ReadFloat();
            BulletHit = msg.ReadFloat();
            MineFired = msg.ReadFloat();
            MineHit = msg.ReadFloat();
            HookFired = msg.ReadFloat();
            HookHit = msg.ReadFloat();
            Healed = msg.ReadFloat();
            GoldEarned = msg.ReadFloat();
            DamageTaken = msg.ReadFloat();
            DeadTime = msg.ReadFloat();
            time = msg.ReadFloat();
            PoweredActions = msg.ReadInt32();

            Actions = msg.ReadSpecialActions(Owner.SceneMgr);
            Stats = msg.ReadStats();
        }
    }
}
