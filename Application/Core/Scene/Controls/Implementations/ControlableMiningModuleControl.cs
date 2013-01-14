using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Orbit.Core.Scene.Entities.Implementations;
using Orbit.Core.Scene.Entities;
using System.Windows;
using Orbit.Core.Players;
using Orbit.Core.Scene.Controls.Health.Implementations;

namespace Orbit.Core.Scene.Controls.Implementations
{
    public class ControlableMiningModuleControl : ControlableDeviceControl
    {
        private int leftTopOffset = 0;
        private int rightBottomOffset = 0;

        private bool initializedOffsets = false;

        /// <summary>
        /// je potreba inicializovat az v updatu, protoze HpBar (Arc) se vytvari a pridava az kdyz je Module hotovy
        /// </summary>
        private void TryInitOffsets()
        {
            if (initializedOffsets)
                return;

            HpBarControl c = me.GetControlOfType<HpBarControl>();
            if (c != null)
            {
                rightBottomOffset = (c.Bar as MiningModuleIntegrityBar).Radius * 2;

                leftTopOffset = rightBottomOffset / 2 - (me as MiningModule).Radius;

                initializedOffsets = true;
            }
        }

        /// <summary>
        /// MiningModule nemuze letat az k bazim (narazil by a vybuchnul ;)
        /// </summary>
        /// <param name="tpf"></param>
        protected override void UpdateControl(float tpf)
        {
            TryInitOffsets();

            Vector botVector = new Vector(0, SharedDef.SPECTATOR_MODULE_SPEED * tpf);
            Vector rightVector = new Vector(SharedDef.SPECTATOR_MODULE_SPEED * tpf, 0);

            if (IsMovingTop && (me.Position.Y - botVector.Y - leftTopOffset) > 0)
                me.Position -= botVector;
            if (IsMovingDown && (me.Position.Y + botVector.Y + rightBottomOffset) < PlayerBaseLocation.GetBaseLocation(PlayerPosition.LEFT).Top - 30)
                me.Position += botVector;
            if (IsMovingLeft && (me.Position.X - rightVector.X - leftTopOffset) > 0)
                me.Position -= rightVector;
            if (IsMovingRight && (me.Position.X + rightVector.X + rightBottomOffset) < SharedDef.VIEW_PORT_SIZE.Width)
                me.Position += rightVector;
        }
    }
}
