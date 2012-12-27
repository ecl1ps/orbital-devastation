using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Orbit.Core.Scene.Entities;
using Orbit.Core.Scene.Entities.Implementations;
using System.Windows;
using Orbit.Core.Scene.Controls.Collisions;
using Orbit.Core.Client.GameStates;
using Orbit.Core.Players;
using Lidgren.Network;


namespace Orbit.Core.Scene.Controls.Implementations
{
    public class HookControl : Control, ICollisionReactionControl
    {
        public int Speed { get; set; }
        public int Lenght { get; set; }
        public Vector Origin { get; set; }
        public Vector HitVector { get; set; } //neposilano
        public bool Returning { get; set; } // neposilano

        protected Hook hook;

        protected override void InitControl(ISceneObject me)
        {
            Returning = false;

            if (me is Hook)
                hook = me as Hook;
        }

        protected override void UpdateControl(float tpf)
        {
            if (hook.HasCaughtObject() || Returning)
            {
                MoveBackwards(tpf);

                if (IsAtStart())
                    hook.PulledCaughtObjectToBase();
            }
            else
            {
                MoveForward(tpf);
                if (IsAtEnd())
                    Returning = true;
            }

            if (hook is PowerHook)
                (hook as PowerHook).CaughtObjects.ForEach(obj => MoveWithObject(obj));
            else if(hook.CaughtObject != null)
                MoveWithObject(hook.CaughtObject);
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

        public virtual void DoCollideWith(ISceneObject other, float tpf)
        {
            if (hook.HasCaughtObject())
                return;

            if ((other is ICatchable && !Returning))
                CatchObject(other as ICatchable);
        }

        protected virtual void CatchObject(ICatchable caught)
        {
            if (caught == null)
                return;

            if (!caught.Enabled)
                return;

            if (!hook.Owner.IsCurrentPlayerOrBot())
                return;

            ProcessScore(caught);

            Vector hitVector = caught.Position - hook.Position;

            if (caught.Enabled)
                Catch(caught, hitVector);
        }

        protected virtual void ProcessScore(ICatchable caught)
        {
            if (caught is IContainsGold)
            {
                if (hook.Owner.IsCurrentPlayer())
                    hook.SceneMgr.FloatingTextMgr.AddFloatingText(ScoreDefines.HOOK_HIT, hook.Center, FloatingTextManager.TIME_LENGTH_1,
                        FloatingTextType.SCORE);
                hook.Owner.AddScoreAndShow(ScoreDefines.HOOK_HIT);
            }

            if (GetDistanceFromOriginPct() > 0.9)
            {
                hook.SceneMgr.FloatingTextMgr.AddFloatingText("Max range!", hook.Center,
                    FloatingTextManager.TIME_LENGTH_4, FloatingTextType.SCORE, FloatingTextManager.SIZE_BIG, false, true);
                hook.Owner.AddScoreAndShow(ScoreDefines.HOOK_CAUGHT_OBJECT_AFTER_90PCT_DISTANCE);
            }
        }

        public virtual void Catch(ICatchable caught, Vector hitVector)
        {
            if (caught is IDestroyable)
                (caught as IDestroyable).TakeDamage(0, hook);

            if (caught is UnstableAsteroid)
                return;

            hook.CaughtObject = caught;
            caught.Enabled = false;

            HitVector = hitVector;

            hook.OnCatch();
        }
    }
}
