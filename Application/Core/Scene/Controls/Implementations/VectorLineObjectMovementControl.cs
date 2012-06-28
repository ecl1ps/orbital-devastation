using System;
using System.Windows;
using Orbit.Core.Scene.Entities;
using Orbit.Core.Scene.Entities.Implementations;

namespace Orbit.Core.Scene.Controls.Implementations
{
    public class VectorLineObjectMovementControl : Control
    {
        private VectorLine meLine;
        public ISceneObject Parent { get; set; }

        public override void InitControl(ISceneObject obj)
        {
            meLine = obj as VectorLine;
        }

        public override void UpdateControl(float tpf)
        {
            if (meLine == null || Parent == null)
                return;

            if (Parent is Sphere)
                meLine.Position = (Parent as Sphere).Center;
            else
                meLine.Position = Parent.Position;
            meLine.Direction = (Parent as IMovable).Direction;
        }
    }
}
