using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Orbit.Core.Scene.Controls.Implementations;
using Orbit.Core.Client;
using Orbit.Core.Scene.Entities;
using Lidgren.Network;
using Orbit.Core.Client.GameStates;

namespace Orbit.Core.SpecialActions.Spectator
{
    class AsteroidDamage : SpectatorAction
    {
        public AsteroidDamage(SceneMgr mgr, Players.Player owner)
            : base(mgr, owner)
        {
            Name = "Asteroid damage";
            Type = SpecialActionType.ASTEROID_DAMAGE;
            ImageSource = "pack://application:,,,/resources/images/icons/asteroid-damage-icon.png";
            this.control = control;
            
            //nastavime parametry
            this.CoolDown = 1; //sekundy
            this.normal = 2;
            this.gold = 0;

            this.limit = Limit.BOTTOM_LIMIT;
        }

        public override void StartAction()
        {
            if (!control.Enabled)
                return;

            base.StartAction();

            List<IDestroyable> temp = new List<IDestroyable>();

            NetOutgoingMessage msg = SceneMgr.CreateNetMessage();
            msg.Write((int)PacketType.OBJECTS_TAKE_DAMAGE);

            foreach (MiningObject afflicted in control.currentlyMining)
            {
                if (afflicted.Obj is IDestroyable)
                {
                    temp.Add(afflicted.Obj as IDestroyable);
                    (afflicted.Obj as IDestroyable).TakeDamage(SharedDef.SPECTATOR_DAMAGE, null);
                    SceneMgr.FloatingTextMgr.AddFloatingText(SharedDef.SPECTATOR_DAMAGE, afflicted.Obj.Position, FloatingTextManager.TIME_LENGTH_3, FloatingTextType.DAMAGE); 
                }
            }

            msg.Write(Owner.GetId());
            msg.Write(temp.Count);
            msg.Write(SharedDef.SPECTATOR_DAMAGE);
            temp.ForEach(obj => msg.Write(obj.Id));

            SceneMgr.SendMessage(msg);
        }
    }
}
