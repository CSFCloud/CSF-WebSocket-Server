using CSFWebSocket.Socket;
using CSFWebSocket.Socket.Packets;
using System;

namespace CSFWebSocket.ServerEventArgs {
    public class PacketReceivedEventArgs : EventArgs {

        public Packet Packet { get; }
        public Client Client { get; }

        public PacketReceivedEventArgs(Client c, Packet p) {
            this.Client = c;
            this.Packet = p;
        }

    }
}
