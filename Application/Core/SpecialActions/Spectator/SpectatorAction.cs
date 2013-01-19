using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Orbit.Core.Client;
using Orbit.Core.Scene.Controls.Implementations;
using Orbit.Core.Scene.Entities.Implementations;
using Orbit.Core.Client.GameStates;
using System.Windows.Media;
using Orbit.Core.Scene.Entities;
using Lidgren.Network;
using Orbit.Core.Scene.Controls.Health;

namespace Orbit.Core.SpecialActions.Spectator
{
    public abstract class SpectatorAction : SpecialAction, ISpectatorAction
    {
        protected MiningModuleControl control;
        protected bool exact = false;

        public Range Range { get; set; }
        public float CastingTime { get; set; }
        public Color CastingColor { get; set; }
        public bool TowardsMe { get; set; }

        public SpectatorAction(SceneMgr mgr, Players.Player owner, params ISpectatorAction[] actions)
            : base(mgr, owner, actions)
        {
            this.control = owner.Device.GetControlOfType<MiningModuleControl>();
            BackgroundColor = Colors.Bisque;
            TowardsMe = false;
        }

        public override bool IsReady()
        {
            return !IsOnCooldown() && HasEnergy() && Range.GetValidGroup(control.CurrentlyMining).Count > 0;
        }

        public virtual bool IsExact()
        {
            int rangeCount = Range.GetValidGroup(control.CurrentlyMining).Count;
            return control.CurrentlyMining.Count == rangeCount && rangeCount == (Range.Maximum - Range.Minimum);
        }

        private bool HasEnergy()
        {
            HpControl control = Owner.Device.GetControlOfType<HpControl>();

            if(control != null) 
                return control.Hp == control.MaxHp;

            return true;
        }


        public override void StartAction()
        {
            if (!control.Enabled)
                return;

            base.StartAction();

            List<Asteroid> group = Range.GetValidGroup(control.CurrentlyMining);
            bool exact = IsExact();

            Owner.SpectatorActionMgr.ScheduleAction(this, group, exact);
            SendActionStart();

            if (exact)
            {
                Owner.Statistics.PoweredActions++;
                SceneMgr.FloatingTextMgr.AddFloatingText("Powered action!", control.Position,
                    FloatingTextManager.TIME_LENGTH_3, FloatingTextType.SYSTEM, FloatingTextManager.SIZE_MEDIUM,
                    true, true);
            }
        }

        private void SendActionStart()
        {
            NetOutgoingMessage msg = SceneMgr.CreateNetMessage();
            msg.Write((int)PacketType.SPECTATOR_ACTION_START);

            msg.Write(Owner.GetId());
            msg.Write(Name);

            SceneMgr.SendMessage(msg);
        }

        public abstract void StartAction(List<Asteroid> afflicted, bool exact);
   
    }
}
