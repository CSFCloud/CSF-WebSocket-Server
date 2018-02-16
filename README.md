# CSF-WebSocket-Server

## Installation

The Server can be installed with the dotnet package manager. Run this command:

```BASH
dotnet add package CSFCloud.WebSocket.Server
```

## Usage

### To start a server:

Usages:
```C#
using CSFCloud.WebSocket;
```

Create and start the server:
```C#
Server server = new Server(80); // The default port is 80
```

To recieve events, use the ```server.OnUserConnect ```, ```server.OnPacketReceived```, ```server.OnUserDisconnect``` event listeners.
