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
using Orbit.Core.Helpers;
using Microsoft.Xna.Framework;

namespace Orbit.Core.Scene.Controls.Implementations
{
    public class HookControl : Control, ICollisionReactionControl
    {
        public int Speed { get; set; }
        public int Lenght { get; set; }
        public Vector2 Origin { get; set; }
        public Vector2 HitVector { get; set; } //neposilano
        public bool Returning { get; set; } // neposilano

        protected List<ICatchable> caughtObjects;
        protected Hook hook;

        private bool reportedStatistics = false;

        protected override void InitControl(ISceneObject me)
        {
            Returning = false;

            if (me is Hook)
                hook = me as Hook;

            caughtObjects = new List<ICatchable>();
        }

        protected override void UpdateControl(float tpf)
        {
            if (HasFullCapacity() || Returning)
            {
                MoveBackwards(tpf);

                if (IsAtStart())
                    ProcessObjectsPulledToBase();
            }
            else
            {
                MoveForward(tpf);
                if (IsAtEnd())
                    Returning = true;
            }

            caughtObjects.ForEach(obj => MoveWithObject(obj));
        }

        public virtual void DoCollideWith(ISceneObject other, float tpf)
        {
            if (HasFullCapacity())
                return;

            if (!(other is ICatchable))
                return;

            // nechyta kdyz se vraci
            if (Returning)
                return;

            TryToCatchCollidedObject(other as ICatchable);
        }

        protected virtual void TryToCatchCollidedObject(ICatchable caught)
        {
            if (caught == null)
                return;

            if (!caught.Enabled)
                return;

            if (!hook.Owner.IsCurrentPlayerOrBot())
                return;

            HitVector = (hook.Position - caught.Position) * 0.9f;

            CatchObject(caught, HitVector);

            SendHookHitMsg(caught as ISceneObject);

            if (caught is UnstableAsteroid)
                return;

            // za unstable se nedostava vubec zadne score
            ProcessScore(caught);

            if (!reportedStatistics)
            {
                reportedStatistics = true;
                (me as Hook).Owner.Statistics.HookHit++;
            }
        }

        /// <summary>
        /// tato metoda je volana kud po kolizi, pokud je vlastnikem soucasny hrac nebo zpravou, pokud neni
        /// </summary>
        public void CatchObject(ICatchable caught, Vector2 hitVector)
        {
            if (caught is IDestroyable)
                (caught as IDestroyable).TakeDamage(0, hook);

            if (caught is UnstableAsteroid)
                return;

            caughtObjects.Add(caught);
            caught.Enabled = false;

            // toto je potreba, protoze vektor muze prijit zpravou od jineho hrace
            HitVector = hitVector;
        }

        private void SendHookHitMsg(ISceneObject caught)
        {
            NetOutgoingMessage msg = me.SceneMgr.CreateNetMessage();
            msg.Write((int)PacketType.HOOK_HIT);
            msg.Write(me.Id);
            msg.Write(caught.Id);
            msg.Write(caught.Position);
            msg.Write(HitVector);
            me.SceneMgr.SendMessage(msg);
        }

        private void ProcessObjectsPulledToBase()
        {
            caughtObjects.ForEach(obj => ProccesObjectsJustPulledToBase(obj));
            me.DoRemoveMe();
        }

        private void ProccesObjectsJustPulledToBase(ICatchable caught)
        {
            if (caught != null && !caught.Dead)
            {
                if (caught is IContainsGold)
                {
                    me.SceneMgr.FloatingTextMgr.AddFloatingText((caught as IContainsGold).Gold, hook.Center,
                        FloatingTextManager.TIME_LENGTH_3, FloatingTextType.GOLD, FloatingTextManager.SIZE_BIG);
                    AddGoldToOwner((caught as IContainsGold).Gold);
                    caught.DoRemoveMe();
                }
                else
                {
                    caught.Enabled = true;
                    if (caught is IMovable)
                        (caught as IMovable).Direction = new Vector2(0, 1);
                }
            }

            me.DoRemoveMe();
        }

        private void ProcessScore(ICatchable caught)
        {
            if (caught is IContainsGold)
            {
                if (hook.Owner.IsCurrentPlayer())
                    hook.SceneMgr.FloatingTextMgr.AddFloatingText(ScoreDefines.HOOK_HIT, hook.Center, FloatingTextManager.TIME_LENGTH_1,
                        FloatingTextType.SCORE);
                hook.Owner.AddScoreAndShow(ScoreDefines.HOOK_HIT);
            }

            if (!reportedStatistics && GetDistanceFromOriginPct() > 0.9)
            {
                hook.SceneMgr.FloatingTextMgr.AddFloatingText(Strings.ft_score_hook_max, hook.Center,
                    FloatingTextManager.TIME_LENGTH_4, FloatingTextType.BONUS_SCORE, FloatingTextManager.SIZE_BIG, false, true);
                hook.Owner.AddScoreAndShow(ScoreDefines.HOOK_CAUGHT_OBJECT_AFTER_90PCT_DISTANCE);
            }
        }

        protected virtual void AddGoldToOwner(int gold)
        {
            hook.Owner.AddGoldAndShow(gold);
        }

        protected virtual bool HasFullCapacity()
        {
            return caughtObjects.Count > 0;
        }

        private void MoveWithObject(ICatchable obj)
        {
            obj.Position = hook.Position - HitVector;
        }

        private void MoveForward(float tpf)
        {
            hook.Position += (hook.Direction * Speed * tpf);
        }

        private void MoveBackwards(float tpf)
        {
            // jinak se obcas neco bugne a on leti do nekonecna
            Vector2 dirToBase = Origin + new Vector2(-hook.Radius, -hook.Radius) - hook.Position;
            dirToBase.Normalize();
            hook.Position += (dirToBase * Speed * tpf);
        }

        private bool IsAtEnd()
        {
            return GetDistanceFromOrigin() > Lenght || OutOfScreen();
        }

        private bool OutOfScreen()
        {
            return hook.Position.X < 0 || hook.Position.Y < 0 ||
                hook.Position.X > SharedDef.VIEW_PORT_SIZE.Width || hook.Position.Y > SharedDef.VIEW_PORT_SIZE.Height;
        }

        private bool IsAtStart()
        {
            return GetDistanceFromOrigin() < 15;
        }

        private double GetDistanceFromOrigin()
        {
            return (Origin - hook.Position).Length();
        }

        private double GetDistanceFromOriginPct()
        {
            return GetDistanceFromOrigin() / Lenght;
        }
    }
}
