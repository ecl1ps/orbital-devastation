using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Windows;
using System.Reflection;
using System.Net;
using Lidgren.Network;
using Orbit.Core;
using MasterServer.Core;
using System.Threading;

namespace MasterServer
{
    public partial class App : Application
    {
        private static readonly log4net.ILog Logger = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        public volatile bool shouldQuit;
        private Thread serverThread;

        protected override void OnStartup(StartupEventArgs e)
        {
            log4net.Config.BasicConfigurator.Configure();

            serverThread = new Thread(new ThreadStart(RunServer));
            serverThread.IsBackground = false;
            serverThread.Name = "Master Server Thread";
            serverThread.Start();
        }

        protected override void OnExit(ExitEventArgs e)
        {
            if (serverThread != null)
                serverThread.Abort();
        }

        private void RunServer()
		{
            List<PeerData> registeredTournaments = new List<PeerData>();
            //List<PeerData> registeredSinglePlayers = new List<PeerData>();

            NetPeerConfiguration config = new NetPeerConfiguration("Orbit");
            config.SetMessageTypeEnabled(NetIncomingMessageType.UnconnectedData, true);
            config.Port = SharedDef.MASTER_SERVER_PORT_NUMBER;

            NetPeer server = new NetPeer(config);
            server.Start();

            while (!shouldQuit)
            {
                NetIncomingMessage msg;
                while ((msg = server.ReadMessage()) != null)
                {
                    switch (msg.MessageType)
                    {
                        case NetIncomingMessageType.UnconnectedData:
                            switch ((MasterServerPacketType)msg.ReadByte())
                            {
                                case MasterServerPacketType.REGISTER_TOURNAMENT:
                                    {
                                        PeerData pd = new PeerData(msg.ReadIPEndpoint(), msg.SenderEndpoint); 
                                        Logger.Info("Got tournament registration from host " + pd.ExternalIPEndPoint);
                                        registeredTournaments.Add(pd);
                                    }
                                    break;
                                case MasterServerPacketType.UNREGISTER_TOURNAMENT:
                                    foreach (PeerData tpd in registeredTournaments)
                                    {
                                        if (tpd.ExternalIPEndPoint.Equals(msg.SenderConnection))
                                        {
                                            registeredTournaments.Remove(tpd);
                                            Logger.Info("Tournament from " + tpd.ExternalIPEndPoint + " unregistered");
                                            break;
                                        }
                                    }
                                    break;
                                case MasterServerPacketType.UNREGISTER_HOST:
                                    // TODO
                                    break;
                                case MasterServerPacketType.REQUEST_QUICK_MATCH:
                                    // TODO
                                    break;
                                case MasterServerPacketType.REQUEST_TOURNAMENT_LIST:
                                    Logger.Info("Sending list of " + registeredTournaments.Count + " tournaments to client " + msg.SenderEndpoint);

                                    NetOutgoingMessage om = server.CreateMessage();
                                    om.Write((byte)MasterServerPacketType.RESPONSE_TOURNAMENT_LIST);
                                    om.Write(registeredTournaments.Count);

                                    foreach (PeerData p in registeredTournaments)
                                    {
                                        om.Write(p.InternalIPEndPoint);
                                        om.Write(p.ExternalIPEndPoint);
                                    }
                                    server.SendUnconnectedMessage(om, msg.SenderEndpoint);
                                    break;
                                case MasterServerPacketType.REQUEST_INTRODUCTION:
                                    IPEndPoint clientInternal = msg.ReadIPEndpoint();
                                    IPEndPoint hostExternal = msg.ReadIPEndpoint();
                                    string token = msg.ReadString();

                                    Logger.Info(msg.SenderEndpoint + " requesting introduction to " + hostExternal + " (token " + token + ")");

                                    foreach (PeerData tpd in registeredTournaments)
                                    {
                                        if (tpd.ExternalIPEndPoint.Equals(hostExternal))
                                        {
                                            Logger.Info("Sending introduction...");
                                            server.Introduce(
                                                tpd.InternalIPEndPoint,     // host internal
                                                tpd.ExternalIPEndPoint,     // host external
                                                clientInternal,             // client internal
                                                msg.SenderEndpoint,         // client external
                                                token                       // request token
                                            );
                                            break;
                                        }
                                    }
                                    break;
                            }
                            break;
                        case NetIncomingMessageType.DebugMessage:
                        case NetIncomingMessageType.VerboseDebugMessage:
                        case NetIncomingMessageType.WarningMessage:
                        case NetIncomingMessageType.ErrorMessage:
                            Logger.Info(msg.ReadString());
                            break;
                    }
                }
            }

            server.Shutdown("shutting down");
		}
    }
}
