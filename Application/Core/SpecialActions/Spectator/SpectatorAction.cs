using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Orbit.Core.Client;
using Orbit.Core.Scene.Controls.Implementations;
using Orbit.Core.Scene.Entities.Implementations;
using Orbit.Core.Client.GameStates;

namespace Orbit.Core.SpecialActions.Spectator
{
    public abstract class SpectatorAction : SpecialAction, ISpectatorAction
    {
        private const float LOCK_TIME = 2;

        protected MiningModuleControl control;

        public RangeGroup Normal { get; set; }
        public RangeGroup Gold { get; set; }

        public float CastingTime { get; set; }

        protected Boolean locked = false;
        protected List<MiningObject> lockedObjects = null;
        protected float lockedTime = 0;

        public SpectatorAction(SceneMgr mgr, Players.Player owner)
            : base(mgr, owner)
        {
            this.control = owner.Device.GetControlOfType<MiningModuleControl>();
        }

        public float Percentage
        {
            get { return ComputePercentage(); }
        }

        public override bool IsReady()
        {
            return locked;
        }

        private float ComputePercentage() {
            if (Gold == null || Normal == null)
                throw new Exception("gold and normal ranges must be set");

            return Gold.ComputePercentage(control.currentlyMining) * Normal.ComputePercentage(control.currentlyMining);
        }

        public int ComputeMissing(RangeGroup range)
        {
            return range.ComputeMissing(control.currentlyMining);
        }

        public override void Update(float tpf)
        {
            base.Update(tpf);

            if (!locked && isReadyToBeLocked())
            {
                lockAction();
            } else if(locked) {
                lockedTime -= tpf;

                if (lockedTime < 0)
                    locked = false;
                else
                {
                    if (isReadyToBeLocked() && control.currentlyMining.Count > lockedObjects.Count)
                        lockedObjects = new List<MiningObject>(control.currentlyMining);
                }
            }
        }

        protected void lockAction() 
        {
            locked = true;
            lockedObjects = new List<MiningObject>(control.currentlyMining);
            lockedTime = LOCK_TIME;

            SceneMgr.FloatingTextMgr.AddFloatingText("LOCKED " + Name, control.Position, FloatingTextManager.TIME_LENGTH_2, FloatingTextType.SYSTEM, FloatingTextManager.SIZE_BIG, true);
        }

        private bool isReadyToBeLocked()
        {
            return !IsOnCooldown() && Percentage == 1;
        }
   
    }
}
