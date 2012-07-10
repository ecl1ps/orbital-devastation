using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Orbit.Core.Scene.Entities;
using Orbit.Core.Scene.Entities.Implementations;
using System.Windows;


namespace Orbit.Core.Scene.Controls.Implementations
{
    public class HookControl : Control
    {
        public int Speed { get; set; }
        public int Lenght { get; set; }
        public Vector Origin { get; set; }
        public Vector HitVector { get; set; } //neposilano
        public bool Returning { get; set; } // neposilano

        protected Hook hook;

        public override void InitControl(ISceneObject me)
        {
            Returning = false;

            if (me is Hook)
                hook = me as Hook;
        }

        public override void UpdateControl(float tpf)
        {
            if (hook.HasCaughtObject() || Returning)
            {
                MoveBackwards(tpf);
                if (hook.HasCaughtObject())
                {
                    if (hook is PowerHook)
                        (hook as PowerHook).CaughtObjects.ForEach(obj => MoveWithObject(obj));
                    else
                        MoveWithObject(hook.CaughtObject);
                }

                if (IsAtStart())
                    hook.PulledCaughtObjectToBase();
            }
            else
            {
                MoveForward(tpf);
                if (IsAtEnd())
                    Returning = true;
            }
        }

        protected void MoveWithObject(ICatchable obj)
        {
            obj.Position = hook.Position + HitVector;
        }

        protected void MoveForward(float tpf)
        {
            hook.Position += (hook.Direction * Speed * tpf);
        }

        protected void MoveBackwards(float tpf)
        {
            // jinak se obcas neco bugne a on leti do nekonecna
            Vector dirToBase = Origin + new Vector(-hook.Radius, -hook.Radius) - hook.Position;
            dirToBase.Normalize();
            hook.Position += (dirToBase * Speed * tpf);
        }

        protected bool IsAtEnd()
        {
            return GetDistanceFromOrigin() > Lenght || OutOfScreen();
        }

        private bool OutOfScreen()
        {
            return hook.Position.X < 0 || hook.Position.Y < 0 ||
                hook.Position.X > SharedDef.VIEW_PORT_SIZE.Width || hook.Position.Y > SharedDef.VIEW_PORT_SIZE.Height;
        }

        protected bool IsAtStart()
        {
            return GetDistanceFromOrigin() < 15;
        }

        private double GetDistanceFromOrigin()
        {
            return (Origin - hook.Position).Length;
        }

        public double GetDistanceFromOriginPct()
        {
            return GetDistanceFromOrigin() / Lenght;
        }

        public void CaughtObject(Vector hitVector)
        {
            HitVector = hitVector;
        }
    }
}
