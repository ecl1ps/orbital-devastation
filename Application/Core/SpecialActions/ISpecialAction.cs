using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Orbit.Core.Players;
using Orbit.Core.Client;

namespace Orbit.Core.SpecialActions
{
    public enum SpecialActionType
    {
        BRUTAL_GRAVITY,
        ASTEROID_THROW
    }

    public interface ISpecialAction
    {
        Player Owner { get; set; }
        SceneMgr SceneMgr { get; set; }
        String Name { get; set; }
        String ImageSource { get; set; }
        SpecialActionType Type { get; set; }
        float Cost { get; set; }

        void StartAction();

        Boolean IsReady();

        void SendActionExecuted();
    }
}
