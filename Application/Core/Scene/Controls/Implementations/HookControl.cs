﻿using System;
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

        private Hook hook;

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
                    MoveWithObject(hook.GoldObject);

                if (IsAtStart())
                    hook.AddGoldToPlayer();
            }
            else
            {
                MoveForward(tpf);
                if (IsAtEnd())
                    Returning = true;
            }
        }

        private void MoveWithObject(IContainsGold obj)
        {
            obj.Position = hook.Position + HitVector;
        }

        private void MoveForward(float tpf)
        {
            hook.Position += (hook.Direction * Speed * tpf);
        }

        private void MoveBackwards(float tpf)
        {
            hook.Position -= (hook.Direction * Speed * tpf);
        }

        private bool IsAtEnd()
        {
            return GetDistanceFromOrigin() > Lenght;
        }

        private bool IsAtStart()
        {
            return GetDistanceFromOrigin() < 50;
        }

        private double GetDistanceFromOrigin()
        {
            return (Origin - hook.Position).Length;
        }

        public void CaughtObject()
        {
            HitVector = hook.GoldObject.Position - hook.Position;
        }
    }
}