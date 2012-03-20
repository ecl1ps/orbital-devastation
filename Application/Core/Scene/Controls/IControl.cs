using System;
using Orbit.Core.Scene.Entities;

namespace Orbit.Core.Scene.Controls
{
    public interface IControl
    {
        void InitControl(ISceneObject me);

        void UpdateControl(float tpf);
    }
}
