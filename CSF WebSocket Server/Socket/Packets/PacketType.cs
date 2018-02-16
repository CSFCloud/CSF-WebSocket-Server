namespace CSFCloud.WebSocket.Socket.Packets {

    public enum PacketType {

        Undefined = -1,
        Hello = 0,
        Heartbeat = 1,
        HeartbeatACK = 2,
        Reconnect = 3,
        Identify = 4,
        InvalidSession = 5,

        Custom = 100

    }

}
