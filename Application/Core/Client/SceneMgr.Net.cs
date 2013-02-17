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
using System.Windows.Threading;
using System.Collections.Concurrent;
using Lidgren.Network;
using Orbit.Core.Scene.Entities.Implementations;
using Orbit.Core.Scene.Controls.Implementations;
using Orbit.Core.Helpers;
using Orbit.Core.AI;
using Orbit.Core.Client.GameStates;
using Orbit.Core.Scene.Controls.Health.Implementations;
using Orbit.Core.Scene.Controls.Health;
using Orbit.Gui;

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
            /*conf.SimulatedMinimumLatency = 0.1f; // 100ms
            conf.SimulatedRandomLatency = 0.05f; // +- 50ms*/

            conf.EnableMessageType(NetIncomingMessageType.DebugMessage);
            conf.EnableMessageType(NetIncomingMessageType.Error);
            conf.EnableMessageType(NetIncomingMessageType.ErrorMessage);
            conf.EnableMessageType(NetIncomingMessageType.Receipt);
            conf.EnableMessageType(NetIncomingMessageType.UnconnectedData);
            conf.EnableMessageType(NetIncomingMessageType.WarningMessage);
#endif
#if VERBOSE
            conf.EnableMessageType(NetIncomingMessageType.VerboseDebugMessage);
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
                        Logger.Debug(msg.ReadString());
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
                        switch ((NetConnectionStatus)msg.ReadByte())
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
                                string reason = msg.ReadString();
                                if (reason == "kicked") // musi byt delayed - az v dalsim updatu - jinak zavreni oken premaze i info okno
                                    Enqueue(new Action(() => Application.Current.Dispatcher.Invoke(
                                        new Action(() => App.Instance.AddMenu(new InfoUC(Strings.ui_warning_kicked))))));
                                EndGame(null, GameEnd.SERVER_DISCONNECTED);
                                break;
                        }

                        // NOTE: Disconnecting and Disconnected are not instant unless client is shutdown with disconnect()
                        Logger.Debug(msg.SenderConnection.ToString() + " status changed to: " + msg.SenderConnection.Status);
                        break;
                    default:
                        Logger.Warn("Unhandled message type: " + msg.MessageType);
                        break;
                }
                client.Recycle(msg);
            }
        }

        private void ProcessIncomingDataMessage(NetIncomingMessage msg)
        {
            PacketType type = (PacketType)msg.ReadInt32();
#if VERBOSE
            Logger.Debug("Client " + GetCurrentPlayer().GetId() + ": received msg " + type.ToString());
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
                case PacketType.NEW_SINGULARITY_BOUNCING_BULLET:
                    ReceivedNewSingularityBouncingBulletMsg(msg);
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
                case PacketType.HOOK_FORCE_PULL:
                    ReceivedHookForcePullMsg(msg);
                    break;
                case PacketType.BULLET_HIT:
                    ReceivedBulletHitMsg(msg);
                    break;
                case PacketType.REMOVE_OBJECT:
                    ReceiveRemoveObject(msg);
                    break;
                case PacketType.BASE_INTEGRITY_CHANGE:
                    ReceivedBaseIntegrityChangeMsg(msg);
                    break;
                case PacketType.PLAYER_HEAL:
                    ReceivedPlayerHealMsg(msg);
                    break;
                case PacketType.PLAYER_SCORE_AND_GOLD:
                    ReceivedPlayerAndGoldScoreMsg(msg);
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
                case PacketType.PLAYER_RECONNECTED:
                    ReceivedPlayerReconnectedMsg(msg);
                    break;
                case PacketType.TOURNAMENT_SETTINGS:
                    ReceivedTournamentSettingsMsg(msg);
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
                case PacketType.PLAYER_BOUGHT_UPGRADE:
                    ReceivedPlayerBoughtUpgradeMsg(msg);
                    break;
                case PacketType.FLOATING_TEXT:
                    ReceivedFloatingTextMsg(msg);
                    break;
                case PacketType.PLAY_SOUND:
                    SoundManager.Instance.ReadSoundMessage(msg);
                    break;
                case PacketType.MOVE_STATE_CHANGED:
                    ChangeMoveState(msg);
                    break;
                case PacketType.MINING_MODULE_DMG_TAKEN:
                    ReceiveModuleDamage(msg);
                    break;
                case PacketType.ASTEROIDS_DIRECTIONS_CHANGE:
                    ReceiveAsteroidsDirectionChange(msg);
                    break;
                case PacketType.PLAYER_SCORE_UPDATE:
                    ReceivedPlayerScoreUpdate(msg);
                    break;
                case PacketType.OBJECTS_TAKE_DAMAGE:
                    ReceiveObjectsDamage(msg);
                    break;
                case PacketType.OBJECTS_HEAL_AMOUNT:
                    ReceiveObjectsHeal(msg);
                    break;
                case PacketType.MODULE_COLOR_CHANGE:
                    ReceiveModuleColorChange(msg);
                    break;
                case PacketType.PLAYER_COLOR_CHANGED:
                    ReceivePlayerColorChange(msg);
                    break;
                case PacketType.SPECTATOR_ACTION_START:
                    ReceivedSpectatorActionStarted(msg);
                    break;
                case PacketType.SHOW_ALLERT_MESSAGE:
                    AlertMessageMgr.ReceiveShowMessage(msg);
                    break;
                case PacketType.SCHEDULE_SPECTATOR_ACTION:
                    ReceivedActionScheduleMsg(msg);
                    break;
                case PacketType.PARTICLE_EMMITOR_CREATE:
                    ReceivedEmmitorSpawn(msg);
                    break;
                default:
                    Logger.Warn("Received unhandled packet type: " + type);
                    break;
            }
        }

        private void CreateAndAddBot(Player plr)
        {
            if (plr.Data.BotType == BotType.NONE || !plr.IsActivePlayer())
                return;

            // jinak budou provadet update na vice klientech - n-nasobek akci
            if (GameType == Gametype.TOURNAMENT_GAME && !GetCurrentPlayer().Data.LobbyLeader)
                return;

            switch (plr.Data.BotType)
            {
                case BotType.LEVEL1:
                    StateMgr.AddGameState(new SimpleBot(this, objects, plr));
                    break;
                case BotType.LEVEL2:
                    StateMgr.AddGameState(new HookerBot(this, objects, plr));
                    break;
                case BotType.LEVEL3:
                     StateMgr.AddGameState(new MedicoreBot(this, objects, plr));
                    break;
                case BotType.LEVEL4:
                case BotType.LEVEL5:
                default:
                    break;
            }
        }

        private Asteroid CreateNewAsteroid(AsteroidType asteroidType)
        {
            Asteroid asteroid;
            switch (asteroidType)
            {
                case AsteroidType.GOLDEN:
                    asteroid = new Asteroid(this, -1);
                    break;
                case AsteroidType.NORMAL:
                    asteroid = new Asteroid(this, -1);
                    break;
                case AsteroidType.SPAWNED:
                    asteroid = new MinorAsteroid(this, -1);
                    break;
                case AsteroidType.UNSTABLE:
                    asteroid = new UnstableAsteroid(this, -1);
                    break;
                default:
                    asteroid = new Asteroid(this, -1);
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
