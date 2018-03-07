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

        public WsPacket(byte[] input) {
            Decode(input);
        }

        public byte[] Encode() {
            List<byte> data = new List<byte> {
                (byte)(128 + opcode)
            };

            data.AddRange(CalculateSize());

            /*List<byte> key = GenerateXORKey();
            data.AddRange(key);
            data.AddRange(Encrypt(key));*/

            data.AddRange(Encoding.UTF8.GetBytes(payload));

            return data.ToArray();
        }

        private List<byte> CalculateSize() {
            List<byte> leng = new List<byte>();

            if (payload.Length <= 125) {
                leng.Add((byte)(payload.Length));
            } else if (payload.Length <= Math.Pow(2, 2 * 8)) {
                leng.Add((byte)(126));
                leng.Add((byte)(payload.Length / Math.Pow(2, 8)));
                leng.Add((byte)(payload.Length % Math.Pow(2, 8)));
            } else {
                byte[] lengthbytes = new byte[8];

                int l = payload.Length;
                int mod = (int)Math.Pow(2, 8);

                for (int i = 0; i < 8; i++) {
                    lengthbytes[7 - i] = (byte)(l % mod);
                    l /= mod;
                }

                leng.AddRange(lengthbytes);
            }

            return leng;
        }

        private List<byte> GenerateXORKey() {
            return new List<byte> {
                (byte)random.Next(0, 255),
                (byte)random.Next(0, 255),
                (byte)random.Next(0, 255),
                (byte)random.Next(0, 255)
            };
        }

        private List<byte> Encrypt(List<byte> key) {
            List<byte> output = new List<byte>();
            byte[] input = Encoding.UTF8.GetBytes(payload);

            for (int i = 0; i < input.Length; i++) {
                output.Add((byte)(input[i] ^ key[i % key.Count]));
            }

            return output;
        }

        private void Decode(byte[] input) {
            opcode = (OpCode)(input[0] % 16);
            final = (input[0] / 128) % 2 == 1;
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

            payload = Encoding.UTF8.GetString(output);
        }

        private int GetLengthFromBytes(byte[] input, int offset, int lengthByteCount) {
            int length = 0;

            for (int i = 0; i < lengthByteCount; i++) {
                length = length << 8;
                length += input[offset + i];
            }

            return length;
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
