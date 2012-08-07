using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Orbit.Core.Client;

namespace Orbit.Core.Scene.Entities.Implementations
{
    class SingularityExplodingExcludingBullet : SingularityExplodingBullet
    {
        private List<ISceneObject> ignored;

        public SingularityExplodingExcludingBullet(ISceneObject toIgnore, SceneMgr mgr)
            : base(mgr)
        {
            ignored = new List<ISceneObject>();
            ignored.Add(toIgnore);
        }

        public SingularityExplodingExcludingBullet(List<ISceneObject> toIgnore, SceneMgr mgr) 
            : base(mgr) 
        {
            ignored = toIgnore;
        }

        public override bool CollideWith(ICollidable other)
        {
            foreach(ISceneObject obj in ignored) 
            {
                if(other.Equals(obj))
                    return false;
            }

 	        return base.CollideWith(other);
        }
    }
}
