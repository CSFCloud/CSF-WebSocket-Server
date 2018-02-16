using CSFWebSocket.Socket;
using System;

namespace CSFWebSocket.ServerEventArgs {

    public class UserConnectEventArgs : EventArgs {

        public Client Client { get; }

        public UserConnectEventArgs(Client Client) {
            this.Client = Client;
        }

    }

}
