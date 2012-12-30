using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Orbit.Core.Scene.Controls.Implementations;
using Orbit.Core.Client;
using Orbit.Core.Scene.Entities;
using Lidgren.Network;
using Orbit.Core.Client.GameStates;
using Orbit.Core.Scene.Entities.Implementations;

namespace Orbit.Core.SpecialActions.Spectator
{
    class AsteroidDamage : SpectatorAction
    {
        public AsteroidDamage(SceneMgr mgr, Players.Player owner, params ISpectatorAction[] actions)
            : base(mgr, owner, actions)
        {
            Name = "Asteroid damage";
            Type = SpecialActionType.ASTEROID_DAMAGE;
            ImageSource = "pack://application:,,,/resources/images/icons/asteroid-damage-icon.png";
            
            //nastavime parametry
            this.Cooldown = 3; //sekundy
            this.Range = new RangeGroup(new Range());
        }

        protected override void StartAction(List<Asteroid> afflicted)
        {
            List<IDestroyable> temp = new List<IDestroyable>();

            NetOutgoingMessage msg = SceneMgr.CreateNetMessage();
            msg.Write((int)PacketType.OBJECTS_TAKE_DAMAGE);

            foreach (Asteroid ast in afflicted)
            {
                temp.Add(ast);
                ast.TakeDamage(SharedDef.SPECTATOR_DAMAGE, null);
                SceneMgr.FloatingTextMgr.AddFloatingText(SharedDef.SPECTATOR_DAMAGE, ast.Position, FloatingTextManager.TIME_LENGTH_3, FloatingTextType.DAMAGE);
            }

            msg.Write(Owner.GetId());
            msg.Write(temp.Count);
            msg.Write(SharedDef.SPECTATOR_DAMAGE);
            temp.ForEach(obj => msg.Write(obj.Id));

            SceneMgr.SendMessage(msg);
        }
    }
}
