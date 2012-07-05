using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Orbit.Core.Players;
using Orbit.Core.Client;

namespace Orbit.Core.SpecialActions
{
    public class BrutalGravity : SpecialAction
    {
        public BrutalGravity(SceneMgr mgr, Player owner)
        {
            SceneMgr = mgr;
            Owner = owner;
            Name = "Brutal Gravity";
            Type = SpecialActionType.BRUTAL_GRAVITY;
        }

        public override void StartAction()
        {
            SceneMgr.LevelEnv.ChangeGravity(SharedDef.GRAVITY * 5, 2);
            SendActionExecuted();
        }

        public override bool IsReady()
        {
            return true;
        }
    }
}
