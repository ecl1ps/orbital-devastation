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
using Orbit.Core.Scene.Entities.Implementations;
using Orbit.Core.Scene.Controls.Implementations;
using Orbit.Core.Helpers;
using Orbit.Core.AI;

namespace Orbit.Core.Client
{
    public partial class SceneMgr
    {
        private NetClient client;
        private string serverAddress;
        private NetConnection serverConnection;
        private Queue<NetOutgoingMessage> pendingMessages;
        private bool tournametRunnig;

        private void InitNetwork()
        {
            pendingMessages = new Queue<NetOutgoingMessage>();
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
            msg.Write(GetCurrentPlayer().Data.HashId);

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
                                break;
                            case NetConnectionStatus.Connected:
                                if (msg.SenderConnection.RemoteHailMessage.ReadInt32() == (int)PacketType.PLAYER_ID_HAIL)
                                {
                                    currentPlayer.Data.Id = msg.SenderConnection.RemoteHailMessage.ReadInt32();
                                    // pokud uz takove jmeno na serveru existuje, tak obdrzime nove
                                    currentPlayer.Data.Name = msg.SenderConnection.RemoteHailMessage.ReadString();
                                    Application.Current.Dispatcher.Invoke(new Action(() =>
                                    {
                                        (Application.Current as App).PlayerName = currentPlayer.Data.Name;
                                    }));
                                    // pokud je hra zakladana pres lobby, tak o tom musi vedet i klient, ktery ji nezakladal
                                    Gametype serverType = (Gametype)msg.SenderConnection.RemoteHailMessage.ReadByte();
                                    tournametRunnig = msg.SenderConnection.RemoteHailMessage.ReadBoolean();
                                    if (GameType != Gametype.TOURNAMENT_GAME && serverType == Gametype.TOURNAMENT_GAME)
                                    {
                                        GameType = serverType;
                                        if (!tournametRunnig)
                                            Application.Current.Dispatcher.Invoke(new Action(() =>
                                            {
                                                (Application.Current as App).CreateLobbyGui(false);
                                            }));
                                    }

                                    while (pendingMessages.Count != 0)
                                        SendMessage(pendingMessages.Dequeue());

                                    Console.WriteLine("LOGIN confirmed (id: " + IdMgr.GetHighId(GetCurrentPlayer().Data.Id) + ")");

                                    SendPlayerDataRequestMessage();
                                }     
                                break;
                            case NetConnectionStatus.Disconnected:
                            case NetConnectionStatus.Disconnecting:
                                EndGame(null, GameEnd.SERVER_DISCONNECTED);
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

        private void SendPlayerDataRequestMessage()
        {
            NetOutgoingMessage reqmsg = CreateNetMessage();
            reqmsg.Write((int)PacketType.ALL_PLAYER_DATA_REQUEST);
            SendMessage(reqmsg);
        }

        public void SendStartGameRequest()
        {
            NetOutgoingMessage msg = CreateNetMessage();
            msg.Write((int)PacketType.START_GAME_REQUEST);
            SendMessage(msg);
        }

        public void SendPlayerReadyMessage()
        {
            NetOutgoingMessage msg = CreateNetMessage();
            msg.Write((int)PacketType.PLAYER_READY);
            msg.Write(currentPlayer.GetId());
            msg.Write(currentPlayer.Data.LobbyLeader);
            SendMessage(msg);
        }

        private void ProcessIncomingDataMessage(NetIncomingMessage msg)
        {
            PacketType type = (PacketType)msg.ReadInt32();
            Console.WriteLine("Client " + GetCurrentPlayer().GetId() + ": received msg " + type.ToString());
            switch (type)
            {
                case PacketType.ALL_PLAYER_DATA:
                    int playerCount = msg.ReadInt32();
                    List<int> newPlrs = new List<int>(playerCount);

                    for (int i = 0; i < playerCount; ++i)
                    {
                        Player plr = GetPlayer(msg.ReadInt32());
                        // jeste hrace nezname
                        if (plr == null)
                        {
                            plr = CreatePlayer();
                            players.Add(plr);

                            msg.ReadObjectPlayerData(plr.Data);

                            if (plr.Data.Name.Equals("Bot"))
                                stateMgr.AddGameState(new SimpleBot(this, objects, plr));
                        }
                        else // hrace uz zname, ale mohl se zmenit jeho stav na active a take se mohly zmenit dalsi player data
                            msg.ReadObjectPlayerData(plr.Data);

                        newPlrs.Add(plr.GetId());
                    }

                    // pokud mame navic nejake stare hrace, tak je odstranime
                    if (playerCount != newPlrs.Count)
                        for (int i = 0; i < players.Count; ++i)
                            if (!newPlrs.Contains(players[i].GetId()))
                                players.RemoveAt(i);

                    if ((GameType != Gametype.TOURNAMENT_GAME || tournametRunnig) && !currentPlayer.Data.StartReady)
                        SendStartGameRequest();
                    else
                    {
                        CheckAllPlayersReady();
                        UpdateLobbyPlayers();
                    }
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
                        if (msg.ReadInt32() != (int)PacketType.NEW_ASTEROID)
                        {
                            Console.WriteLine("Corrupted object PacketType.SYNC_ALL_ASTEROIDS");
                            return;
                        }
                        Asteroid s = CreateNewAsteroid((AsteroidType)msg.ReadByte());
                        s.ReadObject(msg);
                        s.SetGeometry(SceneGeometryFactory.CreateAsteroidImage(s));
                        DelayedAttachToScene(s);
                        SyncReceivedObject(s, msg);
                    }
                    isGameInitialized = true;
                    break;
                case PacketType.NEW_ASTEROID:
                    {
                        Asteroid s = CreateNewAsteroid((AsteroidType)msg.ReadByte());
                        s.ReadObject(msg);
                        s.SetGeometry(SceneGeometryFactory.CreateAsteroidImage(s));
                        DelayedAttachToScene(s);
                        SyncReceivedObject(s, msg);
                    }
                    break;
                case PacketType.NEW_SINGULARITY_MINE:
                    {
                        SingularityMine s = new SingularityMine(this);
                        s.ReadObject(msg);
                        s.Owner = GetOpponentPlayer();
                        s.SetGeometry(SceneGeometryFactory.CreateRadialGradientEllipseGeometry(s));
                        DelayedAttachToScene(s);
                        SyncReceivedObject(s, msg);
                    }
                    break;
                case PacketType.NEW_SINGULARITY_BULLET:
                    {
                        SingularityBullet s = new SingularityBullet(this);
                        s.ReadObject(msg);
                        s.Player = GetOpponentPlayer();
                        s.SetGeometry(SceneGeometryFactory.CreateConstantColorEllipseGeometry(s));
                        DelayedAttachToScene(s);
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
                        DelayedAttachToScene(h);
                        SyncReceivedObject(h, msg);
                    }
                    break;
                case PacketType.PLAYER_WON:
                    {
                        Player winner = GetPlayer(msg.ReadInt32());
                        winner.Data.WonMatches = msg.ReadInt32();
                        EndGame(winner, GameEnd.WIN_GAME);
                    }
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
                case PacketType.HOOK_HIT:
                    long hookId = msg.ReadInt64();
                    long asteroidId = msg.ReadInt64();
                    Vector position = msg.ReadVector();
                    Vector hitVector = msg.ReadVector();
                    Hook hook = null;
                    IContainsGold g = null;
                    foreach (ISceneObject obj in objects)
                    {
                        if (obj.Id == hookId)
                        {
                            if (obj is Hook)
                            {
                                hook = obj as Hook;
                                if(g != null) {
                                    hook.Catch(g, hitVector);
                                    break;
                                } else
                                    continue;
                            } else
                                Console.Error.WriteLine("Object id " + hookId + " (" + obj.GetType().Name + ") is supposed to be a Hook but it is not");
                        }

                        if (obj.Id == asteroidId)
                        {
                            if (obj is IContainsGold)
                            {
                                g = obj as IContainsGold;
                                obj.Position = position;
                                if (hook != null)
                                {
                                    hook.Catch(g, hitVector);
                                    break;
                                }
                            } else
                                Console.Error.WriteLine("Object id " + asteroidId + " (" + obj.GetType().Name + ") is supposed to be a IContainsGold but it is not");
                        }

                    }
                    break;
                case PacketType.BASE_INTEGRITY_CHANGE:
                case PacketType.PLAYER_HEAL:
                    GetPlayer(msg.ReadInt32()).SetBaseIntegrity(msg.ReadInt32());
                    break;
                case PacketType.START_GAME_RESPONSE:
                    string leftPlr = players.Find(p => p.IsActivePlayer() && p.GetPosition() == PlayerPosition.LEFT).Data.Name;
                    string rightPlr = players.Find(p => p.IsActivePlayer() && p.GetPosition() == PlayerPosition.RIGHT).Data.Name;

                    foreach (Player p in players)
                        if (p.IsActivePlayer())
                        {
                            p.CreateWeapons();
                            // zobrazi aktualni integrity bazi
                            p.SetBaseIntegrity(p.GetBaseIntegrity());
                            p.Baze = SceneObjectFactory.CreateBase(this, p);
                            DelayedAttachToScene(p.Baze);
                        }

                    Invoke(new Action(() =>
                    {
                        Label lbl3 = (Label)LogicalTreeHelper.FindLogicalNode(canvas, "lblNameLeft");
                        if (lbl3 != null)
                            lbl3.Content = leftPlr;

                        Label lbl4 = (Label)LogicalTreeHelper.FindLogicalNode(canvas, "lblNameRight");
                        if (lbl4 != null)
                            lbl4.Content = rightPlr;
                    }));

                    if (currentPlayer.IsActivePlayer())
                        ShowStatusText(3, "You are " + (currentPlayer.GetPlayerColor() == Colors.Red ? "Red" : "Blue"));
                    else
                        ShowStatusText(3, "You are Spectator");

                    SetMainInfoText("");
                    if (currentPlayer.IsActivePlayer())
                        userActionsDisabled = false;
                    break;
                case PacketType.TOURNAMENT_STARTING:
                    Application.Current.Dispatcher.Invoke(new Action(() =>
                    {
                        (Application.Current as App).CreateGameGui(false);
                        Canvas c = (Application.Current as App).GetCanvas();
                        SetCanvas(c, new Size(c.Width, c.Height));
                    }));
                    
                    actionMgr = new PlayerActionManager(this);
                    stateMgr.AddGameState(actionMgr);
                    BeginInvoke(new Action(() =>
                    {
                        Label lbl = (Label)LogicalTreeHelper.FindLogicalNode(canvas, "lblEndGame");
                        if (lbl != null)
                            lbl.Content = "";

                        Label lblw = (Label)LogicalTreeHelper.FindLogicalNode(canvas, "lblWaiting");
                        if (lblw != null)
                            lblw.Content = "";

                    }));
                    break;
                case PacketType.TOURNAMENT_FINISHED:
                    EndGame(GetPlayer(msg.ReadInt32()), GameEnd.TOURNAMENT_FINISHED);
                    break;
                case PacketType.PLAYER_READY:
                    Player pl = GetPlayer(msg.ReadInt32());
                    pl.Data.LobbyReady = true;
                    CheckAllPlayersReady();
                    break;
                case PacketType.CHAT_MESSAGE:
                    ShowChatMessage(msg.ReadString());
                    break;
                case PacketType.PLAYER_DISCONNECTED:
                    Player disconnected = GetPlayer(msg.ReadInt32());

                    players.Remove(disconnected);

                    if (disconnected.IsActivePlayer())
                        EndGame(disconnected, GameEnd.LEFT_GAME);

                    if (GameType == Gametype.TOURNAMENT_GAME)
                    {
                        UpdateLobbyPlayers();
                        CheckAllPlayersReady();
                    }
                    break;
                case PacketType.SERVER_SHUTDOWN:
                    EndGame(null, GameEnd.SERVER_DISCONNECTED);
                    break;
            }

        }

        private Asteroid CreateNewAsteroid(AsteroidType asteroidType)
        {
            Asteroid asteroid;
            switch (asteroidType)
            {
                case AsteroidType.GOLDEN:
                    asteroid = new Asteroid(this);
                    break;
                case AsteroidType.NORMAL:
                    asteroid = new Asteroid(this);
                    break;
                case AsteroidType.SPAWNED:
                    asteroid = new MinorAsteroid(this);
                    break;
                case AsteroidType.UNSTABLE:
                    asteroid = new UnstableAsteroid(this);
                    break;
                default:
                    asteroid = new Asteroid(this);
                    break;
            }

            asteroid.AsteroidType = asteroidType;
            return asteroid;
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
            if (serverConnection.Status != NetConnectionStatus.Connected)
                pendingMessages.Enqueue(msg);
            else
                client.SendMessage(msg, serverConnection, NetDeliveryMethod.ReliableOrdered);
        }
    }
}
