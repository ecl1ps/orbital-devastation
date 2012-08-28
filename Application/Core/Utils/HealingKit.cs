using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Orbit.Core;
using Orbit.Core.Scene;
using Lidgren.Network;
using Orbit.Core.Client;
using Orbit.Core.Players;
using System.Windows;
using Orbit.Core.Weapons;

namespace Orbit.Core.Utils
{
    public class HealingKit : IHealingKit
    {
        public UpgradeLevel UpgradeLevel { get; set; }
        public int Cost { get; set; }

        private SceneMgr mgr;
        private Player owner;

        public HealingKit(SceneMgr mgr, Player owner)
        {
            UpgradeLevel = UpgradeLevel.LEVEL1;
            Cost = SharedDef.HEAL_START_COST;
            this.mgr = mgr;
            this.owner = owner;
        }

        public void Heal()
        {
            if (owner.Data.Gold >= Cost)
            {
                owner.ChangeBaseIntegrity(SharedDef.HEAL_AMOUNT, true);

                Cost *= SharedDef.HEAL_MULTIPLY_COEF;

                SendMessageWithHeal();
            }
        }

        private void SendMessageWithHeal()
        {
            NetOutgoingMessage message = mgr.CreateNetMessage();
            message.Write((int)PacketType.PLAYER_HEAL);
            message.Write((int)owner.GetId());
            message.Write((int)owner.GetBaseIntegrity());

            mgr.SendMessage(message);
        }
    }
}
