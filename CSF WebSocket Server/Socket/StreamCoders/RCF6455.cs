using CSFCloud.Utils;
using CSFCloud.WebSocket.Socket.WsPackets;
using System;
using System.Collections.Generic;
using System.Text;

namespace CSFCloud.WebSocket.Socket.StreamCoders {
    class RCF6455 : StreamCipher {

        public override WsPacket Decode(byte[] input) {
            OpCode opcode = (OpCode)(input[0] % 16);
            bool final = (input[0] / 128) % 2 == 1;

            byte lengthIndicator = (byte)(input[1] - 128);
            long length = 0;
            int nextByte = 0;

            if (lengthIndicator <= 125) {
                length = lengthIndicator;
                nextByte = 2;
            } else if (lengthIndicator == 126) {
                // 2 byte length
                length = GetLengthFromBytes(input, 2, 2);
                nextByte = 4;
            } else if (lengthIndicator == 127) {
                // 8 byte length
                length = GetLengthFromBytes(input, 2, 8);
                nextByte = 10;
            }

            byte[] key = new byte[4];
            key[0] = input[nextByte];
            key[1] = input[nextByte + 1];
            key[2] = input[nextByte + 2];
            key[3] = input[nextByte + 3];
            nextByte += 4;

            byte[] output = new byte[length];

            for (int i = 0; i < length; i++) {
                output[i] = (byte)(input[nextByte + i] ^ key[i % 4]);
            }

            string payload = Encoding.UTF8.GetString(output);

            return new WsPacket(opcode, payload);
        }

        public override byte[] Encode(WsPacket data) {
            List<byte> result = new List<byte> {
                (byte)(128 + data.GetOpCode())
            };

            byte[] payloadBytes = Encoding.UTF8.GetBytes(data.GetPayload());

            result.AddRange(CalculateSize(payloadBytes.Length));
            result.AddRange(payloadBytes);
            //Logger.Debug(string.Join(" ", result));

            return result.ToArray();
        }

        private List<byte> CalculateSize(int size) {
            List<byte> leng = new List<byte>();

            if (size <= 125) {
                leng.Add((byte)(size));
            } else if (size <= Math.Pow(2, 2 * 8)) {
                leng.Add((byte)(126));
                leng.Add((byte)(size / Math.Pow(2, 8)));
                leng.Add((byte)(size % Math.Pow(2, 8)));
            } else {
                byte[] lengthbytes = new byte[8];

                int l = size;
                int mod = (int)Math.Pow(2, 8);

                for (int i = 0; i < 8; i++) {
                    lengthbytes[7 - i] = (byte)(l % mod);
                    l /= mod;
                }

                leng.AddRange(lengthbytes);
            }

            return leng;
        }

        private int GetLengthFromBytes(byte[] input, int offset, int lengthByteCount) {
            int length = 0;

            for (int i = 0; i < lengthByteCount; i++) {
                length = length << 8;
                length += input[offset + i];
            }

            return length;
        }

    }
}
