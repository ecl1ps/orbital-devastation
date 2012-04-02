using System;

namespace Orbit.Core.Scene.Entities
{

    public class SingularityMine : SceneObject, ICollidable
    {
        public int Radius { get; set; }

        public override bool IsOnScreen()
        {
            throw new Exception("Not implemented");
        }

        public bool CollideWith(ICollidable other)
        {
            throw new Exception("Not implemented");
        }

        public void DoCollideWith(ICollidable other)
        {
            throw new Exception("Not implemented");
        }

        public override void UpdateGeometric()
        {
            throw new Exception("The method or operation is not implemented.");
        }
    }

}
