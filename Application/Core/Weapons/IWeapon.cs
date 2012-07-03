using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Orbit.Core.Scene.Entities;
using System.Windows;
using Orbit.Core.Scene;
using Orbit.Core.Players;
using Orbit.Core.Client;
using System.Windows.Input;

namespace Orbit.Core.Weapons
{
    public enum DeviceType
    {
        WEAPON_FIRST,
        HOOK,
        CANNON,
        MINE,
        HEALING_KIT,
        WEAPON_LAST
    }

    public enum UpgradeLevel
    {
        LEVEL_NONE = 0,
        LEVEL1,
        LEVEL2,
        LEVEL3,
    }

    public interface IWeapon : IGameState
    {
        Player Owner { get; set; }
        SceneMgr SceneMgr { get; set; }
        float ReloadTime { get; set; }
        int Cost { get; set; }
        String Name { get; set; }
        DeviceType DeviceType { get; set; }
        UpgradeLevel UpgradeLevel { get; set; }

        IWeapon Next();

        void ProccessClickEvent(Point point, MouseButton button, MouseButtonState buttonState);

        void Shoot(Point point);

        Boolean IsReady();

        void TriggerUpgrade(IWeapon old);
    }
}
