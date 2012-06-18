using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Orbit.Core;
using Orbit.Core.Scene;
using Lidgren.Network;
using Orbit.Core.Client;
using Orbit.Core.Players;

namespace Orbit.Core.Utils
{
    public class HealingKit : IHealingKit
    {
        public int Cost { get; set; }

        private SceneMgr mgr;
        private Player owner;

        public HealingKit(SceneMgr mgr, Player owner)
        {
            Cost = SharedDef.HEAL_START_COST;
            this.mgr = mgr;
            this.owner = owner;
        }

        public void Heal()
        {
            if (owner.Data.Gold >= Cost)
            {
                owner.AddGoldAndShow(-Cost);
                owner.ChangeBaseIntegrity(SharedDef.HEAL_AMOUNT);
                if (owner.GetBaseIntegrity() > SharedDef.BASE_MAX_INGERITY)
                    owner.SetBaseIntegrity(SharedDef.BASE_MAX_INGERITY);

                Cost *= SharedDef.HEAL_MULTIPLY_COEF;

                if (mgr.GameType != Gametype.SOLO_GAME)
                    SendMessage();
            }
        }

        private void SendMessage()
        {
            NetOutgoingMessage message = mgr.CreateNetMessage();
            message.Write((int)PacketType.PLAYER_HEAL);
            message.Write((int)owner.GetId());
            message.Write((int)owner.GetBaseIntegrity());

            mgr.SendMessage(message);
        }
    }
}
