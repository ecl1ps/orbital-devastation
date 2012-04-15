using System;
using Lidgren.Network;

namespace Orbit.Core.Scene.Entities
{
    public interface ISendable
    {
        void WriteObject(NetOutgoingMessage msg);

        void ReadObject(NetIncomingMessage msg);
    }
}
