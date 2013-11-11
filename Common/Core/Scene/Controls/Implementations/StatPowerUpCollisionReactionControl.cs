using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Orbit.Core.Scene.Controls.Collisions;
using Orbit.Core.Scene.Entities;
using Orbit.Core.Scene.Entities.Implementations;

namespace Orbit.Core.Scene.Controls.Implementations
{
    public class StatPowerUpCollisionReactionControl : Control, ICollisionReactionControl
    {
        public void DoCollideWith(ISceneObject other, float tpf)
        {
            if (other is Base)
            {
                me.SceneMgr.StatsMgr.OnPlayerCaughtPowerUp((other as Base).Owner, (me as StatPowerUp).PowerUpType);
                me.DoRemoveMe();
            }
        }
    }
}
