using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Orbit.Core.Players;
using Orbit.Core.Scene.Entities.Implementations;
using Orbit.Core.Scene.Controls.Collisions;
using Orbit.Core.Scene.Controls.Health;
using Lidgren.Network;
using Orbit.Core.Client;
using System.Windows;
using Orbit.Core.Helpers;

namespace Orbit.Core.Scene.Controls.Health.Implementations
{
    public class BaseHealthControl : Control, IHpControl
    {
        private Base baze;

        public int Hp { get { return baze.Owner.GetBaseIntegrity(); } set { baze.Owner.SetBaseIntegrity(value); } }
        public int MaxHp { get { return baze.Owner.Data.MaxBaseIntegrity; } set { baze.Owner.Data.MaxBaseIntegrity = value; } }

        private Random rand;

        protected override void InitControl(Entities.ISceneObject me)
        {
            if (me is Base)
                baze = me as Base;

            this.rand = me.SceneMgr.GetRandomGenerator();
        }

        public void RefillHp()
        {
        }

        protected override void UpdateControl(float tpf)
        {
            base.UpdateControl(tpf);
        }
    }
}
