using CSFCloud.WebSocket.Socket;
using System;
using System.Collections.Generic;
using System.Text;

namespace CSFCloud.WebSocket.ServerEventArgs {
    public class UserTemporaryConnectionLostEventArgs : EventArgs {

        public Client Client { get; }

        public UserTemporaryConnectionLostEventArgs(Client Client) {
            this.Client = Client;
        }

    }
}
