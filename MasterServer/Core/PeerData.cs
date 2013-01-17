using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;

namespace MasterServer.Core
{
    public class PeerData
    {
        public IPEndPoint InternalIPEndPoint { get; set; }
        public IPEndPoint ExternalIPEndPoint { get; set; }

        public PeerData(IPEndPoint internalIP, IPEndPoint externalIP)
        {
            InternalIPEndPoint = internalIP;
            ExternalIPEndPoint = externalIP;
        }
    }
}
