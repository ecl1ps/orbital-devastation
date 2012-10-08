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
    public struct MiningObject
    {
        public IContainsGold Obj { get; set; }
        public SolidLine MiningLine { get; set; }

        public MiningObject(IContainsGold obj, SolidLine line) : this()
        {
            Obj = obj;
            MiningLine = line;
        }
    }

    class CollisionData
    {
        public ISceneObject Obj { get; set; }
        public float TimeLeft { get; set; }

        public CollisionData(ISceneObject obj)
        {
            Obj = obj;
            TimeLeft = SharedDef.SPECTATOR_COLLISION_INTERVAL;
        }
    }

    public class MiningModuleControl : Control
    {
        private SceneMgr sceneMgr;
        private float farmedGold;
        private List<CollisionData> recentlyCollided;

        public List<MiningObject> currentlyMining;
        public Players.Player Owner { get; set; }
        public Vector Position { get { return me.Position; } }

        public override bool Enabled
        {
            get 
            { 
                return base.Enabled; 
            }
	        set 
	        { 
		        base.Enabled = value;
                if (!value)
                    removeMiningLines();
	        }
}

        public override void InitControl(ISceneObject me)
        {
            sceneMgr = me.SceneMgr;
            currentlyMining = new List<MiningObject>();
            recentlyCollided = new List<CollisionData>();
            farmedGold = 0;
        }

        public override void UpdateControl(float tpf)
        {
            CheckMining();
            MineObjects(tpf);
            UpdateRecentCollisions(tpf);
        }

        private void UpdateRecentCollisions(float tpf)
        {
            for(int i = recentlyCollided.Count - 1; i >= 0; i--) 
            {
                recentlyCollided[i].TimeLeft -= tpf;
                if (recentlyCollided[i].TimeLeft < 0)
                    recentlyCollided.RemoveAt(i);
            }
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

        private void CheckMining()
        {
            List<ISceneObject> collided = new List<ISceneObject>();

            foreach (ISceneObject obj in sceneMgr.GetSceneObjects(typeof(Asteroid)))
            {
                if (!(obj is IContainsGold))
                    continue;

                Vector center = (obj is Sphere) ? (obj as Sphere).Center : obj.Position;

                if ((center - me.Position).Length < SharedDef.SPECTATOR_MINING_RADIUS)
                {
                    collided.Add(obj);

                    if (!IsPresent(obj))
                        InitNewMining(obj as IContainsGold);
                }
            }

            for (int i = currentlyMining.Count - 1; i >= 0; i--)
            {
                if (!IsPresent(currentlyMining[i], collided))
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
            StretchingLineControl stretchingControl = new StretchingLineControl();
            stretchingControl.FirstObj = me;
            stretchingControl.SecondObj = obj;

            SolidLine line = new SimpleLine(sceneMgr, me.Position.ToPoint(), obj.Position.ToPoint(), Colors.Black, 1);
            line.AddControl(stretchingControl);
            line.AddControl(new MiningLineControl());

            sceneMgr.DelayedAttachToScene(line);
            currentlyMining.Add(new MiningObject(obj, line));
        }

        private void StopMining(MiningObject obj)
        {
            obj.MiningLine.DoRemoveMe();
            obj.MiningLine.Dead = true;
            currentlyMining.Remove(obj);
        }

        public void Collide(ISceneObject obj)
        {
            if (!Enabled || !(me is IDestroyable))
                return;

            if (obj is Asteroid)
            {
                foreach (CollisionData data in recentlyCollided)
                    if (data.Obj.Id == obj.Id)
                        return;

                int damage = (obj as Asteroid).Radius;
                (me as IDestroyable).TakeDamage(damage, null);
                me.SceneMgr.FloatingTextMgr.AddFloatingText(damage, me.Position, FloatingTextManager.TIME_LENGTH_4, FloatingTextType.DAMAGE);

                recentlyCollided.Add(new CollisionData(obj));
            }
        }

        private void removeMiningLines()
        {
            currentlyMining.ForEach(mineObj => mineObj.MiningLine.DoRemoveMe());
        }
    }
}
