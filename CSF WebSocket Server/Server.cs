using CSFWebSocket.ServerEventArgs;
using CSFWebSocket.Socket;
using CSFWebSocket.Socket.Packets;
using System;

namespace CSFWebSocket {

    public class Server {

        private WSServer socket;
        public event EventHandler<UserConnectEventArgs> OnUserConnect;
        public event EventHandler<PacketReceivedEventArgs> OnPacketReceived;
        public event EventHandler<UserDosconnectEventArgs> OnUserDisconnect;

        public Server(int port) {
            socket = new WSServer(port, EnvokeUserConnect, EnvokePacketReceived, EnvokeUserDisconnect);
        }

        public Client[] GetClientList() {
            return socket.GetClientList().ToArray();
        }

        private void EnvokeUserConnect(Client c) {
            OnUserConnect?.Invoke(this, new UserConnectEventArgs(c));
        }

        private void EnvokePacketReceived(Client c, Packet p) {
            OnPacketReceived?.Invoke(this, new PacketReceivedEventArgs(c, p));
        }

        private void EnvokeUserDisconnect(Client c) {
            OnUserDisconnect?.Invoke(this, new UserDosconnectEventArgs(c));
        }

    }

}
