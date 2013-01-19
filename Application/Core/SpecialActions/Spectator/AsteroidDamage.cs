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
using System.Windows.Media;

namespace Orbit.Core.SpecialActions.Spectator
{
    class AsteroidDamage : SpectatorAction
    {
        protected float exactBonus;

        public AsteroidDamage(SceneMgr mgr, Players.Player owner, params ISpectatorAction[] actions)
            : base(mgr, owner, actions)
        {
            Name = "Asteroid damage";
            Type = SpecialActionType.ASTEROID_DAMAGE;
            ImageSource = "pack://application:,,,/resources/images/icons/asteroid-damage-icon.png";
            
            //nastavime parametry
            this.Cooldown = 3; //sekundy
            this.CastingTime = 0.5f;
            this.CastingColor = Colors.Red;
            this.Range = new Range(3);
            this.exactBonus = 1.2f;
        }

        public override void StartAction(List<Asteroid> afflicted, bool exact)
        {
            List<IDestroyable> temp = new List<IDestroyable>();

            NetOutgoingMessage msg = SceneMgr.CreateNetMessage();
            msg.Write((int)PacketType.OBJECTS_TAKE_DAMAGE);

            int damage = (int) (exact ? SharedDef.SPECTATOR_DAMAGE * exactBonus : SharedDef.SPECTATOR_DAMAGE);
            foreach (Asteroid ast in afflicted)
            {
                temp.Add(ast);
                ast.TakeDamage(damage, Owner.Device);
                SceneMgr.FloatingTextMgr.AddFloatingText(damage, ast.Position, FloatingTextManager.TIME_LENGTH_3, FloatingTextType.DAMAGE);
            }

            msg.Write(Owner.GetId());
            msg.Write(temp.Count);
            msg.Write(damage);
            temp.ForEach(obj => msg.Write(obj.Id));

            SceneMgr.SendMessage(msg);
        }
    }
}
