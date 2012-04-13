using System;
using Orbit;
using Orbit.Core.Player;
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
        private List<PlayerData> playerData;
        private PlayerPosition mePlayer;
        private PlayerPosition firstPlayer;
        private PlayerPosition secondPlayer;
        private Rect orbitArea;
        private Random randomGenerator;
        public Size ViewPortSizeOriginal { get; set; }
        public Size ViewPortSize { get; set; }
        private ConcurrentQueue<Action> synchronizedQueue;

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
            isInitialized = false;
            shouldQuit = false;
            objects = new List<ISceneObject>();
            objectsToRemove = new List<ISceneObject>();
            randomGenerator = new Random(Environment.TickCount);
            playerData = new List<PlayerData>(2);
            synchronizedQueue = new ConcurrentQueue<Action>();

            GetUIDispatcher().Invoke(DispatcherPriority.Send, new Action(() =>
            {
                Label lbl = (Label)LogicalTreeHelper.FindLogicalNode(GetCanvas(), "lblEndGame");
                if (lbl != null)
                    lbl.Content = "";

                Label lbl1 = (Label)LogicalTreeHelper.FindLogicalNode(GetCanvas(), "lblIntegrityLeft");
                if (lbl1 != null)
                    lbl1.Content = 100 + "%";

                Label lbl2 = (Label)LogicalTreeHelper.FindLogicalNode(GetCanvas(), "lblIntegrityRight");
                if (lbl2 != null)
                    lbl2.Content = 100 + "%";

                Label lblw = (Label)LogicalTreeHelper.FindLogicalNode(GetCanvas(), "lblWaiting");
                if (lblw != null)
                    if (gameType == Gametype.SERVER_GAME)
                        lblw.Content = "Waiting for the other player to connect";
                    else
                        lblw.Content = "";
                
            }));

            if (gameType == Gametype.SERVER_GAME)
            {
                userActionsDisabled = true;
                CreateNewPlayerData();
                mePlayer = firstPlayer;
                InitNetwork();
                CreateAsteroidField();
                isInitialized = true;
            }
            else if (gameType == Gametype.CLIENT_GAME)
            {
                userActionsDisabled = true;
                InitNetwork();
                //mePlayer = secondPlayer;
            }
            else /*Gametype.SOLO_GAME*/
            {
                userActionsDisabled = false;
                CreateNewPlayerData();
                mePlayer = firstPlayer;
                CreateAsteroidField();
                isInitialized = true;
            }

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

            peer = new NetPeer(conf);
            peer.Start();

            if (!isServer)
            {
                NetOutgoingMessage msg = peer.CreateMessage();
                msg.Write((byte)PacketType.PLAYER_CONNECT);
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
                        // ( Enums can be casted to bytes, so it be used to make bytes human readable )
                        if (msg.ReadByte() == (byte)PacketType.PLAYER_CONNECT)
                        {
                            Console.WriteLine("Incoming LOGIN");

                            // Approve clients connection ( Its sort of agreenment. "You can be my client and i will host you" )
                            msg.SenderConnection.Approve();

                            // Create message, that can be written and sent
                            NetOutgoingMessage outmsg = peer.CreateMessage();

                            // first we write byte
                            outmsg.Write((byte)PacketType.SYNC_ALL_PLAYER_DATA);

                            // iterate trought every character ingame
                            foreach (PlayerData data in playerData)
                            {
                                // This is handy method
                                // It writes all the properties of object to the packet
                                outmsg.WriteAllProperties(data);
                            }

                            // Send message/packet to all connections, in reliably order, channel 0
                            // Reliably means, that each packet arrives in same order they were sent. Its slower than unreliable, but easyest to understand
                            peer.SendMessage(outmsg, msg.SenderConnection, NetDeliveryMethod.ReliableOrdered);

                            // Debug
                            Console.WriteLine("Approved new connection and updated the world status");
                        }

                        break;
                    case NetIncomingMessageType.Data:

                        // Read first byte
                        if (msg.ReadByte() == (byte)PacketType.SYNC_ALL_PLAYER_DATA)
                            Console.WriteLine("Client received init player data");

                        break;
                    case NetIncomingMessageType.StatusChanged:
                        // In case status changed
                        // It can be one of these
                        // NetConnectionStatus.Connected;
                        // NetConnectionStatus.Connecting;
                        // NetConnectionStatus.Disconnected;
                        // NetConnectionStatus.Disconnecting;
                        // NetConnectionStatus.None;

                        // NOTE: Disconnecting and Disconnected are not instant unless client is shutdown with disconnect()
                        Console.WriteLine(msg.SenderConnection.ToString() + " status changed to: " + msg.SenderConnection.Status);
                        if (msg.SenderConnection.Status == NetConnectionStatus.Disconnected || msg.SenderConnection.Status == NetConnectionStatus.Disconnecting)
                        {
                            EndGame(GetPlayerData(mePlayer == firstPlayer ? secondPlayer : firstPlayer), GameEnd.LEFT_GAME);
                        }
                        break;
                    default:
                        Console.WriteLine("Unhandled message type: " + msg.MessageType);
                        break;
                }
                peer.Recycle(msg);
            }
        }

        private void EndGame(PlayerData plr, GameEnd endType)
        {
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
                    canvas.Children.Remove(obj.GeometryElement);
            }));
        }

        public void AttachToScene(ISceneObject obj)
        {
            objects.Add(obj);
            GetUIDispatcher().BeginInvoke(DispatcherPriority.Send, new Action(() =>
            {
                canvas.Children.Add(obj.GeometryElement);
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
                canvas.Children.Remove(obj.GeometryElement);
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

        private void CreateNewPlayerData()
        {
            firstPlayer = randomGenerator.Next(2) == 0 ? PlayerPosition.LEFT : PlayerPosition.RIGHT;
            secondPlayer = firstPlayer == PlayerPosition.RIGHT ? PlayerPosition.LEFT : PlayerPosition.RIGHT;

            Base myBase = SceneObjectFactory.CreateBase(firstPlayer, randomGenerator.Next(2) == 0 ? Colors.Red : Colors.Blue);
            AttachToScene(myBase);

            PlayerData pd = new PlayerData();
            pd.SetBase(myBase);
            playerData.Add(pd);

            Base opponentsBase = SceneObjectFactory.CreateBase(secondPlayer, myBase.Color == Colors.Blue ? Colors.Red : Colors.Blue);
            AttachToScene(opponentsBase);

            pd = new PlayerData();
            pd.SetBase(opponentsBase);
            playerData.Add(pd);
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
                if (tpf > 0.001 && isInitialized)
                    Update(tpf);

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
            ProcessMessages();

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

        public PlayerData GetPlayerData(PlayerPosition pos)
        {
            foreach (PlayerData data in playerData)
            {
                if (data.GetPosition() == pos)
                    return data;
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

            if (GetPlayerData(mePlayer).IsMineReady())
            {
                GetPlayerData(mePlayer).UseMine();
                AttachToScene(SceneObjectFactory.CreateSingularityMine(point, GetPlayerData(mePlayer)));
            }
        }

        private void CheckPlayerStates()
        {
            if (GetPlayerData(firstPlayer).GetBaseIntegrity() <= 0)
                EndGame(GetPlayerData(secondPlayer), GameEnd.WIN_GAME);
            else if (GetPlayerData(secondPlayer).GetBaseIntegrity() <= 0)
                EndGame(GetPlayerData(firstPlayer), GameEnd.WIN_GAME);
        }

        private void PlayerWon(PlayerData winner)
        {
            GetUIDispatcher().Invoke(DispatcherPriority.Render, new Action(() =>
            {
                Label lbl = (Label)LogicalTreeHelper.FindLogicalNode(GetCanvas(), "lblEndGame");
                if (lbl != null)
                    lbl.Content = (winner.GetPlayerColor() == Colors.Red ? "Red" : "Blue") + " player won!";
            }));
        }


        private void PlayerLeft(PlayerData leaver)
        {
            GetUIDispatcher().Invoke(DispatcherPriority.Render, new Action(() =>
            {
                Label lbl = (Label)LogicalTreeHelper.FindLogicalNode(GetCanvas(), "lblEndGame");
                if (lbl != null)
                    lbl.Content = (leaver.GetPlayerColor() == Colors.Red ? "Red" : "Blue") + " player left the game!";
            }));
        }

        public void SetRemoteServerAddress(string serverAddress)
        {
            this.serverAddress = serverAddress;
        }
    }

}
