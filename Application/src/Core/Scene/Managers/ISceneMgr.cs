using System;
using Orbit.Core.Scene.Entities;
using System.Windows;
using Orbit.Core.Players;
using Lidgren.Network;

namespace Orbit.Core.Scene
{
    public interface ISceneMgr
    {
        Gametype GameType { get; set; }
        
        Size ViewPortSizeOriginal { get; set; }

        void Init(Gametype gameType);
        
        bool IsServer();
        
        void SetIsServer(bool isServer);

        void AttachToScene(ISceneObject obj, bool asNonInteractive = false);
        
        Rect GetOrbitArea();
        
        Player GetPlayer(PlayerPosition pos);
        
        Random GetRandomGenerator();

        void RemoveFromSceneDelayed(ISceneObject obj);

        NetOutgoingMessage CreateNetMessage();

        void SendMessage(NetOutgoingMessage msg);

        void Invoke(Action a);

        void BeginInvoke(Action a);

        void ShowStatusText(int index, string text);
    }
}
