using System;

namespace CSFCloud.WebSocket.Socket.Headers {
    class ResponseHeader : HttpHeader {

        public string protocol = "HTTP/1.1";
        public HttpCodes responseCode = HttpCodes.OK;

        public ResponseHeader(string str) : base(str) { }
        public ResponseHeader() : base() { }

        protected override void DeserializeStartLine(string line) {
            string[] words = line.Split(" ");
            if (words.Length < 2) {
                throw new Exception("Invalid input");
            }
            protocol = words[0];
            responseCode = (HttpCodes)int.Parse(words[1]);
        }

        protected override string SerializeStartLine() {
            return protocol + " " + ((int)responseCode);
        }
    }
}
