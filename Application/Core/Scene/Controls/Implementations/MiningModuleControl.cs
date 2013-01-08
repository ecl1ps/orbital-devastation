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
using Orbit.Core.Scene.Controls.Collisions;
using Orbit.Core.Players;
using Orbit.Core.Client.GameStates;

namespace Orbit.Core.Scene.Controls.Implementations
{
    public struct MiningObject
    {
        public IContainsGold Obj { get; set; }
        public Line MiningLine { get; set; }

        public MiningObject(IContainsGold obj, Line line)
            : this()
        {
            Obj = obj;
            MiningLine = line;
        }
    }

    public class MiningModuleControl : Control, ICollisionReactionControl
    {
        private class CollisionData
        {
            public ISceneObject Obj { get; set; }
            public float TimeLeft { get; set; }

            public CollisionData(ISceneObject obj)
            {
                Obj = obj;
                TimeLeft = SharedDef.SPECTATOR_COLLISION_INTERVAL;
            }
        }

        private SceneMgr sceneMgr;
        private List<CollisionData> recentlyCollided;

        public List<MiningObject> CurrentlyMining;
        public Player Owner { get; set; }
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
                    RemoveMiningLines();
	        }
}

        protected override void InitControl(ISceneObject me)
        {
            sceneMgr = me.SceneMgr;
            CurrentlyMining = new List<MiningObject>();
            recentlyCollided = new List<CollisionData>();
        }

        protected override void UpdateControl(float tpf)
        {
            CheckModuleLink();
            UpdateRecentCollisions(tpf);
        }

        private void UpdateRecentCollisions(float tpf)
        {
            for (int i = recentlyCollided.Count - 1; i >= 0; i--) 
            {
                recentlyCollided[i].TimeLeft -= tpf;
                if (recentlyCollided[i].TimeLeft < 0)
                    recentlyCollided.RemoveAt(i);
            }
        }

        private void CheckModuleLink()
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
                        InitNewLink(obj as IContainsGold);
                }
            }

            for (int i = CurrentlyMining.Count - 1; i >= 0; i--)
            {
                if (!IsPresent(CurrentlyMining[i], collided))
                    StopLink(CurrentlyMining[i]);
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
            foreach (MiningObject mined in CurrentlyMining)
            {
                if (mined.Obj.Id == obj.Id)
                    return true;
            }

            return false;
        }

        private void InitNewLink(IContainsGold obj) 
        {
            StretchingLineControl stretchingControl = new StretchingLineControl();
            stretchingControl.FirstObj = me;
            stretchingControl.SecondObj = obj;

            Line line = new Line(sceneMgr, IdMgr.GetNewId(sceneMgr.GetCurrentPlayer().GetId()), me.Position, obj.Position, Colors.Black, 1);
            line.AddControl(stretchingControl);
            line.AddControl(new MiningLineControl());

            sceneMgr.DelayedAttachToScene(line);
            CurrentlyMining.Add(new MiningObject(obj, line));
        }

        private void StopLink(MiningObject obj)
        {
            obj.MiningLine.DoRemoveMe();
            obj.MiningLine.Dead = true;
            CurrentlyMining.Remove(obj);
        }

        private void RemoveMiningLines()
        {
            CurrentlyMining.ForEach(mineObj => mineObj.MiningLine.DoRemoveMe());
            CurrentlyMining.Clear();
        }

        public void DoCollideWith(ISceneObject other, float tpf)
        {
            if (!Enabled || !(me is IDestroyable))
                return;

            if (other is Asteroid)
            {
                foreach (CollisionData data in recentlyCollided)
                    if (data.Obj.Id == other.Id)
                        return;

                int damage = (other as Asteroid).Radius;

                sceneMgr.FloatingTextMgr.AddFloatingText(damage, me.Center + ((other.Center - me.Center) / 2), FloatingTextManager.TIME_LENGTH_3, FloatingTextType.DAMAGE);

                (me as IDestroyable).TakeDamage(damage, other);
                recentlyCollided.Add(new CollisionData(other));
            }
        }
    }
}
