using CSFCloud.WebSocket.Socket;
using System;

namespace CSFCloud.WebSocket.ServerEventArgs {

    public class UserDosconnectEventArgs : EventArgs {

        public Client Client { get; }

        public UserDosconnectEventArgs(Client sid) {
            Client = sid;
        }

    }

}
