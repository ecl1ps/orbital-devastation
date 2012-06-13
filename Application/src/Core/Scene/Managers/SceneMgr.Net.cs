using System;
using Orbit;
using Orbit.Core.Players;
using Orbit.Core.Scene.Controls;
using Orbit.Core.Scene.Entities;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Diagnostics;
using System.Threading;
using System.Windows.Media;
using System.Windows.Shapes;
using System.Windows.Threading;
using System.Collections.Concurrent;
using Lidgren.Network;

namespace Orbit.Core.Scene
{
    public partial class SceneMgr
    {
        private NetClient client;
        private string serverAddress;
        private NetConnection serverConnection;

        private void InitNetwork()
        {
            NetPeerConfiguration conf = new NetPeerConfiguration("Orbit");

            /*conf.SimulatedMinimumLatency = 0.1f;
            conf.SimulatedRandomLatency = 0.05f;*/

            // debug
            conf.EnableMessageType(NetIncomingMessageType.DebugMessage);
            conf.EnableMessageType(NetIncomingMessageType.Error);
            conf.EnableMessageType(NetIncomingMessageType.ErrorMessage);
            conf.EnableMessageType(NetIncomingMessageType.Receipt);
            conf.EnableMessageType(NetIncomingMessageType.UnconnectedData);
            conf.EnableMessageType(NetIncomingMessageType.VerboseDebugMessage);
            conf.EnableMessageType(NetIncomingMessageType.WarningMessage);

            client = new NetClient(conf);
            client.Start();
        }

        private void ConnectToServer()
        {
            NetOutgoingMessage msg = client.CreateMessage();
            msg.Write((int)PacketType.PLAYER_CONNECT);
            msg.Write(GetCurrentPlayer().Data.Name);

            serverConnection = client.Connect(serverAddress, SharedDef.PORT_NUMBER, msg);
        }

        private void ProcessMessages()
        {
            if (client == null || client.Status != NetPeerStatus.Running)
                return;

            NetIncomingMessage msg;
            while ((msg = client.ReadMessage()) != null)
            {
                switch (msg.MessageType)
                {
                    case NetIncomingMessageType.VerboseDebugMessage:
                    case NetIncomingMessageType.DebugMessage:
                    case NetIncomingMessageType.WarningMessage:
                    case NetIncomingMessageType.ErrorMessage:
                        Console.WriteLine(msg.ReadString());
                        break;
                    case NetIncomingMessageType.DiscoveryRequest:
                        break;
                    // If incoming message is Request for connection approval
                    // This is the very first packet/message that is sent from client
                    case NetIncomingMessageType.ConnectionApproval:                   
                        break;
                    case NetIncomingMessageType.Data:
                        ProcessIncomingDataMessage(msg);
                        break;
                    case NetIncomingMessageType.StatusChanged:
                        switch (msg.SenderConnection.Status)
                        {
                            case NetConnectionStatus.None:
                            case NetConnectionStatus.InitiatedConnect:
                            case NetConnectionStatus.RespondedAwaitingApproval:
                            case NetConnectionStatus.RespondedConnect:
                            case NetConnectionStatus.Connected:
                                if (msg.SenderConnection.RemoteHailMessage.ReadInt32() == (int)PacketType.PLAYER_ID_HAIL)
                                {
                                    GetCurrentPlayer().Data.Id = msg.SenderConnection.RemoteHailMessage.ReadInt32();

                                    Console.WriteLine("LOGIN confirmed (id: " + IdMgr.GetHighId(GetCurrentPlayer().Data.Id) + ")");

                                    // TODO: pri dostatecnem poctu hracu pri multiplayeru (2) ukazat tlacitko, ktere posle SendStartGameRequest()
                                    // ted se hra vzdy pusti hned pri dvou hracich
                                    //if (GameType == Gametype.SOLO_GAME)
                                        SendStartGameRequest();
                                }     
                                break;
                            case NetConnectionStatus.Disconnected:
                            case NetConnectionStatus.Disconnecting:
                                // spis by se melo zobrazit neco jako ze bylo ztraceno spojeni k serveru
                                EndGame(GetOpponentPlayer(), GameEnd.LEFT_GAME);
                                break;
                        }

                        // NOTE: Disconnecting and Disconnected are not instant unless client is shutdown with disconnect()
                        Console.WriteLine(msg.SenderConnection.ToString() + " status changed to: " + msg.SenderConnection.Status);
                        break;
                    default:
                        Console.WriteLine("Unhandled message type: " + msg.MessageType);
                        break;
                }
                client.Recycle(msg);
            }
        }

        public void SendStartGameRequest()
        {
            NetOutgoingMessage msg = CreateNetMessage();
            msg.Write((int)PacketType.START_GAME_REQUEST);
            SendMessage(msg);
        }

