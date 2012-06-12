using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Orbit.Core.Scene.Entities;
using System.Windows;
using Orbit.Core.Scene;
using Orbit.Core.Players;

namespace Orbit.Core.Weapons
{
    public interface IWeapon
    {
        Player Owner { get; set; }
        SceneMgr SceneMgr { get; set; }
        float ReloadTime { get; set; }
        int Cost { get; set; }

        IWeapon Next();

        void Shoot(Point point);

        Boolean IsReady();

        void UpdateTimer(float value);
    }
}
