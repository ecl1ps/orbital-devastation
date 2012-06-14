using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Orbit.Core;
using Orbit.Core.Scene;
using Lidgren.Network;

namespace Orbit.Core.Utils
{
   public class HealingKit : IHealingKit
    {
        public int Cost { get; set; }

        private SceneMgr mgr;

        public HealingKit(SceneMgr mgr)
        {
            Cost = SharedDef.HEAL_START_COST;
            this.mgr = mgr;
        }

        public void Heal()
        {
            if (mgr.GetCurrentPlayer().Data.Gold >= Cost)
            {
                mgr.GetCurrentPlayer().AddGoldAndShow(-Cost);
                mgr.GetCurrentPlayer().ChangeBaseIntegrity(SharedDef.HEAL_AMOUNT);
                if (mgr.GetCurrentPlayer().GetBaseIntegrity() > SharedDef.BASE_MAX_INGERITY)
                    mgr.GetCurrentPlayer().SetBaseIntegrity(SharedDef.BASE_MAX_INGERITY);

                Cost *= SharedDef.HEAL_MULTIPLY_COEF;

                if (mgr.GameType != Gametype.SOLO_GAME)
                    SendMessage();
            }
        }

        private void SendMessage()
        {
            NetOutgoingMessage message = mgr.CreateNetMessage();
            message.Write((int)PacketType.PLAYER_HEAL);
            message.Write((int)mgr.GetCurrentPlayer().GetBaseIntegrity());

            mgr.SendMessage(message);
        }
    }
}
