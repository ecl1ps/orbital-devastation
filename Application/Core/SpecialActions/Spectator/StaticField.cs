using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Orbit.Core.Client;
using Orbit.Core.Scene.Entities.Implementations;
using Orbit.Core.Scene.Controls.Implementations;

namespace Orbit.Core.SpecialActions.Spectator
{
    class StaticField : SpectatorAction
    {
        public StaticField(SceneMgr mgr, Players.Player owner, params ISpectatorAction[] actions)
            : base(mgr, owner, actions)
        {
            Name = "Static Field";
            Type = SpecialActionType.STATIC_FIELD;
            ImageSource = "pack://application:,,,/resources/images/icons/asteroid-damage-icon.png";
            
            //nastavime parametry
            this.Cooldown = 10; //sekundy
            this.Range = new RangeGroup(new Range());
        }

        public override bool IsReady()
        {
            return base.IsReady() && Owner.Device.GetControlOfType<StaticFieldControl>() == null;
        }


        protected override void StartAction(List<Asteroid> afflicted)
        {
            StaticFieldControl control = new StaticFieldControl();
            control.Force = 100;
            control.LifeTime = 5;
            control.Radius = 200;

            Owner.Device.AddControl(control);
        }
    }
}
