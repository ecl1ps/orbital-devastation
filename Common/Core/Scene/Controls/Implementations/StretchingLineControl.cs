using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Orbit.Core.Scene.Entities;

namespace Orbit.Core.Scene.Controls.Implementations
{
    public class StretchingLineControl : Control
    {
        private ISceneObject firstObj;
        public ISceneObject FirstObj {
            get
            {
                return firstObj;
            }
            set {
                firstObj = value;
                if (SecondObj != null)
                    Enabled = true;
            } 
        }
        private ISceneObject secondObj;
        public ISceneObject SecondObj {
            get
            {
                return secondObj;
            }
            set
            {
                secondObj = value;
                if (FirstObj != null)
                    Enabled = true;
            }
        }
        protected Line line;

        protected override void InitControl(ISceneObject me)
        {
            if (me is Line)
            {
                line = me as Line;
            }
            else
                throw new Exception("Stretching line control must be attached to solid line object");
        }

        protected override void UpdateControl(float tpf)
        {
            if (FirstObj == null || SecondObj == null) {
                Enabled = false;
                return;
            }

            if (FirstObj.Dead || SecondObj.Dead)
                me.DoRemoveMe();

            UpdateLine();
        }

        protected virtual void UpdateLine()
        {
            if (firstObj is Sphere)
                line.Start = (FirstObj as Sphere).Center;
            else
                line.Start = FirstObj.Position;

            if (SecondObj is Sphere)
                line.End = (SecondObj as Sphere).Center;
            else
                line.End = secondObj.Position;
        }
    }
}
