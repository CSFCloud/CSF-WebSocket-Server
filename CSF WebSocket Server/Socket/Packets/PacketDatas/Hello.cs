namespace CSFCloud.WebSocket.Socket.Packets.PacketDatas {

    public class Hello : PacketData {

        public string session_id;
        public int heartbeat_interval = 5 * 60 * 1000;
        public string message;

    }

}
