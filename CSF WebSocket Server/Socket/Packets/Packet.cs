using CSFCloud.Utils;
using CSFCloud.WebSocket.Socket.Packets.PacketDatas;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;

namespace CSFCloud.WebSocket.Socket.Packets {

    public class Packet {

        private PacketType type = PacketType.Undefined;
        private object data = null;

        public Packet(PacketData data = null) : this(PacketType.Custom, data) {}

        public Packet(PacketType type, PacketData data = null) {
            this.type = type;
            this.data = data;
        }

        public Packet(string str) {
            JObject jd = JsonConvert.DeserializeObject<JObject>(str);
            if (jd["type"] == null) {
                throw new Exception($"Missing type");
            }
            try {
                type = (PacketType)((int)jd["type"]);
            } catch {
                throw new Exception($"Invalid type [{jd["type"]}]");
            }
            data = jd["data"];
        }

        public string Serialize(int seq) {
            PacketContainer cont = new PacketContainer() {
                type = (int)type,
                data = data,
                created = ServerTime.GetCurrentUnixTimestampMillis(),
                s = seq
            };

            return JsonConvert.SerializeObject(cont);
        }

        public PacketType GetPacketType() {
            return type;
        }

        public T GetPacketData<T>() {
            return JsonConvert.DeserializeObject<T>(JsonConvert.SerializeObject(data));
        }

        private class PacketContainer {
            public int type;
            public object data;
            public long created;
            public int s;
        }

    }

}
