using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Orbit.Core.Scene.Entities;
using System.Windows;

namespace Orbit.Core.Scene.Controls.Implementations
{
    class ControlableDeviceControl : Control, IControledDevice
    {
        public bool IsMovingDown { get; set; }
        public bool IsMovingTop { get; set; }
        public bool IsMovingLeft { get; set; }
        public bool IsMovingRight { get; set; }

        public override void InitControl(ISceneObject me)
        {
            IsMovingDown = false;
            IsMovingTop = false;
            IsMovingLeft = false;
            IsMovingRight = false;
        }

        public override void UpdateControl(float tpf)
        {
            Vector botVector = new Vector(0, SharedDef.SPECTATOR_MODULE_SPEED * tpf);
            Vector rightVector = new Vector(SharedDef.SPECTATOR_MODULE_SPEED * tpf, 0);

            if(IsMovingTop && me.Position.Y > 0)
                me.Position -= botVector;
            if (IsMovingDown && me.Position.Y < SharedDef.VIEW_PORT_SIZE.Height)
                me.Position += botVector;
            if (IsMovingLeft && me.Position.X > 0)
                me.Position -= rightVector;
            if (IsMovingRight && me.Position.Y < SharedDef.VIEW_PORT_SIZE.Width)
                me.Position += rightVector;
        }
    }
}
