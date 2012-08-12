using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Orbit.Core.Scene.Entities;
using Orbit.Core.Client;
using Orbit.Core.Helpers;
using System.Windows.Media;
using Orbit.Core.Scene.Entities.Implementations;
using System.Windows;

namespace Orbit.Core.Scene.Controls.Implementations
{
    struct MiningObject
    {
        public IContainsGold Obj { get; set; }
        public SolidLine MiningLine { get; set; }

        public MiningObject(IContainsGold obj, SolidLine line) : this()
        {
            Obj = obj;
            MiningLine = line;
        }
    }

    public class MiningModuleControl : Control
    {
        private SceneMgr sceneMgr;
        private List<MiningObject> currentlyMining;
        private float farmedGold;
        private float lastGoldPerSec = -1;

        public Players.Player Owner { get; set; }

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
            float goldPerSec = 0;
            foreach (MiningObject mined in currentlyMining)
                goldPerSec += mined.Obj.Gold;

            farmedGold += goldPerSec * SharedDef.SPECTATOR_GOLD_MULTIPLY * tpf;

            while (farmedGold > 1)
            {
                farmedGold -= 1;
                Owner.Data.Gold += 1;
            }

            me.SceneMgr.ShowStatusText(3, Owner.Data.Gold.ToString() + " + " + goldPerSec.ToString("0.##") + "/s");
        }

        private void CheckCollisions()
        {
            List<ISceneObject> colliding = new List<ISceneObject>();

            foreach (ISceneObject obj in sceneMgr.GetSceneObjects(typeof(Asteroid)))
            {
                if (!(obj is IContainsGold))
                    continue;

                Vector center = (obj is Sphere) ? (obj as Sphere).Center : obj.Position;

                if ((center - me.Position).Length < SharedDef.SPECTATOR_MINING_RADIUS)
                {
                    colliding.Add(obj);

                    if (!IsPresent(obj))
                        InitNewMining(obj as IContainsGold);
                }
            }

            for (int i = currentlyMining.Count - 1; i >= 0; i--)
            {
                if (!IsPresent(currentlyMining[i], colliding))
                    StopMining(currentlyMining[i]);
            }
        }

        private bool IsPresent(MiningObject mined, List<ISceneObject> objects)
        {
            foreach (ISceneObject obj in objects)
            {
                if (mined.Obj.Id == obj.Id)
                    return true;
            }

            return false;
        }

        private bool IsPresent(ISceneObject obj)
        {
            foreach (MiningObject mined in currentlyMining)
            {
                if (mined.Obj.Id == obj.Id)
                    return true;
            }

            return false;
        }

        private void InitNewMining(IContainsGold obj) 
        {
            StretchingLineControl control = new StretchingLineControl();
            control.FirstObj = me;
            control.SecondObj = obj;

            SolidLine line = new SimpleLine(sceneMgr, me.Position.ToPoint(), obj.Position.ToPoint(), Colors.Black, 1);
            line.AddControl(control);

            sceneMgr.DelayedAttachToScene(line);
            currentlyMining.Add(new MiningObject(obj, line));
        }

        private void StopMining(MiningObject obj)
        {
            obj.MiningLine.DoRemoveMe();
            obj.MiningLine.Dead = true;
            currentlyMining.Remove(obj);
        }
    }
}
