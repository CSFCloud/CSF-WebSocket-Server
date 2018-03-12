namespace CSFCloud.WebSocket.Socket.Packets.PacketDatas {

    public class Hello : PacketData {

        public string session_id;
        public int heartbeat_interval;
        public int purge_timeout;
        public string message;

    }

}
