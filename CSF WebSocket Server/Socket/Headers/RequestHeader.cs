using System;

namespace CSFWebSocket.Socket.Headers {

    class RequestHeader : HttpHeader {

        public string method = "GET";
        public string resource = "/";
        public string protocol = "HTTP/1.1";

        public RequestHeader(string str) : base(str) { }
        public RequestHeader() : base() { }


        protected override void DeserializeStartLine(string line) {
            string[] words = line.Split(" ");
            if (words.Length != 3) {
                throw new Exception("Invalid input");
            }

            method = words[0];
            resource = words[1];
            protocol = words[2];
        }

        protected override string SerializeStartLine() {
            return method + " " + resource + " " + protocol;
        }
    }

}
