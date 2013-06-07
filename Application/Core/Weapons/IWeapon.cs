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
using Orbit.Core.SpecialActions;

namespace Orbit.Core.Weapons
{
    public enum DeviceType
    {
        DEVICE_FIRST,
        HOOK,
        CANNON,
        MINE,
        HEALING_KIT,
        DEVICE_LAST
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
        string Name { get; set; }
        DeviceType DeviceType { get; set; }
        UpgradeLevel UpgradeLevel { get; set; }

        ISpecialAction NextSpecialAction();

        void ProccessClickEvent(Point point, MouseButton button, MouseButtonState buttonState);

        IWeaponClickListener AddClickListener(IWeaponClickListener listener);

        void RemoveClickListener(IWeaponClickListener listener);

        ISceneObject Shoot(Point point, bool noControl = false);

        bool IsReady();

        void TriggerUpgrade(IWeapon old);
    }
}
