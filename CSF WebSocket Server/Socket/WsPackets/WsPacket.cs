using CSFCloud.Utils;
using System;
using System.Collections.Generic;
using System.Text;

namespace CSFCloud.WebSocket.Socket.WsPackets {
    internal class WsPacket {

        private static Random random = new Random();
        private OpCode opcode;
        private string payload;
        private bool final = true;

        public WsPacket(OpCode opcode, string payload = "") {
            this.opcode = opcode;
            this.payload = payload;
        }

        public string GetPayload() {
            return payload;
        }

        public OpCode GetOpCode() {
            return opcode;
        }

        public bool IsFinal() {
            return final;
        }

    }
}
