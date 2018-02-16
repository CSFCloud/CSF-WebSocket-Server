﻿using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace CSFWebSocket.Socket.Headers {

    internal abstract class HttpHeader {

        public Dictionary<string, string> parameters = new Dictionary<string, string>();

        public HttpHeader(string data) {
            string[] lines = data.Split("\r\n");
            Deserialize(lines);
        }

        public HttpHeader() {}

        public void Deserialize(string[] lines) {
            DeserializeStartLine(lines[0]);

            parameters.Clear();
            for (int i = 1; i < lines.Length; i++) {
                if (lines[i] != "") {
                    Match m = new Regex("(.*): (.*)").Match(lines[i]);
                    string key = m.Groups[1].Value.Trim();
                    string value = m.Groups[2].Value.Trim();

                    parameters[key] = value;
                }
            }
        }

        public string Serialize() {
            string str = "";

            str += SerializeStartLine() + "\r\n";
            foreach (string key in parameters.Keys) {
                str += key + ": " + parameters[key] + "\r\n";
            }
            str += "\r\n";

            return str;
        }

        protected abstract void DeserializeStartLine(string line);
        protected abstract string SerializeStartLine();

    }

}