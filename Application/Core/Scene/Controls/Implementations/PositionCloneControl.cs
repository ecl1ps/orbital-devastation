using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using Orbit.Core.Scene.Entities;

namespace Orbit.Core.Scene.Controls.Implementations
{
    public class PositionCloneControl : Control
    {
        public Vector Offset { get; set; }

        private ISceneObject toFollow;
        private bool delayed;

        public PositionCloneControl(ISceneObject toFollow, bool delayed = false)
        {
            Offset = new Vector(0, 0);
            this.toFollow = toFollow;
            this.delayed = delayed;
        }

        protected override void InitControl(ISceneObject me)
        {
            Move();
        }

        protected override void UpdateControl(float tpf)
        {
            if (delayed)
                return;

            Move();
        }

        public override void AfterUpdate(float tpf)
        {
            if (!delayed)
                return;

            Move();
        }

        private void Move()
        {
            me.Position = toFollow.Position - Offset;
        }
    }
}
