using CSFCloud.WebSocket.Socket.Headers;
using CSFCloud.WebSocket.Socket.Packets;
using CSFCloud.WebSocket.Socket.Packets.PacketDatas;
using CSFCloud.WebSocket.Socket.WsPackets;
using CSFCloud.WebSocket.Util;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace CSFCloud.WebSocket.Socket {
    public class Client {

        private static SortedSet<string> UsedSessions = new SortedSet<string>();
        private static Random random = new Random();
        private TcpClient tcpclient;
        private bool stillOk = true;
        private long lastHeartBeat;
        private string session;
        private int PacketSequence = 0;
        private Identity identity = null;
        private RequestHeader requestHeader;

        public int Available {
            get {
                return tcpclient.Available;
            }
        }

        public Client(TcpClient tc) {
            this.session = GenerateSessionId();
            tcpclient = tc;
            HeartBeatNow();
        }

        public void HeartBeatNow() {
            lastHeartBeat = ServerTime.GetCurrentUnixTimestampMillis();
        }

        public bool IsAlive() {
            if (!tcpclient.Connected) {
                return false;
            }
            if (!stillOk) {
                return false;
            }
            long currentTime = ServerTime.GetCurrentUnixTimestampMillis();
            return (currentTime - lastHeartBeat) < 60 * 1000;
        }

        private async Task Send(HttpHeader head) {
            string str = head.Serialize();
            await Send(str);
        }

        private async Task Send(string str) {
            NetworkStream stream = GetStream();
            byte[] buffer = Encoding.UTF8.GetBytes(str);
            await stream.WriteAsync(buffer, 0, buffer.Length);

            Logger.Debug($"Data sent: {str}");
        }

        public async Task Send(Packet packet) {
            string str = packet.Serialize(PacketSequence);
            PacketSequence++;
            WsPacket wspacket = new WsPacket(OpCode.TextFrame, str);

            Logger.Debug($"Sending packet: {str}");
            await Send(wspacket);
        }

        private async Task Send(WsPacket packet) {
            NetworkStream stream = GetStream();
            byte[] buffer = packet.Encode();
            await stream.WriteAsync(buffer, 0, buffer.Length);

            Logger.Debug($"WebSocket Raw Packet sent!");
        }

        public NetworkStream GetStream() {
            return tcpclient.GetStream();
        }

        public async Task<Packet> Listen() {
            NetworkStream stream = GetStream();
            if (stream.DataAvailable) {
                Byte[] bytes = new Byte[tcpclient.Available];
                await stream.ReadAsync(bytes, 0, bytes.Length);
                return await ProcessPacket(bytes);
            }
            return null;
        }

        private async Task<Packet> ProcessPacket(byte[] data) {
            string str = Encoding.UTF8.GetString(data);
            if (Regex.IsMatch(str, "^GET")) {
                requestHeader = new RequestHeader(str);
                Logger.Info($"Requesting resource: {requestHeader.resource}");

                string ip1 = tcpclient.Client.RemoteEndPoint.ToString();
                string ip2 = tcpclient.Client.LocalEndPoint.ToString();

                string[] ip_parts = ip1.Split(".");

                Logger.Info($"IP: {ip1} {ip2}");

                if (ip_parts[0] != "172" && ip_parts[0] != "127") {
                    ResponseHeader respheader = new ResponseHeader {
                        responseCode = HttpCodes.OK
                    };
                    await Send(respheader);
                    await Send("Hello World!");
                    stillOk = false;
                } else if (requestHeader.parameters.ContainsKey("Sec-WebSocket-Key")) {
                    ResponseHeader respheader = new ResponseHeader {
                        responseCode = HttpCodes.SwitchingProtocols
                    };
                    respheader.parameters["Connection"] = "Upgrade";
                    respheader.parameters["Upgrade"] = "websocket";
                    respheader.parameters["Sec-WebSocket-Accept"] = GenerateSecWebSocketKey(requestHeader.parameters["Sec-WebSocket-Key"]);
                    respheader.parameters["x-session-id"] = session;
                    await Send(respheader);

                    await SendHello();
                } else {
                    ResponseHeader respheader = new ResponseHeader {
                        responseCode = HttpCodes.BadRequest
                    };
                    await Send(respheader);
                    stillOk = false;
                }
            } else {
                WsPacket WebSocketPacket = new WsPacket(data);
                OpCode opcode = WebSocketPacket.GetOpCode();
                if (opcode == OpCode.TextFrame) {
                    string pstr = WebSocketPacket.GetPayload();
                    try {
                        Packet p = new Packet(pstr);
                        PacketType type = p.GetPacketType();

                        if (type == PacketType.Heartbeat) {
                            HeartBeatNow();
                            Packet ack = new Packet(PacketType.HeartbeatACK);
                            await Send(ack);
                            return null;
                        } else if (type == PacketType.Reconnect) {
                            Reconnect rp = p.GetPacketData<Reconnect>();
                            if (UsedSessions.Contains(rp.session_id)) {
                                this.session = rp.session_id;
                                await SendHello();
                            } else {
                                Packet invsess = new Packet(PacketType.InvalidSession);
                                await Send(invsess);
                                stillOk = false;
                            }
                            return null;
                        } else if (type == PacketType.Identify) {
                            identity = p.GetPacketData<Identity>();
                        } else {
                            if (identity == null) {
                                stillOk = false;
                                return null;
                            }
                        }

                        return p;
                    } catch (Exception e) {
                        Logger.Error($"Packet conversion error: {e.Message}");
                        Logger.Error($"Receied data: {string.Join(" ", data)}");
                        Logger.Error($"Decoded: {pstr}");
                        Logger.Error($"{e.StackTrace}");
                    }
                } else if (opcode == OpCode.Ping) {
                    Logger.Debug("Ping Pong");
                    WsPacket pong = new WsPacket(OpCode.Pong);
                    await Send(pong);
                }
            }
            return null;
        }

        private async Task SendHello() {
            Packet p = new Packet(PacketType.Hello, new Hello() {
                session_id = session,
                message = "I'm not a teapot"
            });
            await Send(p);
        }

        private static string GenerateSecWebSocketKey(string inputKey) {
            string newKey = inputKey + "258EAFA5-E914-47DA-95CA-C5AB0DC85B11";
            byte[] keyBytes = Encoding.UTF8.GetBytes(newKey);
            byte[] hash = SHA1.Create().ComputeHash(keyBytes);
            string finalKey = Convert.ToBase64String(hash);
            return finalKey;
        }

        public async Task Stop() {
            if (tcpclient.Connected) {
                WsPacket p = new WsPacket(OpCode.ConnectionClose);
                await Send(p);
                tcpclient.Close();
            }
        }

        public string GetSessionId() {
            return session;
        }

        private static string GenerateSessionId() {
            const int length = 32;
            const string chars = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";

            string session;
            do {
                string rand = (new string(Enumerable.Repeat(chars, length).Select(s => s[random.Next(s.Length)]).ToArray()));
                byte[] rawbites = Encoding.UTF8.GetBytes(ServerTime.GetCurrentUnixTimestampMillis() + "." + rand);
                byte[] hash = SHA1.Create().ComputeHash(rawbites);
                session = Convert.ToBase64String(hash);
            } while (UsedSessions.Contains(session));

            UsedSessions.Add(session);

            return session;
        }

        public string GetRequestedResources() {
            return (requestHeader?.resource);
        }

    }
}
