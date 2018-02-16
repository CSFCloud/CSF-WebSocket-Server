using CSFWebSocket.Socket;
using System;

namespace CSFWebSocket.ServerEventArgs {

    public class UserDosconnectEventArgs : EventArgs {

        public Client Client { get; }

        public UserDosconnectEventArgs(Client sid) {
            Client = sid;
        }

    }

}
