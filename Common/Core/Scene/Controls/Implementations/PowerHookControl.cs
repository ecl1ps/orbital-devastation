using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Orbit.Core.Scene.Entities.Implementations;
using Orbit.Core.Scene.Entities;
using System.Windows;
using Orbit.Core.Helpers;
using Lidgren.Network;

namespace Orbit.Core.Scene.Controls.Implementations
{
    public class PowerHookControl : HookControl
    {
        public bool ForcePullUsed { get; set; }

        protected override void InitControl(ISceneObject me)
        {
            base.InitControl(me);
            ForcePullUsed = false;
        }

        public override void DoCollideWith(ISceneObject other, float tpf)
        {
            if (HasFullCapacity() && !other.HasControlOfType<FollowingControl>())
                return;

            if (!(other is ICatchable))
                return;

            TryToCatchCollidedObject(other as ICatchable);
        }

        protected override void TryToCatchCollidedObject(ICatchable caught)
        {
            base.TryToCatchCollidedObject(caught);

            if (caughtObjects.Count == 1)
                Returning = true;
        }

        protected override void AddGoldToOwner(int gold)
        {
            base.AddGoldToOwner((int)(gold * 1.5));
        }

        protected override bool HasFullCapacity()
        {
            return caughtObjects.Count == hook.Owner.Data.HookMaxCatchedObjCount;
        }

        public void EmitForcePullField()
        {
            if (ForcePullUsed)
                return;

            ForcePullUsed = true;

            me.GetControlOfType<HighlightingControl>().Enabled = false;

            List<ISceneObject> nearbyObjects = me.FindNearbyObjects<ISceneObject>(hook.Owner.Data.HookActivePullReachDistance);
            IOrderedEnumerable<ISceneObject> ordered = nearbyObjects.OrderBy(o => (me.Center - o.Center).Length());

            List<ISceneObject> pulledObjects = new List<ISceneObject>();

            int pulledWeight = 0;
            foreach (ISceneObject o in ordered)
            {
                if (!(o is Asteroid) && !(o is StatPowerUp))
                    continue;

                if (o is Asteroid)
                    pulledWeight += (o as Asteroid).Radius;
                else
                    pulledWeight += 10; // pro powerupy

                if (pulledWeight > hook.Owner.Data.HookActivePullableWeight)
                    break;

                pulledObjects.Add(o);
                StartPullingObject(o);
            }

            SendInfoAboutPull(pulledObjects);
            ShowForceFieldEffect();
        }

        private void SendInfoAboutPull(List<ISceneObject> pulledObjects)
        {
            NetOutgoingMessage msg = me.SceneMgr.CreateNetMessage();
            msg.Write((int)PacketType.HOOK_FORCE_PULL);
            msg.Write(me.Id);
            msg.Write(pulledObjects.Count);
            foreach (ISceneObject o in pulledObjects)
                msg.Write(o.Id);
            me.SceneMgr.SendMessage(msg);
        }

        public void ShowForceFieldEffect()
        {
            /*
            ForcePullField f = new ForcePullField(me.SceneMgr, IdMgr.GetNewId(hook.Owner.GetId()));
            f.Radius = (int)hook.Owner.Data.HookActivePullReachDistance;
            f.Position = new Vector2(me.Center.X - f.Radius, me.Center.Y - f.Radius);
            f.HeavyWeightGeometry = HeavyweightGeometryFactory.CreateForceField(f);

            f.AddControl(new CenterCloneControl(me));
            float life = 0.3f; // seconds
            f.AddControl(new LimitedLifeControl(life));
            f.AddControl(new ShrinkingControl(0, life));
            
            me.SceneMgr.DelayedAttachToScene(f);*/
        }

        public void StartPullingObject(ISceneObject o)
        {
            IMovementControl c = o.GetControlOfType<IMovementControl>();
            if (c != null)
                c.Enabled = false;

            FollowingControl follow = new FollowingControl(me);
            follow.Speed = Speed * 2;

            o.AddControl(follow);
        }
    }
}
