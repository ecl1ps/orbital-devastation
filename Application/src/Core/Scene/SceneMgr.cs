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
    public class SceneMgr
    {
        private Canvas canvas;
        private bool isServer;
        private bool isInitialized;
        private bool userActionsDisabled;
        private volatile bool shouldQuit;
        private NetPeer peer;
        private string serverAddress;
        private List<ISceneObject> objects;
        private List<ISceneObject> objectsToRemove;
        private List<Player> players;
        private PlayerPosition mePlayer;
        private PlayerPosition firstPlayer;
        private PlayerPosition secondPlayer;
        private Rect orbitArea;
        private Random randomGenerator;
        public Size ViewPortSizeOriginal { get; set; }
        public Size ViewPortSize { get; set; }
        private ConcurrentQueue<Action> synchronizedQueue;
        public Gametype GameType { get; set; }
        private bool gameEnded;

        private static SceneMgr sceneMgr;
        private static Object lck = new Object();


        public static SceneMgr GetInstance()
        {
            lock (lck)
            {
                if (sceneMgr == null)
                    sceneMgr = new SceneMgr();
                return sceneMgr;
            }
        }

        private SceneMgr()
        {
            isInitialized = false;
        }

        public void Init(Gametype gameType)
        {
            GameType = gameType;
            gameEnded = false;
            isInitialized = false;
            shouldQuit = false;
            objects = new List<ISceneObject>();
            objectsToRemove = new List<ISceneObject>();
            randomGenerator = new Random(Environment.TickCount);
            players = new List<Player>(2);
            synchronizedQueue = new ConcurrentQueue<Action>();

            GetUIDispatcher().Invoke(DispatcherPriority.Send, new Action(() =>
            {
                Label lbl = (Label)LogicalTreeHelper.FindLogicalNode(GetCanvas(), "lblEndGame");
                if (lbl != null)
                    lbl.Content = "";

                if (gameType != Gametype.CLIENT_GAME)
                {
                    Label lbl1 = (Label)LogicalTreeHelper.FindLogicalNode(GetCanvas(), "lblIntegrityLeft");
                    if (lbl1 != null)
                        lbl1.Content = 100 + "%";

                    Label lbl2 = (Label)LogicalTreeHelper.FindLogicalNode(GetCanvas(), "lblIntegrityRight");
                    if (lbl2 != null)
                        lbl2.Content = 100 + "%";
                }

                Label lblw = (Label)LogicalTreeHelper.FindLogicalNode(GetCanvas(), "lblWaiting");
                if (lblw != null)
                    lblw.Content = "";
                
            }));

            if (gameType == Gametype.SERVER_GAME)
            {
                userActionsDisabled = true;
                SetMainInfoText("Waiting for the other player to connect");
                CreatePlayers();
                mePlayer = firstPlayer;
                InitNetwork();
                CreateAsteroidField();
                isInitialized = false;
            }
            else if (gameType == Gametype.CLIENT_GAME)
            {
                userActionsDisabled = true;
                SetMainInfoText("Waiting for the server");
                InitNetwork();
            }
            else /*Gametype.SOLO_GAME*/
            {
                userActionsDisabled = false;
                CreatePlayers();
                mePlayer = firstPlayer;
                CreateAsteroidField();
                isInitialized = true;
            }

        }

        private void SetMainInfoText(String t)
        {
            GetUIDispatcher().Invoke(DispatcherPriority.Send, new Action(() =>
            {
                Label lblw = (Label)LogicalTreeHelper.FindLogicalNode(GetCanvas(), "lblWaiting");
                if (lblw != null)
                    lblw.Content = t;
            }));
        }

        private void CreateAsteroidField()
        {
            for (int i = 0; i < SharedDef.SPHERE_COUNT; ++i)
                AttachToScene(SceneObjectFactory.CreateNewRandomSphere(i % 2 == 0));
        }

        private void InitNetwork()
        {
            NetPeerConfiguration conf = new NetPeerConfiguration("Orbit");
            if (isServer)
            {
                conf.EnableMessageType(NetIncomingMessageType.DiscoveryRequest);
                conf.EnableMessageType(NetIncomingMessageType.ConnectionApproval);
                conf.Port = SharedDef.PORT_NUMBER;
            }
            conf.EnableMessageType(NetIncomingMessageType.Data);
            conf.EnableMessageType(NetIncomingMessageType.DebugMessage);
            conf.EnableMessageType(NetIncomingMessageType.Error);
            conf.EnableMessageType(NetIncomingMessageType.ErrorMessage);
            conf.EnableMessageType(NetIncomingMessageType.Receipt);
            conf.EnableMessageType(NetIncomingMessageType.UnconnectedData);
            conf.EnableMessageType(NetIncomingMessageType.VerboseDebugMessage);
            conf.EnableMessageType(NetIncomingMessageType.WarningMessage);

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
                    // Here you can do new player initialisation stuff
                    case NetIncomingMessageType.ConnectionApproval:

                        // Read the first byte of the packet
                        // (Enums can be casted to bytes, so it be used to make bytes human readable )
                        if (msg.ReadInt32() == (int)PacketType.PLAYER_CONNECT)
                        {
                            Console.WriteLine("Incoming LOGIN");

                            // Approve clients connection ( Its sort of agreenment. "You can be my client and i will host you" )
                            msg.SenderConnection.Approve();
                        }

                        break;
                    case NetIncomingMessageType.Data:
                        Console.WriteLine("Received data msg");
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
                                    Sphere s = new Sphere();
                                    msg.ReadObjectSphere(s);
                                    s.SetGeometry(SceneGeometryFactory.CreateAsteroidImage(s));

                                    LinearMovementControl nmc = new LinearMovementControl();
                                    nmc.Speed = msg.ReadFloat();
                                    s.AddControl(nmc);

                                    LinearRotationControl lrc = new LinearRotationControl();
                                    lrc.RotationSpeed = msg.ReadFloat();
                                    s.AddControl(lrc);

                                    AttachToScene(s);
                                }

                                SetMainInfoText("");
                                isInitialized = true;
                                userActionsDisabled = false;
                                break;
                            case PacketType.NEW_ASTEROID:
                                {
                                    Sphere s = new Sphere();
                                    msg.ReadObjectSphere(s);
                                    s.SetGeometry(SceneGeometryFactory.CreateAsteroidImage(s));

                                    LinearMovementControl nmc = new LinearMovementControl();
                                    nmc.Speed = msg.ReadFloat();
                                    s.AddControl(nmc);

                                    LinearRotationControl lrc = new LinearRotationControl();
                                    lrc.RotationSpeed = msg.ReadFloat();

                                    s.AddControl(lrc);

                                    AttachToScene(s);
                                }
                                break;
                            case PacketType.NEW_SINGULARITY_MINE:
                                {
                                    SingularityMine s = new SingularityMine();
                                    msg.ReadObjectSingularityMine(s);
                                    s.SetGeometry(SceneGeometryFactory.CreateRadialGradientEllipseGeometry(s));

                                    SingularityControl sc = new SingularityControl();
                                    msg.ReadObjectSingularityControl(sc);
                                    s.AddControl(sc);

                                    AttachToScene(s);
                                }
                                break;
                            case PacketType.PLAYER_WON:
                                EndGame(GetPlayer((PlayerPosition)msg.ReadByte()), GameEnd.WIN_GAME);
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
                                objects.ForEach(new Action<ISceneObject>(x => { if (x is Sphere) count++; }));
                                outmsg.Write(count);

                                foreach (ISceneObject obj in objects)
                                    if (obj is Sphere)
                                    {
                                        outmsg.WriteObjectSphere(obj as Sphere);
                                        outmsg.Write((obj.GetControlOfType(typeof(LinearMovementControl)) as LinearMovementControl).Speed);
                                        outmsg.Write((obj.GetControlOfType(typeof(LinearRotationControl)) as LinearRotationControl).RotationSpeed);
                                    }

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

        public void ShowStatusText(int index, string text)
        {
            GetUIDispatcher().Invoke(DispatcherPriority.Send, new Action(() =>
            {
                Label lbl = (Label)LogicalTreeHelper.FindLogicalNode(GetCanvas(), "statusText" + index);
                if (lbl != null)
                    lbl.Content = text;
            }));
        }

        private void EndGame(Player plr, GameEnd endType)
        {
            if (gameEnded)
                return;

            gameEnded = true;
            if (endType == GameEnd.WIN_GAME)
                PlayerWon(plr);
            else if (endType == GameEnd.LEFT_GAME)
                PlayerLeft(plr);

            RequestStop();

            Thread.Sleep(3000);
        }

        public void CloseGame()
        {
            RequestStop();
        }

        private void CleanUp()
        {
            if (peer != null && peer.Status != NetPeerStatus.NotRunning)
            {
                peer.Shutdown("Peer closed connection");
                Thread.Sleep(10); // networking threadu chvili trva ukonceni
            }

            objectsToRemove.Clear();

            GetUIDispatcher().Invoke(DispatcherPriority.Send, new Action(() =>
            {
                foreach (ISceneObject obj in objects)
                    canvas.Children.Remove(obj.GetGeometry());
            }));
        }

        public void AttachToScene(ISceneObject obj)
        {
            objects.Add(obj);
            GetUIDispatcher().BeginInvoke(DispatcherPriority.Send, new Action(() =>
            {
                canvas.Children.Add(obj.GetGeometry());
            }));
        }

        public void RemoveFromSceneDelayed(ISceneObject obj)
        {
            obj.Dead = true;
            objectsToRemove.Add(obj);
        }

        private void DirectRemoveFromScene(ISceneObject obj)
        {
            objects.Remove(obj);
            canvas.Dispatcher.BeginInvoke(DispatcherPriority.DataBind, (Action)(delegate
            {
                canvas.Children.Remove(obj.GetGeometry());
            }));
        }

        private void RemoveObjectsMarkedForRemoval()
        {
            foreach (ISceneObject obj in objectsToRemove)
            {
                obj.OnRemove();
                DirectRemoveFromScene(obj);
            }

            objectsToRemove.Clear();
        }

        private void CreatePlayers()
        {
            firstPlayer = randomGenerator.Next(2) == 0 ? PlayerPosition.LEFT : PlayerPosition.RIGHT;
            secondPlayer = firstPlayer == PlayerPosition.RIGHT ? PlayerPosition.LEFT : PlayerPosition.RIGHT;

            Player plr = new Player();
            plr.Data = new PlayerData();
            plr.Data.PlayerPosition = firstPlayer;
            plr.Data.PlayerColor = randomGenerator.Next(2) == 0 ? Colors.Red : Colors.Blue;

            Base baze = SceneObjectFactory.CreateBase(plr.Data);
            AttachToScene(baze);
            plr.Baze = baze;

            players.Add(plr);

            plr = new Player();
            plr.Data = new PlayerData();
            plr.Data.PlayerPosition = secondPlayer;
            plr.Data.PlayerColor = players[0].Data.PlayerColor == Colors.Blue ? Colors.Red : Colors.Blue;

            baze = SceneObjectFactory.CreateBase(plr.Data);
            AttachToScene(baze);
            plr.Baze = baze;

            players.Add(plr);
        }


        public void Run()
        {
            Stopwatch sw = new Stopwatch();
            float tpf = 0;
            sw.Start();

            while (!shouldQuit)
            {
                tpf = sw.ElapsedMilliseconds / 1000.0f;
                
                sw.Restart();

                ProcessMessages();

                if (tpf > 0.001 && isInitialized)
                    Update(tpf);

                ShowStatusText(1, "TPF: " + tpf);

                //Console.Out.WriteLine(tpf + ": " +sw.ElapsedMilliseconds);

		        if (sw.ElapsedMilliseconds < SharedDef.MINIMUM_UPDATE_TIME) 
                {
                    Thread.Sleep((int)(SharedDef.MINIMUM_UPDATE_TIME - sw.ElapsedMilliseconds));
		        }
            }

            sw.Stop();

            CleanUp();

            if (Application.Current != null)
                Application.Current.Dispatcher.BeginInvoke(new Action(() =>
                {
                    (Application.Current as App).GameEnded();
                }));
        }

        private void RequestStop()
        {
            shouldQuit = true;
        }

        public void Enqueue(Action act)
        {
            if (!shouldQuit && isInitialized)
                synchronizedQueue.Enqueue(act);
        }

        public void Update(float tpf)
        {
            ProcessActionQueue();

            UpdateSceneObjects(tpf);
            RemoveObjectsMarkedForRemoval();

            CheckCollisions();
            RemoveObjectsMarkedForRemoval();

            try
            {
                UpdateGeomtricState();
            }
            catch (NullReferenceException e)
            {
                // UI is closed before game finished its Update loop
                System.Console.Error.WriteLine(e);
            }

            CheckPlayerStates();
        }

        private void ProcessActionQueue()
        {
            Action act = null;
            while (synchronizedQueue.TryDequeue(out act))
                act.Invoke();
        }

        private void UpdateGeomtricState()
        {
            foreach (ISceneObject obj in objects)
            {
                obj.UpdateGeometric();
            }
        }

        public void UpdateSceneObjects(float tpf)
        {
            foreach (ISceneObject obj in objects)
            {             
                obj.Update(tpf);
                if (!obj.IsOnScreen(ViewPortSizeOriginal))
                    RemoveFromSceneDelayed(obj);
            }
        }


        public void CheckCollisions()
        {
            foreach (ISceneObject obj1 in objects)
            {
                if (!(obj1 is ICollidable))
                    continue;

                foreach (ISceneObject obj2 in objects)
                {
                    if (obj1.Id == obj2.Id)
                        continue;

                    /*if (obj2.IsDead())
                        continue;*/

                    if (!(obj2 is ICollidable))
                        continue;

                    if (((ICollidable)obj1).CollideWith((ICollidable)obj2))
                        ((ICollidable)obj1).DoCollideWith((ICollidable)obj2);
                }
            }
        }

        public Player GetPlayer(PlayerPosition pos)
        {
            foreach (Player plr in players)
            {
                if (plr.GetPosition() == pos)
                    return plr;
            }
            return null;
        }

        public void SetCanvas(Canvas canvas)
        {
            this.canvas = canvas;
            ViewPortSizeOriginal = new Size(canvas.Width, canvas.Height);
            orbitArea = new Rect(0, 0, canvas.Width, canvas.Height / 3);
        }

        public Canvas GetCanvas()
        {
            return canvas;
        }

        public Dispatcher GetUIDispatcher()
        {
            return canvas.Dispatcher;
        }

        public void OnViewPortChange(Size size)
        {
            ViewPortSize = size;
            GetUIDispatcher().BeginInvoke(DispatcherPriority.Normal, new Action(() =>
            {
                canvas.RenderTransform = new ScaleTransform(size.Width / ViewPortSizeOriginal.Width, size.Height / ViewPortSizeOriginal.Height);
            }));
        }

        public void SetIsServer(bool isServer)
        {
            this.isServer = isServer;
        }

        public Rect GetOrbitArea()
        {
            return orbitArea;
        }

        public Random GetRandomGenerator()
        {
            return randomGenerator;
        }

        public void OnCanvasClick(Point point)
        {
            if (userActionsDisabled)
                return;

            if (GetPlayer(mePlayer).IsMineReady())
            {
                GetPlayer(mePlayer).UseMine();
                SingularityMine mine = SceneObjectFactory.CreateSingularityMine(point, GetPlayer(mePlayer).Data);

                if (GameType != Gametype.SOLO_GAME)
                {
                    NetOutgoingMessage msg = CreatNetMessage();
                    msg.Write((int)PacketType.NEW_SINGULARITY_MINE);
                    msg.WriteObjectSingularityMine(mine);
                    msg.WriteObjectSingularityControl(mine.GetControlOfType(typeof(SingularityControl)) as SingularityControl);
                    SendMessage(msg);
                }

                AttachToScene(mine);
            }
        }

        private void CheckPlayerStates()
        {
            if (GetPlayer(firstPlayer).GetBaseIntegrity() <= 0)
                EndGame(GetPlayer(secondPlayer), GameEnd.WIN_GAME);
            else if (GetPlayer(secondPlayer).GetBaseIntegrity() <= 0)
                EndGame(GetPlayer(firstPlayer), GameEnd.WIN_GAME);
        }

        private void PlayerWon(Player winner)
        {
            if (GameType != Gametype.SOLO_GAME)
            {
                NetOutgoingMessage msg = CreatNetMessage();
                msg.Write((int)PacketType.PLAYER_WON);
                msg.Write((byte)winner.GetPosition());
                SendMessage(msg);
            }

            GetUIDispatcher().Invoke(DispatcherPriority.Render, new Action(() =>
            {
                Label lbl = (Label)LogicalTreeHelper.FindLogicalNode(GetCanvas(), "lblEndGame");
                if (lbl != null)
                    lbl.Content = (winner.Data.PlayerColor == Colors.Red ? "Red" : "Blue") + " player won!";
            }));
        }


        private void PlayerLeft(Player leaver)
        {
            if (leaver == null)
                return;

            GetUIDispatcher().Invoke(DispatcherPriority.Render, new Action(() =>
            {
                Label lbl = (Label)LogicalTreeHelper.FindLogicalNode(GetCanvas(), "lblEndGame");
                if (lbl != null)
                    lbl.Content = (leaver.Data.PlayerColor == Colors.Red ? "Red" : "Blue") + " player left the game!";
            }));
        }

        public void SetRemoteServerAddress(string serverAddress)
        {
            this.serverAddress = serverAddress;
        }

        public bool IsServer()
        {
            return isServer;
        }

        public static NetOutgoingMessage CreatNetMessage()
        {
            return GetInstance().peer.CreateMessage();
        }

        public static void SendMessage(NetOutgoingMessage msg)
        {
            GetInstance().peer.SendMessage(msg, GetInstance().GetOtherPlayer().Connection, NetDeliveryMethod.ReliableOrdered);
        }

        private Player GetOtherPlayer()
        {
            return IsServer() ? players[1] : players[0];
        }
    }

}
