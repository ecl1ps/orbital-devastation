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

namespace Orbit.Core.Players
{
    public class ActionObjects 
    {
        public SpectatorAction action;
        public List<Asteroid> targets;

        public ActionObjects(SpectatorAction action, List<Asteroid> objects)
        {
            this.action = action;
            this.targets = objects;
        }
    }

    public class SpectatorActionsManager : IGameState
    {
        public Player Owner { get; set; }
        private Queue<ActionObjects> actions;

        private ActionObjects action = null;
        private bool lockedDevice = false;
        private float time = 0;

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
            else if (lockedDevice)
                EnableDevice(true);

            time -= tpf;
        }

        private void EnableDevice(bool enable)
        {
            Owner.Device.GetControlOfType<ControlableDeviceControl>().Enabled = enable;
            Control c = Owner.Device.GetControlOfType<MiningModuleControl>();
            if (c != null)
                c.Enabled = enable;

            lockedDevice = !enable;
        }

        private void CastAction()
        {
            action.action.StartAction(action.targets);
            LoadAnotherAction();
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
            Line line = new Line(Owner.SceneMgr, IdMgr.GetNewId(Owner.GetId()), Owner.Device.Position, target.Position, color, 1);
            HeavyweightLine strong = new HeavyweightLine(Owner.SceneMgr, IdMgr.GetNewId(Owner.GetId()), Owner.Device.Position, target.Position, color, 4);

            //OrbitEllipse s = SceneObjectFactory.CreateOrbitEllipse(Owner.SceneMgr, Owner.Device.Position, 3, 3, color);

            StretchingLineControl c = new StretchingLineControl();
            TimeStretchingControl tc = new TimeStretchingControl();
            tc.Time = time;
            if (towardsMe)
            {
                c.SecondObj = target;
                c.FirstObj = Owner.Device;
                tc.FirstObj = target;
                tc.SecondObj = Owner.Device;
            }
            else
            {
                c.FirstObj = target;
                c.SecondObj = Owner.Device;
                tc.SecondObj = target;
                tc.FirstObj = Owner.Device;
            }

            //LineTravelingControl tc = new LineTravelingControl();
            //tc.TravellingTime = time;
            //tc.LineToFollow = line;

            line.AddControl(c);
            line.AddControl(new LimitedLifeControl(time));
            strong.AddControl(tc);
            strong.AddControl(new LimitedLifeControl(time));

            Owner.SceneMgr.DelayedAttachToScene(line);
            Owner.SceneMgr.DelayedAttachToScene(strong);

            this.time = time;
        }

        public void ScheduleAction(SpectatorAction action, List<Asteroid> targets)
        {
            actions.Enqueue(new ActionObjects(action, targets));
        }

    }
}
