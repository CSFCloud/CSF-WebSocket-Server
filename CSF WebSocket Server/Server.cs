using CSFCloud.Utils;
using CSFCloud.WebSocket.ServerEventArgs;
using CSFCloud.WebSocket.Socket;
using CSFCloud.WebSocket.Socket.Packets;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Threading.Tasks;

namespace CSFCloud.WebSocket {

    public class Server {

        private TcpListener server;
        private List<Looper> loops = new List<Looper>();
        private List<Client> clients = new List<Client>();

        public event EventHandler<UserConnectEventArgs> OnUserConnect;
        public event EventHandler<PacketReceivedEventArgs> OnPacketReceived;
        public event EventHandler<UserDosconnectEventArgs> OnUserDisconnect;
        //public event EventHandler<UserTemporaryConnectionLostEventArgs> OnUserTemporaryConnectionLost;
        //public event EventHandler<UserConnectionBackEventArgs> OnUserConnectionBack;

        public int HeartBeatInterval = 30 * 1000;
        public int PurteTimeout = 5 * 60 * 1000;

        public Server(int port = 80) {
            //socket = new WSServer(port, EnvokeUserConnect, EnvokePacketReceived, EnvokeUserDisconnect);

            Logger.Info("Starting server...");
            server = new TcpListener(IPAddress.Parse("0.0.0.0"), port);
            server.Start();
            Logger.Info("Server started!");

            Looper l1 = new Looper(1000, 100);
            l1.SetLoopFunction(Accept);
            loops.Add(l1);

            Looper l2 = new Looper(100, 10);
            l2.SetLoopFunction(Listen);
            loops.Add(l2);

            foreach (Looper l in loops) {
                l.Start();
            }

            Logger.Info("Loops started!");
        }

        private async Task Accept() {
            TcpClient tcpclient = await server.AcceptTcpClientAsync();

            string ip = tcpclient.Client.RemoteEndPoint.ToString();

            Logger.Debug($"New client connected! {ip}");

            Client client = new Client(tcpclient, HeartBeatInterval, PurteTimeout);
            clients.Add(client);
        }

        private async Task Listen() {
            List<string> removeList = new List<string>();

            for (int i = clients.Count - 1; i >= 0; i--) {
                Client client = clients[i];

                if (client.IsAlive()) {
                    Packet p = await client.Listen();
                    if (p != null) {
                        PacketType type = p.GetPacketType();
                        if (type == PacketType.Identify) {
                            OnUserConnect.Invoke(this, new UserConnectEventArgs(client));
                        } else {
                            OnPacketReceived.Invoke(this, new PacketReceivedEventArgs(client, p));
                        }
                    }
                } else {
                    await client.Stop();
                    Logger.Info($"Client dropped [{client.GetSessionId()}]");
                    clients.RemoveAt(i);
                    string sess = client.GetSessionId();

                    bool onlyone = true;
                    for (int j = clients.Count - 1; j >= 0; j--) {
                        if (clients[j].GetSessionId() == sess) {
                            onlyone = false;
                            break;
                        }
                    }

                    if (onlyone) {
                        OnUserDisconnect.Invoke(this, new UserDosconnectEventArgs(client));
                    }
                }
            }
        }

        public async Task Stop() {
            foreach (Client client in clients) {
                await client.Stop();
            }
            server.Stop();
        }

        public List<Client> GetClientList() {
            return clients;
        }

    }

}
