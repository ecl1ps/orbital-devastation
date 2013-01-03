using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Shapes;
using Orbit.Core.Client;

namespace Orbit.Core.Scene.Entities.HeavyWeight
{
    public abstract class HeavyWeightSceneObject : SceneObject
    {
        public Path Path { get; set; }

        public HeavyWeightSceneObject(SceneMgr mgr, long id)
            : base(mgr, id)
        {
        }

        public override void DoRemove(ISceneObject obj)
        {
            obj.Dead = true;
            SceneMgr.RemoveHeavyweightSceneObjectFromScene(obj as HeavyWeightSceneObject);
        }
    }
}
