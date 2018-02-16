using CSFCloud.WebSocket.Loopers;
using CSFCloud.WebSocket.Util;
using CSFCloud.WebSocket.Socket.Packets;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Threading.Tasks;
using System;

namespace CSFCloud.WebSocket.Socket {

    internal class WSServer {

        private TcpListener server;
        private List<Looper> loops = new List<Looper>();
        private List<Client> clients = new List<Client>();

        private Action<Client, Packet> packetCallback;
        private Action<Client> clientConnectCallback;
        private Action<Client> clientDisconnectCallback;

        public WSServer(int port, Action<Client> cc, Action<Client, Packet> ac, Action<Client> dc) {
            clientConnectCallback = cc;
            packetCallback = ac;
            clientDisconnectCallback = dc;

            Logger.Info("Starting server...");
            server = new TcpListener(IPAddress.Parse("0.0.0.0"), port);
            server.Start();
            Logger.Info("Server started!");

            Looper l1 = new Looper(0, 0);
            l1.SetLoopFunction(Accept);
            loops.Add(l1);

            Looper l2 = new Looper(0, 0);
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

            Client client = new Client(tcpclient);
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
                            clientConnectCallback(client);
                        } else {
                            packetCallback(client, p);
                        }
                    }
                } else {
                    await client.Stop();
                    Logger.Info($"Client dropped [{client.GetSessionId()}]");
                    clientDisconnectCallback(client);
                    clients.RemoveAt(i);
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
