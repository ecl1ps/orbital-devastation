using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Orbit.Core.Client;
using Orbit.Core.Scene.Entities.Implementations;
using Orbit.Core.Scene.Controls.Implementations;
using Orbit.Core.Helpers;
using System.Windows.Media;
using Orbit.Core.Scene.Entities.Implementations.HeavyWeight;
using Lidgren.Network;

namespace Orbit.Core.SpecialActions.Spectator
{
    class StaticField : SpectatorAction
    {
        private SphereField field = null;

        public StaticField(SceneMgr mgr, Players.Player owner, params ISpectatorAction[] actions)
            : base(mgr, owner, actions)
        {
            Name = "Static Field";
            Type = SpecialActionType.STATIC_FIELD;
            ImageSource = "pack://application:,,,/resources/images/icons/static-field-action.png";
            
            //nastavime parametry
            this.Cooldown = 10; //sekundy
            this.Range = new RangeGroup(new Range());
        }

        public override bool IsReady()
        {
            return base.IsReady() && (field == null || field.Dead);
        }

        public override void Update(float tpf)
        {
            base.Update(tpf);
            if (field != null && !field.Dead)
                this.RemainingCooldown = Cooldown;
        }

        protected override void StartAction(List<Asteroid> afflicted)
        {
            Color color = new Color();
            color.A = 35;
            color.R = 80;
            color.G = 255;
            color.B = 80;
            field = SceneObjectFactory.CreateSphereField(SceneMgr, Owner.Device.Position, 200, color);
            
            StaticFieldControl control = new StaticFieldControl();
            control.Force = 100;
            control.LifeTime = 5;
            control.Radius = 200;

            RippleEffectControl rippleControl = new RippleEffectControl();
            rippleControl.Speed = 15;
            
            Owner.Device.AddControl(control);
            field.AddControl(rippleControl);
            field.AddControl(new LimitedLifeControl(5));
            field.AddControl(new PositionCloneControl(Owner.Device));

            SceneMgr.DelayedAttachToScene(field);

            NetOutgoingMessage msg = SceneMgr.CreateNetMessage();
            msg.Write((int)PacketType.STATIC_FIELD_START);

            msg.Write(Owner.GetId());
            msg.Write(color);
            msg.Write(control.Force);
            msg.Write(control.LifeTime);
            msg.Write(control.Radius);
            msg.Write(rippleControl.Speed);

            SceneMgr.SendMessage(msg);
        }
    }
}
