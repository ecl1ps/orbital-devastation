using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Controls;
using Lidgren.Network;
using System.Threading;
using System.Net;

namespace Orbit.Core.Net
{
    public class NetSearcher
    {
        private static readonly log4net.ILog Logger = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        public ListBox ServerListBox { get; set; }
        private NetClient client;
        private volatile bool shouldQuit;
        private Thread current;
        private IPEndPoint server;

        public void Run()
        {
            current = Thread.CurrentThread;
            StartClient();

            while (!shouldQuit)
            {
                ReadMessages();

                Thread.Sleep(10);
            }
        }

        private void StartClient()
        {
            NetPeerConfiguration conf = new NetPeerConfiguration("Orbit");
            conf.EnableMessageType(NetIncomingMessageType.DiscoveryResponse);
            conf.EnableMessageType(NetIncomingMessageType.UnconnectedData);
            conf.EnableMessageType(NetIncomingMessageType.NatIntroductionSuccess);
            client = new NetClient(conf);
            client.Start();
        }

        private void ReadMessages()
        {
            NetIncomingMessage msg;
            while ((msg = client.ReadMessage()) != null && !shouldQuit)
            {
                switch (msg.MessageType)
                {
                    case NetIncomingMessageType.VerboseDebugMessage:
                    case NetIncomingMessageType.DebugMessage:
                    case NetIncomingMessageType.WarningMessage:
                    case NetIncomingMessageType.ErrorMessage:
                        Logger.Debug(msg.ReadString());
                        break;
                    case NetIncomingMessageType.DiscoveryResponse:
                        string player = msg.ReadString();
                        AddServer("LAN: " + player + " (" + msg.SenderEndpoint.Address.ToString() + ")");
                        break;
                    case NetIncomingMessageType.UnconnectedData:
                        switch ((MasterServerPacketType)msg.ReadByte())
                        {
                            case MasterServerPacketType.RESPONSE_TOURNAMENT_LIST:
                                {
                                    int count = msg.ReadInt32();
                                    for (int i = 0; i < count; ++i)
                                    {
                                        IPEndPoint hostInternal = msg.ReadIPEndpoint();
                                        IPEndPoint hostExternal = msg.ReadIPEndpoint();

                                        AddServer("SERVER: (" + hostExternal.ToString() + ")");
                                    }
                                    Logger.Info("Received " + count + " records");
                                }
                                break;
                        }
                        break;
                    default:
                        break;
                }
                client.Recycle(msg);
            }
        }

        private void RequestTournamentList()
        {
            server = new IPEndPoint(NetUtility.Resolve(SharedDef.MASTER_SERVER_ADDRESS), SharedDef.MASTER_SERVER_PORT_NUMBER);

            NetOutgoingMessage listRequest = client.CreateMessage();
            listRequest.Write((byte)MasterServerPacketType.REQUEST_TOURNAMENT_LIST);
            client.SendUnconnectedMessage(listRequest, server);
        }

        private void AddServer(string s)
        {
            if (ServerListBox == null)
            {
                Shutdown();
                return;
            }

            ServerListBox.Dispatcher.Invoke(new Action(() =>
            {
                ServerListBox.Items.Add(s);
            }));
        }

        public void Shutdown()
        {
            if (client != null)
                client.Shutdown("");
            /*if (current != null && current.IsAlive)
                current.Interrupt();*/
            shouldQuit = true;
        }

        public void StartNewSearch()
        {
            ServerListBox.Dispatcher.Invoke(new Action(() =>
            {
                ServerListBox.Items.Clear();
            }));
            Thread.Sleep(30);
            if (client != null && (client.Status == NetPeerStatus.Running || client.Status == NetPeerStatus.Starting))
            {
                client.DiscoverLocalPeers(SharedDef.PORT_NUMBER);
                RequestTournamentList();
            }
        }
    }
}
