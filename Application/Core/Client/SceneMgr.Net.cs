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

#if DEBUG
            conf.SimulatedMinimumLatency = 0.1f; // 100ms
            conf.SimulatedRandomLatency = 0.05f; // +- 50ms

            conf.EnableMessageType(NetIncomingMessageType.DebugMessage);
            conf.EnableMessageType(NetIncomingMessageType.Error);
            conf.EnableMessageType(NetIncomingMessageType.ErrorMessage);
            conf.EnableMessageType(NetIncomingMessageType.Receipt);
            conf.EnableMessageType(NetIncomingMessageType.UnconnectedData);
            conf.EnableMessageType(NetIncomingMessageType.VerboseDebugMessage);
            conf.EnableMessageType(NetIncomingMessageType.WarningMessage);
#endif

            client = new NetClient(conf);
            client.Start();
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
                                    ClientConnectionConnected(msg); 
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

        private void ProcessIncomingDataMessage(NetIncomingMessage msg)
        {
            PacketType type = (PacketType)msg.ReadInt32();
#if DEBUG
            Console.WriteLine("Client " + GetCurrentPlayer().GetId() + ": received msg " + type.ToString());
#endif
            switch (type)
            {
                case PacketType.ALL_PLAYER_DATA:
                    ReceivedAllPlayerDataMsg(msg);
                    break;
                case PacketType.ALL_ASTEROIDS:
                    ReceivedAllAsteroidsMsg(msg);
                    break;
                case PacketType.NEW_ASTEROID:
                    ReceivedNewAsteroidMsg(msg);
                    break;
                case PacketType.MINOR_ASTEROID_SPAWN:
                    ReceivedMinorAsteroidSpawnMsg(msg);
                    break;
                case PacketType.NEW_STAT_POWERUP:
                    ReceivedNewStatPowerupMsg(msg);
                    break;
                case PacketType.NEW_SINGULARITY_MINE:
                    ReceivedNewSingularityMineMsg(msg);
                    break;
                case PacketType.NEW_SINGULARITY_BULLET:
                    ReceivedNewSingularityBulletMsg(msg);
                    break;
                case PacketType.NEW_SINGULARITY_EXPLODING_BULLET:
                    ReceivedNewSingularityExplodingBulletMsg(msg);
                    break;
                case PacketType.NEW_HOOK:
                    ReceivedNewHookMsg(msg);
                    break;
                case PacketType.SCORE_QUERY:
                    ReceivedScoreQueryMsg(msg);
                    break;
                case PacketType.PLAYER_WON:
                    ReceivedPlayerWonMsg(msg);
                    break;
                case PacketType.SINGULARITY_MINE_HIT:
                    ReceivedSingularityMineHitMsg(msg);
                    break;
                case PacketType.HOOK_HIT:
                    ReceivedHookHitMsg(msg);
                    break;
                case PacketType.BULLET_HIT:
                    ReceivedBulletHitMsg(msg);
                    break;
                case PacketType.BASE_INTEGRITY_CHANGE:
                    ReceivedBaseIntegrityChangeMsg(msg);
                    break;
                case PacketType.PLAYER_HEAL:
                    ReceivedPlayerHealMsg(msg);
                    break;
                case PacketType.PLAYER_SCORE:
                    ReceivedPlayerScoreMsg(msg);
                    break;
                case PacketType.START_GAME_RESPONSE:
                    ReceivedStartGameResponseMsg(msg);
                    break;
                case PacketType.TOURNAMENT_STARTING:
                    ReceivedTournamentStartingMsg(msg);
                    break;
                case PacketType.TOURNAMENT_FINISHED:
                    ReceivedTournamentFinishedMgs(msg);
                    break;
                case PacketType.PLAYER_READY:
                    ReceivedPlayerReadyMsg(msg);
                    break;
                case PacketType.CHAT_MESSAGE:
                    ShowChatMessage(msg.ReadString());
                    break;
                case PacketType.PLAYER_DISCONNECTED:
                    ReceivedPlayerDisconnectedMsg(msg);
                    break;
                case PacketType.SERVER_SHUTDOWN:
                    ReceivedServerShuttingDownMsg(msg);
                    break;
                case PacketType.PLAYER_RECEIVED_POWERUP:
                    ReceivedPlayerReceivedPowerUpMsg(msg);
                    break;
            }

        }

        private IGameState CreateBot(Player plr)
        {
            switch (plr.Data.BotType)
            {
                case BotType.LEVEL1:
                    return new SimpleBot(this, objects, plr);
                case BotType.LEVEL2:
                    return new HookerBot(this, objects, plr);
                case BotType.LEVEL3:
                case BotType.LEVEL4:
                case BotType.LEVEL5:
                default:
                    return null;
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
