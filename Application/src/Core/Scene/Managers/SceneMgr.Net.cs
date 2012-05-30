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
        private bool isServer;
        private NetPeer peer;
        private string serverAddress;

        private void InitNetwork()
        {
            NetPeerConfiguration conf = new NetPeerConfiguration("Orbit");
            if (isServer)
            {
                conf.EnableMessageType(NetIncomingMessageType.DiscoveryRequest);
                conf.EnableMessageType(NetIncomingMessageType.ConnectionApproval);
                conf.Port = SharedDef.PORT_NUMBER;
            }

            /*conf.SimulatedMinimumLatency = 0.1f;
            conf.SimulatedRandomLatency = 0.05f;*/

            /*conf.EnableMessageType(NetIncomingMessageType.DebugMessage);
            conf.EnableMessageType(NetIncomingMessageType.Error);
            conf.EnableMessageType(NetIncomingMessageType.ErrorMessage);
            conf.EnableMessageType(NetIncomingMessageType.Receipt);
            conf.EnableMessageType(NetIncomingMessageType.UnconnectedData);
            conf.EnableMessageType(NetIncomingMessageType.VerboseDebugMessage);
            conf.EnableMessageType(NetIncomingMessageType.WarningMessage);*/

            peer = new NetPeer(conf);
            peer.Start();

            if (!isServer)
            {
                NetOutgoingMessage msg = peer.CreateMessage();
                msg.Write((int)PacketType.PLAYER_CONNECT);
                peer.Connect(serverAddress, SharedDef.PORT_NUMBER, msg);
            }
        }

        private void ProcessMessages()
        {
            if (peer == null || peer.Status != NetPeerStatus.Running)
                return;

            NetIncomingMessage msg;
            while ((msg = peer.ReadMessage()) != null)
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
                        if (!isServer || !userActionsDisabled)
                            break;

                        NetOutgoingMessage response = peer.CreateMessage();
                        //response.Write("My server name");
                        peer.SendDiscoveryResponse(response, msg.SenderEndpoint);
                        break;
                    // If incoming message is Request for connection approval
                    // This is the very first packet/message that is sent from client
                    case NetIncomingMessageType.ConnectionApproval:
                        if (msg.ReadInt32() == (int)PacketType.PLAYER_CONNECT)
                        {
                            Console.WriteLine("Incoming LOGIN");

                            // Approve clients connection ( Its sort of agreenment. "You can be my client and i will host you" )
                            msg.SenderConnection.Approve();
                        }
                        break;
                    case NetIncomingMessageType.Data:
                        PacketType type = (PacketType)msg.ReadInt32();
                        switch (type)
                        {
                            case PacketType.SYNC_ALL_PLAYER_DATA:
                                if (players.Count != 0)
                                {
                                    Console.WriteLine("Error: receiving new players but already have " + players.Count);
                                    return;
                                }

                                for (int i = 0; i < 2; ++i)
                                {
                                    Player plr = new Player();
                                    plr.Data = new PlayerData();
                                    msg.ReadObjectPlayerData(plr.Data);
                                    plr.Baze = SceneObjectFactory.CreateBase(plr.Data);
                                    AttachToScene(plr.Baze);
                                    players.Add(plr);
                                }

                                GetUIDispatcher().Invoke(DispatcherPriority.Send, new Action(() =>
                                {
                                    Label lbl1 = (Label)LogicalTreeHelper.FindLogicalNode(GetCanvas(), "lblIntegrityLeft");
                                    if (lbl1 != null)
                                        lbl1.Content = 100 + "%";

                                    Label lbl2 = (Label)LogicalTreeHelper.FindLogicalNode(GetCanvas(), "lblIntegrityRight");
                                    if (lbl2 != null)
                                        lbl2.Content = 100 + "%";
                                }));

                                firstPlayer = players[0].GetPosition();
                                secondPlayer = players[1].GetPosition();
                                mePlayer = secondPlayer;
                                players[0].Connection = msg.SenderConnection;
                                ShowStatusText(3, "You are " + (players[1].GetPlayerColor() == Colors.Red ? "Red" : "Blue"));
                                break;
                            case PacketType.SYNC_ALL_ASTEROIDS:
                                if (objects.Count > 2)
                                {
                                    Console.WriteLine("Error: receiving all asteroids but already have " + objects.Count);
                                    return;
                                }

                                int count = msg.ReadInt32();
                                for (int i = 0; i < count; ++i)
                                {
                                    Asteroid s = new Asteroid();
                                    if (msg.ReadInt32() != (int)PacketType.NEW_ASTEROID)
                                    {
                                        Console.Error.WriteLine("Corrupted object PacketType.SYNC_ALL_ASTEROIDS");
                                        return;
                                    }
                                    (s as ISendable).ReadObject(msg);
                                    s.SetGeometry(SceneGeometryFactory.CreateAsteroidImage(s));;
                                    AttachToScene(s);
                                    SyncReceivedObject(s, msg);
                                }

                                SetMainInfoText("");
                                isInitialized = true;
                                userActionsDisabled = false;
                                break;
                            case PacketType.NEW_ASTEROID:
                                {
                                    Asteroid s = new Asteroid();
                                    (s as ISendable).ReadObject(msg);
                                    s.SetGeometry(SceneGeometryFactory.CreateAsteroidImage(s));
                                    AttachToScene(s);
                                    SyncReceivedObject(s, msg);
                                }
                                break;
                            case PacketType.NEW_SINGULARITY_MINE:
                                {
                                    SingularityMine s = new SingularityMine();
                                    (s as ISendable).ReadObject(msg);
                                    s.Owner = GetOtherPlayer();
                                    s.SetGeometry(SceneGeometryFactory.CreateRadialGradientEllipseGeometry(s));
                                    AttachToScene(s);
                                    SyncReceivedObject(s, msg);
                                }
                                break;
                            case PacketType.NEW_SINGULARITY_BULLET:
                                {
                                    SingularityBullet s = new SingularityBullet();
                                    (s as ISendable).ReadObject(msg);
                                    s.Player = GetOtherPlayer();
                                    s.SetGeometry(SceneGeometryFactory.CreateConstantColorEllipseGeometry(s));
                                    AttachToScene(s);
                                    SyncReceivedObject(s, msg);
                                }
                                break;
                            case PacketType.PLAYER_WON:
                                EndGame(GetPlayer((PlayerPosition)msg.ReadByte()), GameEnd.WIN_GAME);
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
                        }

                        break;
                    case NetIncomingMessageType.StatusChanged:
                        switch (msg.SenderConnection.Status)
                        {
                            case NetConnectionStatus.None:
                            case NetConnectionStatus.InitiatedConnect:
                            case NetConnectionStatus.RespondedAwaitingApproval:
                            case NetConnectionStatus.RespondedConnect:
                                break;
                            case NetConnectionStatus.Disconnected:
                            case NetConnectionStatus.Disconnecting:
                                EndGame(GetPlayer(mePlayer == firstPlayer ? secondPlayer : firstPlayer), GameEnd.LEFT_GAME);
                                break;
                            case NetConnectionStatus.Connected:
                                if (!isServer)
                                    return;

                                GetOtherPlayer().Connection = msg.SenderConnection;

                                // poslani dat hracu
                                NetOutgoingMessage outmsg = peer.CreateMessage();

                                outmsg.Write((int)PacketType.SYNC_ALL_PLAYER_DATA);

                                foreach (Player plr in players)
                                    outmsg.WriteObjectPlayerData(plr.Data);

                                // Send message/packet to all connections, in reliably order, channel 0
                                // Reliably means, that each packet arrives in same order they were sent. Its slower than unreliable, but easyest to understand
                                peer.SendMessage(outmsg, msg.SenderConnection, NetDeliveryMethod.ReliableOrdered);

                                // poslani vsech asteroidu
                                outmsg = peer.CreateMessage();
                                outmsg.Write((int)PacketType.SYNC_ALL_ASTEROIDS);

                                Int32 count = 0;
                                objects.ForEach(new Action<ISceneObject>(x => { if (x is Asteroid) count++; }));
                                outmsg.Write(count);

                                foreach (ISceneObject obj in objects)
                                    if (obj is Asteroid && obj is ISendable)
                                        (obj as ISendable).WriteObject(outmsg);

                                peer.SendMessage(outmsg, msg.SenderConnection, NetDeliveryMethod.ReliableOrdered);

                                SetMainInfoText("");
                                isInitialized = true;
                                userActionsDisabled = false;
                                break;
                        }

                        // NOTE: Disconnecting and Disconnected are not instant unless client is shutdown with disconnect()
                        Console.WriteLine(msg.SenderConnection.ToString() + " status changed to: " + msg.SenderConnection.Status);
                        break;
                    default:
                        Console.WriteLine("Unhandled message type: " + msg.MessageType);
                        break;
                }
                peer.Recycle(msg);
            }
        }

        private void SyncReceivedObject(ISceneObject o, NetIncomingMessage msg)
        {
            o.Update(msg.SenderConnection.AverageRoundtripTime / 2);
        }

        public void SetIsServer(bool isServer)
        {
            this.isServer = isServer;
        }

        public void SetRemoteServerAddress(string serverAddress)
        {
            this.serverAddress = serverAddress;
        }

        public bool IsServer()
        {
            return isServer;
        }

        public static NetOutgoingMessage CreateNetMessage()
        {
            return GetInstance().peer.CreateMessage();
        }

        public static void SendMessage(NetOutgoingMessage msg)
        {
            GetInstance().peer.SendMessage(msg, GetInstance().GetOtherPlayer().Connection, NetDeliveryMethod.ReliableOrdered);
        }
    }
}
