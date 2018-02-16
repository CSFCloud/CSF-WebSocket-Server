using CSFCloud.WebSocket.Socket;
using CSFCloud.WebSocket.Socket.Packets;
using System;

namespace CSFCloud.WebSocket.ServerEventArgs {
    public class PacketReceivedEventArgs : EventArgs {

        public Packet Packet { get; }
        public Client Client { get; }

        public PacketReceivedEventArgs(Client c, Packet p) {
            this.Client = c;
            this.Packet = p;
        }

    }
}
