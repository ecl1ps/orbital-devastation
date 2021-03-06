﻿using System;
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
using System.Windows;

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
            this.CastingTime = 0;
            this.Range = new Range();
            this.CastingColor = Colors.MediumTurquoise;
        }

        public override int RangeCount
        {
            get
            {
                return 0;
            }
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

        public override bool IsExact()
        {
            return false;
        }

        public override void StartAction(List<Asteroid> afflicted, bool exact)
        {
            int radius = 100;
            float lifeTime = 2;
            Color color = new Color();
            color.A = 35;
            color.R = 80;
            color.G = 255;
            color.B = 80;
            field = SceneObjectFactory.CreateSphereField(SceneMgr, Owner.Device.Center - new Vector(radius, radius), radius, color);
            
            StaticFieldControl control = new StaticFieldControl();
            control.Force = 140;
            control.LifeTime = lifeTime;
            control.Radius = radius;

            RippleEffectControl rippleControl = new RippleEffectControl();
            rippleControl.Speed = 15;
            
            Owner.Device.AddControl(control);
            field.AddControl(rippleControl);
            field.AddControl(new LimitedLifeControl(lifeTime));
            field.AddControl(new CenterCloneControl(Owner.Device));
            
            SceneMgr.DelayedAttachToScene(field);
        }
    }
}
