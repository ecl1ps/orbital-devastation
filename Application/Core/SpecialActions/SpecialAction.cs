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

        public SpecialAction(SceneMgr mgr, Player owner)
        {
            Owner = owner;
            SceneMgr = mgr;
        }

        public abstract void StartAction();

        public abstract bool IsReady();

        public void SendActionExecuted()
        {
            NetOutgoingMessage msg = SceneMgr.CreateNetMessage();
            //TODO
        }
    }
}
