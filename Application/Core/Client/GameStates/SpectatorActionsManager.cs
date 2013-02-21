using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Orbit.Core.SpecialActions.Spectator;
using Orbit.Core.Scene.Entities;
using Orbit.Core.Scene.Entities.Implementations;
using Orbit.Core.Scene.Controls.Implementations;
using System.Windows.Media;
using Orbit.Core.Helpers;
using Orbit.Core.Scene.Controls;
using System.Windows.Media.Effects;
using Orbit.Core.Scene.Entities.HeavyWeight;
using Lidgren.Network;
using Orbit.Core.Players;

namespace Orbit.Core.Client.GameStates
{
    public class ActionObjects 
    {
        public SpectatorAction action;
        public List<Asteroid> targets;
        public bool exact;

        public ActionObjects(SpectatorAction action, List<Asteroid> objects, bool exact)
        {
            this.action = action;
            this.targets = objects;
            this.exact = exact;
        }
    }

    public class SpectatorActionsManager : IGameState
    {
        private Queue<ActionObjects> actions;

        private ActionObjects action = null;
        private float time = 0;
        private bool lockedDevice = false;
        private Player lastOwner = null;

        public SpectatorActionsManager()
        {
            actions = new Queue<ActionObjects>();
        }

        public void Update(float tpf)
        {
            if (time <= 0 && action != null)
            {
                CastAction();
                return;
            }

            if (action == null && actions.Count > 0)
                LoadAnotherAction();

            if (action != null && !lockedDevice)
                EnableDevice(false);
            else if (lockedDevice && action == null)
                EnableDevice(true);

            time -= tpf;
        }

        private void EnableDevice(bool enable)
        {
            if(!enable)
                lastOwner = action.action.Owner;

            Control c = lastOwner.Device.GetControlOfType<MiningModuleControl>();
            if (c != null)
                c.Enabled = enable;

            lockedDevice = !enable;
        }

        private void CastAction()
        {
            action.action.StartAction(action.targets, action.exact);
            LoadAnotherAction();
        }

        private void SendActionStart(ISpectatorAction action)
        {
            NetOutgoingMessage msg = action.Owner.SceneMgr.CreateNetMessage();
            msg.Write((int)PacketType.SPECTATOR_ACTION_START);

            msg.Write(action.Owner.GetId());
            msg.Write(action.Name);

            action.Owner.SceneMgr.SendMessage(msg);
        }

        private void LoadAnotherAction()
        {
            if (actions.Count > 0)
            {
                action = actions.Dequeue();
                if (action.action.CastingTime == 0)
                    CastAction();
                else
                    action.targets.ForEach(ast => StartAnimation(ast, action.action.CastingTime, action.action.CastingColor, action.action.TowardsMe));
            }
            else
                action = null;
        }

        private void StartAnimation(Asteroid target, float time, Color color, bool towardsMe)
        {
            this.time = time;

            if (target == null)
                return;

            Line line = new Line(action.action.Owner.SceneMgr, IdMgr.GetNewId(action.action.Owner.GetId()), action.action.Owner.Device.Position, target.Position, color, 1);
            HeavyweightLine strong = new HeavyweightLine(action.action.SceneMgr, IdMgr.GetNewId(action.action.Owner.GetId()), action.action.Owner.Device.Position, target.Position, color, 2);

            //OrbitEllipse s = SceneObjectFactory.CreateOrbitEllipse(Owner.SceneMgr, Owner.Device.Position, 3, 3, color);

            StretchingLineControl c = new StretchingLineControl();
            TimeStretchingControl tc = new TimeStretchingControl();
            tc.Time = time;
            if (towardsMe)
            {
                c.SecondObj = target;
                c.FirstObj = action.action.Owner.Device;
                tc.FirstObj = target;
                tc.SecondObj = action.action.Owner.Device;
            }
            else
            {
                c.FirstObj = target;
                c.SecondObj = action.action.Owner.Device;
                tc.SecondObj = target;
                tc.FirstObj = action.action.Owner.Device;
            }

            //LineTravelingControl tc = new LineTravelingControl();
            //tc.TravellingTime = time;
            //tc.LineToFollow = line;

            line.AddControl(c);
            line.AddControl(new LimitedLifeControl(time));
            strong.AddControl(tc);
            strong.AddControl(new LimitedLifeControl(time));

            action.action.Owner.SceneMgr.DelayedAttachToScene(line);
            action.action.Owner.SceneMgr.DelayedAttachToScene(strong);
        }

        public void ScheduleAction(SpectatorAction action, List<Asteroid> targets, bool exact, bool send = true)
        {
            ActionObjects a = new ActionObjects(action, targets, exact);
            actions.Enqueue(a);

            if (send)
                SendActionScheduleMessage(action, targets, exact);
        }

        public void SendActionScheduleMessage(SpectatorAction action, List<Asteroid> targets, bool exact)
        {
            NetOutgoingMessage msg = action.Owner.SceneMgr.CreateNetMessage();
            msg.Write((int)PacketType.SCHEDULE_SPECTATOR_ACTION);
            msg.Write(action.Owner.GetId());
            msg.Write(exact);
            msg.WriteSpecialAction(action);
            msg.Write(targets.Count);
            targets.ForEach(a => msg.Write(a.Id));

            action.Owner.SceneMgr.SendMessage(msg);
        }

        public void ReceiveActionScheduleMessage(NetIncomingMessage msg, Player owner)
        {
            bool exact = msg.ReadBoolean();
            SpectatorAction action = msg.ReadSpecialAction(owner.SceneMgr, owner) as SpectatorAction;

            int count = msg.ReadInt32();
            List<Asteroid> temp = new List<Asteroid>();
            for (int i = 0; i < count; i++)
            {
                Asteroid found = owner.SceneMgr.GetSceneObject(msg.ReadInt64()) as Asteroid;
                if (found != null)
                    temp.Add(found);
            }

            action.CastingTime -= ((msg.SenderConnection.AverageRoundtripTime / 2) / 1000);
            ScheduleAction(action, temp, exact, false);
        }

    }
}
