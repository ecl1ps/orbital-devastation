using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Orbit.Core.Scene.Entities;
using Orbit.Core.Client;
using Orbit.Core.Helpers;
using System.Windows.Media;
using Orbit.Core.Scene.Entities.Implementations;

namespace Orbit.Core.Scene.Controls.Implementations
{
    struct MiningObject
    {
        public ISceneObject obj { get; set; }
        public SolidLine miningLine { get; set; }

        public MiningObject(ISceneObject obj, SolidLine line)
        {
            this.obj = obj;
            this.miningLine = line;
        }
    }

    public class MiningBotControl : Control
    {
        private SceneMgr sceneMgr;
        private List<MiningObject> currentlyMining;

        public override void InitControl(ISceneObject me)
        {
            sceneMgr = me.SceneMgr;
            currentlyMining = new List<MiningObject>();
        }

        public override void UpdateControl(float tpf)
        {
            CheckCollisions();
            MineObjects(tpf);
        }

        private void MineObjects(float tpf)
        {
        }

        private void CheckCollisions()
        {
            List<ISceneObject> colliding = new List<ISceneObject>();

            foreach (ISceneObject obj in sceneMgr.GetSceneObjects())
            {
                if ((obj.Position - me.Position).LengthSquared < SharedDef.SPECTATOR_MINING_RADIUS)
                {
                    if (!IsPresent(obj))
                        InitNewMining(obj);
                }
            }

            foreach (MiningObject obj in currentlyMining)
            {
                if (!IsPresent(obj, colliding))
                    StopMining(obj);
            }
        }

        private bool IsPresent(MiningObject mined, List<ISceneObject> objects)
        {
            foreach (ISceneObject obj in objects)
            {
                if (mined.obj.Equals(obj))
                    return true;
            }

            return false;
        }

        private bool IsPresent(ISceneObject obj)
        {
            foreach (MiningObject mined in currentlyMining)
            {
                if (mined.obj.Equals(obj))
                    return true;
            }

            return false;
        }

        private void InitNewMining(ISceneObject obj) 
        {
            StretchingLineControl control = new StretchingLineControl();
            control.FirstObj = me;
            control.SecondObj = obj;

            SolidLine line = new SimpleLine(sceneMgr, me.Position.ToPoint(), obj.Position.ToPoint(), Colors.CadetBlue, 2);
            line.AddControl(control);

            sceneMgr.DelayedAttachToScene(line);
            currentlyMining.Add(new MiningObject(obj, line));
        }

        private void StopMining(MiningObject obj)
        {
            currentlyMining.Remove(obj);
            obj.miningLine.DoRemoveMe();
        }
    }
}
