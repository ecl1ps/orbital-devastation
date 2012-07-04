using System;

namespace Orbit.Core.Scene.Entities
{
    public interface ICollidable : ISceneObject
    {
        bool CollideWith(ICollidable other);

        void DoCollideWith(ICollidable other, float tpf);
    }
}
