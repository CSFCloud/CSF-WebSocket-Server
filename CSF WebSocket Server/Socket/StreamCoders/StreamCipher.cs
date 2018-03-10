using CSFCloud.WebSocket.Socket.WsPackets;
using System.Collections.Generic;

namespace CSFCloud.WebSocket.Socket.StreamCoders {
    internal abstract class StreamCipher {

        public static StreamCipher Factory(string AcceptedExtensions) {
            List<string> options = new List<string>();

            string[] vaiants = AcceptedExtensions.Split("|");
            foreach (string s in vaiants) {
                string[] d = s.Split(";");
                string[] p = d[0].Split(",");
                foreach (string c in p) {
                    options.Add(c.Trim());
                }
            }

            foreach (string op in options) {
                if (op == "permessage-deflate") {
                    return new RCF6455();
                }
            }

            //throw new UnsupportedExtension();
            return new RCF6455();
        }

        public abstract byte[] Encode(WsPacket data);
        public abstract WsPacket Decode(byte[] data);

    }
}
