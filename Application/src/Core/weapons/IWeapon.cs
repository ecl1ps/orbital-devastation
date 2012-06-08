using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Orbit.Core.Scene.Entities;
using System.Windows;
using Orbit.Core.Scene;

namespace Orbit.Core.Weapons
{
    public interface IWeapon
    {
        ISceneMgr SceneMgr { get; set; }
        float ReloadTime { get; set; }
        int Cost { get; set; }

        IWeapon Next();

        void Shoot(Point point);

        Boolean IsReady();

        void UpdateTimer(float value);
    }
}
