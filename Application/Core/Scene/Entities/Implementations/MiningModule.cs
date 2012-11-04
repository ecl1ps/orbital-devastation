using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Orbit.Core.Client;
using Orbit.Core.Scene.Controls.Implementations;
using System.Windows.Media;
using System.Windows.Controls;
using Lidgren.Network;
using Orbit.Core.Players;
using Orbit.Core.Scene.Controls;
using System.Windows;

namespace Orbit.Core.Scene.Entities.Implementations
{
    public class MiningModule : Sphere, IRotable, IDestroyable, IHasHp
    {
        public float Rotation { get; set; }
        public float Hp { get; set; }

        public Player Owner { get; set; }

        public MiningModule(SceneMgr mgr, Player owner)
            : base(mgr)
        {
            this.Owner = owner;

            Hp = SharedDef.SPECTATOR_MAX_HP;
            HasPositionInCenter = false;
        }

        protected override void UpdateGeometricState()
        {
            base.UpdateGeometricState();
            geometryElement.RenderTransform = new RotateTransform(Rotation);
        }

        public override bool IsOnScreen(Size screenSize)
        {
            //i dont want to be destroyed when moving off screen
            return true;
        }

        public void TakeDamage(int damage, ISceneObject from)
        {
            GetControlsOfType<IDamageControl>().ForEach(control => control.ProccessDamage(damage, from));
        }

        public void RefillHp()
        {
            Hp = SharedDef.SPECTATOR_MAX_HP;
        }
    }
}
