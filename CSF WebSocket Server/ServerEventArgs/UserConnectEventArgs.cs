using CSFCloud.WebSocket.Socket;
using System;

namespace CSFCloud.WebSocket.ServerEventArgs {

    public class UserConnectEventArgs : EventArgs {

        public Client Client { get; }

        public UserConnectEventArgs(Client Client) {
            this.Client = Client;
        }

    }

}
