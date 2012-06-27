using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Orbit.Core.Scene.Entities;
using System.Windows;
using Orbit.Core.Scene;
using Orbit.Core.Players;
using Orbit.Core.Client;

namespace Orbit.Core.Weapons
{
    public enum WeaponType
    {
        HOOK,
        CANNON,
        MINE
    }

    public interface IWeapon
    {
        Player Owner { get; set; }
        SceneMgr SceneMgr { get; set; }
        float ReloadTime { get; set; }
        int Cost { get; set; }
        String Name { get; set; }
        WeaponType WeaponType { get; set; }

        IWeapon Next();

        void Shoot(Point point);

        Boolean IsReady();

        void UpdateTimer(float value);

        void TriggerUpgrade(IWeapon old);
    }
}
