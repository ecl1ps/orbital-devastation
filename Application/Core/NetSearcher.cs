using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Controls;
using Lidgren.Network;
using System.Threading;

namespace Orbit.Core
{
    public class NetSearcher
    {
        private static readonly log4net.ILog Logger = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        public ListBox ServerListBox { get; set; }
        private NetClient client;
        private volatile bool shouldQuit;
        private Thread current;

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
                        Gametype type = (Gametype)msg.ReadByte();
                        string player = msg.ReadString();
                        AddServer(player + " - " + type.ToString() + " (" + msg.SenderEndpoint.Address.ToString() + ")");
                        break;
                    default:
                        break;
                }
                client.Recycle(msg);
            }
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
                client.DiscoverLocalPeers(SharedDef.PORT_NUMBER);
        }
    }
}
