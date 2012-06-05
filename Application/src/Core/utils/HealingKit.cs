using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Orbit.Core;
using Orbit.Core.Scene;

namespace Orbit.src.Core.utils
{
   public class HealingKit : IHealingKit
    {
        public int Cost { get; set; }

        public HealingKit()
        {
            Cost = SharedDef.HEAL_START_COST;
        }

        public void heal()
        {
            if (SceneMgr.GetInstance().GetMePlayer().Data.Gold >= Cost)
            {
                SceneMgr.GetInstance().GetMePlayer().Data.Gold -= Cost;
                SceneMgr.GetInstance().GetMePlayer().Baze.Integrity += SharedDef.HEAL_AMOUNT;
                if (SceneMgr.GetInstance().GetMePlayer().Baze.Integrity > SharedDef.BASE_MAX_INGERITY)
                    SceneMgr.GetInstance().GetMePlayer().Baze.Integrity = SharedDef.BASE_MAX_INGERITY;

                Cost *= 10;
            }
        }
    }
}
