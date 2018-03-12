using CSFCloud.WebSocket.Socket;
using System;
using System.Collections.Generic;
using System.Text;

namespace CSFCloud.WebSocket.ServerEventArgs {
    public class UserConnectionBackEventArgs : EventArgs {

        public Client Client { get; }

        public UserConnectionBackEventArgs(Client Client) {
            this.Client = Client;
        }

    }
}
