using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Orbit.Core;
using Orbit.Core.Scene;
using Lidgren.Network;

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

                Cost *= SharedDef.HEAL_MULTIPLY_COEF;

                if (SceneMgr.GetInstance().GameType != Gametype.SOLO_GAME)
                    sendMessage();
            }
        }

        private void sendMessage()
        {
            NetOutgoingMessage message = SceneMgr.CreateNetMessage();
            message.Write((int)PacketType.PLAYER_HEAL);
            message.Write((int)SceneMgr.GetInstance().GetMePlayer().Baze.Integrity);

            SceneMgr.SendMessage(message);
        }
    }
}
