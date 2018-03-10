using System;
using System.Runtime.Serialization;

namespace CSFCloud.WebSocket.Socket.StreamCoders {
    [Serializable]
    internal class UnsupportedExtension : Exception {
        public UnsupportedExtension() {
        }

        public UnsupportedExtension(string message) : base(message) {
        }

        public UnsupportedExtension(string message, Exception innerException) : base(message, innerException) {
        }

        protected UnsupportedExtension(SerializationInfo info, StreamingContext context) : base(info, context) {
        }
    }
}