        private void ProcessIncomingDataMessage(NetIncomingMessage msg)
        {
            PacketType type = (PacketType)msg.ReadInt32();
            switch (type)
            {
                case PacketType.ALL_PLAYER_DATA:
                    if (players.Count != 0)
                    {
                        Console.WriteLine("Error: receiving new players but already have " + players.Count);
                        return;
                    }

                    int playerCount = msg.ReadInt32();

                    for (int i = 0; i < playerCount; ++i)
                    {
                        Player plr = CreatePlayer();
                        msg.ReadObjectPlayerData(plr.Data);
                        players.Add(plr);

                        if (plr.Data.Id == currentPlayer.Data.Id)
                            currentPlayer = plr;

                        if (plr.IsActivePlayer())
                        {
                            plr.CreateWeapons();
                            plr.Baze = SceneObjectFactory.CreateBase(this, plr);
                            AttachToScene(plr.Baze);
                        }
                    }

                    Invoke(new Action(() =>
                    {
                        Label lbl1 = (Label)LogicalTreeHelper.FindLogicalNode(canvas, "lblIntegrityLeft");
                        if (lbl1 != null)
                            lbl1.Content = 100 + "%";

                        Label lbl2 = (Label)LogicalTreeHelper.FindLogicalNode(canvas, "lblIntegrityRight");
                        if (lbl2 != null)
                            lbl2.Content = 100 + "%";
                    }));

                    ShowStatusText(3, "You are " + (GetCurrentPlayer().GetPlayerColor() == Colors.Red ? "Red" : "Blue"));
                    break;
                case PacketType.ALL_ASTEROIDS:
                    if (objects.Count > 2)
                    {
                        Console.WriteLine("Error: receiving all asteroids but already have " + objects.Count);
                        return;
                    }

                    int count = msg.ReadInt32();
                    for (int i = 0; i < count; ++i)
                    {
                        Asteroid s = new Asteroid(this);
                        if (msg.ReadInt32() != (int)PacketType.NEW_ASTEROID)
                        {
                            Console.Error.WriteLine("Corrupted object PacketType.SYNC_ALL_ASTEROIDS");
                            return;
                        }
                        s.ReadObject(msg);
                        s.SetGeometry(SceneGeometryFactory.CreateAsteroidImage(s)); ;
                        AttachToScene(s);
                        SyncReceivedObject(s, msg);
                    }
                    isInitialized = true;
                    break;
                case PacketType.NEW_ASTEROID:
                    {
                        Asteroid s = new Asteroid(this);
                        s.ReadObject(msg);
                        s.SetGeometry(SceneGeometryFactory.CreateAsteroidImage(s));
                        AttachToScene(s);
                        SyncReceivedObject(s, msg);
                    }
                    break;
                case PacketType.NEW_SINGULARITY_MINE:
                    {
                        SingularityMine s = new SingularityMine(this);
                        s.ReadObject(msg);
                        s.Owner = GetOpponentPlayer();
                        s.SetGeometry(SceneGeometryFactory.CreateRadialGradientEllipseGeometry(s));
                        AttachToScene(s);
                        SyncReceivedObject(s, msg);
                    }
                    break;
                case PacketType.NEW_SINGULARITY_BULLET:
                    {
                        SingularityBullet s = new SingularityBullet(this);
                        s.ReadObject(msg);
                        s.Player = GetOpponentPlayer();
                        s.SetGeometry(SceneGeometryFactory.CreateConstantColorEllipseGeometry(s));
                        AttachToScene(s);
                        SyncReceivedObject(s, msg);
                    }
                    break;
                case PacketType.NEW_HOOK:
                    {
                        Hook h = new Hook(this);
                        h.ReadObject(msg);
                        h.Owner = GetOpponentPlayer();
                        h.SetGeometry(SceneGeometryFactory.CreateHookHead(h));
                        BeginInvoke(new Action(() =>
                        {
                            Canvas.SetZIndex(h.GetGeometry(), 99);
                        }));
                        h.PrepareLine();
                        AttachToScene(h);
                        SyncReceivedObject(h, msg);
                    }
                    break;
                case PacketType.PLAYER_WON:
                    EndGame(GetPlayer(msg.ReadInt32()), GameEnd.WIN_GAME);
                    break;
                case PacketType.SINGULARITY_MINE_HIT:
                    long mineId = msg.ReadInt64();
                    long id = msg.ReadInt64();
                    Vector pos = msg.ReadVector();
                    Vector dir = msg.ReadVector();
                    foreach (ISceneObject obj in objects)
                    {
                        if (obj.Id == mineId)
                        {
                            DroppingSingularityControl c = obj.GetControlOfType(typeof(DroppingSingularityControl)) as DroppingSingularityControl;
                            if (c == null)
                                Console.Error.WriteLine("Object id " + mineId + " (" + obj.GetType().Name + ") is supposed to be a SingularityMine and have DroppingSingularityControl, but control is null");
                            else
                                c.StartDetonation();
                            continue;
                        }

                        if (obj.Id != id)
                            continue;

                        obj.Position = pos;
                        (obj as IMovable).Direction += dir;
                    }
                    break;
                case PacketType.BASE_INTEGRITY_CHANGE:
                    GetOpponentPlayer().SetBaseIntegrity(msg.ReadInt32());
                    break;
                case PacketType.START_GAME_RESPONSE:
                    SetMainInfoText("");
                    userActionsDisabled = false;
                    break;
            }

        }

        private void SyncReceivedObject(ISceneObject o, NetIncomingMessage msg)
        {
            o.Update(msg.SenderConnection.AverageRoundtripTime / 2);
        }

        public void SetRemoteServerAddress(string serverAddress)
        {
            this.serverAddress = serverAddress;
        }

        public bool IsServer()
        {
            return false;
        }

        public NetOutgoingMessage CreateNetMessage()
        {
            return client.CreateMessage();
        }

        public void SendMessage(NetOutgoingMessage msg)
        {
            client.SendMessage(msg, serverConnection, NetDeliveryMethod.ReliableOrdered);
        }
    }
}
