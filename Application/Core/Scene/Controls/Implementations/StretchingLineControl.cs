using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Orbit.Core.Scene.Entities;

namespace Orbit.Core.Scene.Controls.Implementations
{
    class StretchingLineControl : Control
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
        private SolidLine line;

        public override void InitControl(ISceneObject me)
        {
            if (me is SolidLine)
            {
                line = me as SolidLine;
            }
            else
                throw new Exception("Stretching line control must be attached to solid line object");
        }

        public override void UpdateControl(float tpf)
        {
            if(FirstObj == null || SecondObj == null) {
                Enabled = false;
                return;
            }

            if (FirstObj.Dead || SecondObj.Dead)
                me.DoRemoveMe();

            UpdateLine();
        }

        private void UpdateLine()
        {
            if (firstObj is Sphere)
                line.Start = (FirstObj as Sphere).Center.ToPoint();
            else
                line.Start = FirstObj.Position.ToPoint();

            if (SecondObj is Sphere)
                line.End = (SecondObj as Sphere).Center.ToPoint();
            else
                line.End = secondObj.Position.ToPoint();
        }
    }
}
