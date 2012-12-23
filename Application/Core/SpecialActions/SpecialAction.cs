using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Orbit.Core.Players;
using Orbit.Core.Client;
using Lidgren.Network;

namespace Orbit.Core.SpecialActions
{
    public abstract class SpecialAction : ISpecialAction
    {
        public Player Owner { get; set; }
        public SceneMgr SceneMgr { get; set; }
        public String Name { get; set; }
        public String ImageSource { get; set; }
        public SpecialActionType Type { get; set; }
        public float Cost { get; set; }
        private float timer = 0;
        public float CoolDown { get; set; }

        public SpecialAction(SceneMgr mgr, Player owner)
        {
            Owner = owner;
            SceneMgr = mgr;
        }

        public virtual void StartAction()
        {
            Owner.AddGoldAndShow((int) -Cost);
            timer = CoolDown;
        }

        public abstract bool IsReady();

        public void SendActionExecuted()
        {
            NetOutgoingMessage msg = SceneMgr.CreateNetMessage();
            //TODO
        }

        public void Update(float tpf)
        {
            if (timer > 0)
            {
                timer -= tpf;
                if (timer < 0)
                    timer = 0;
            }
        }

        protected bool isOnCoolDown()
        {
            return timer == 0;
        }
    }
}
