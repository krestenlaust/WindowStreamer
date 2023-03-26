# WindowStreamer

WindowStreamer is a (self-proclaimed) user-friendly, open-source application written in C# using .NET Forms that enables screen streaming between computers. It only supports one-way streaming and works only on Windows operating systems.

## Features

- Stream your screen from one computer to another

### Would like to implement
- Support for keyboard or mouse input
- Compression (right now it's only supposed to be used on a local network with high bandwidth)
- Stream audio when streaming an application

## Installation

To install WindowStreamer, follow these simple steps:

- Download the latest release of WindowStreamer from the project's GitHub repository.
- Extract the contents of the downloaded ZIP file to a folder of your choice.

## Usage

Using WindowStreamer should be straightforward as the UI has been designed to be accessible and easy to use. But if you're experiencing issues, then it's reaffirming to know you're doing it right: 

### Server
1. Open the server program on the computer that's supposed to stream it's content.
2. Press start to begin accepting connections
3. When a connection comes through a popup will appear and you can choose to accept it.
4. You can resize the window whenever to match the size of the content you would like to stream.

### Viewer
1. Open the client program on the computer that's supposed to receive the stream.
2. Specify an IP-address of the computer that the server is running on, and press connect.
3. Once the connection is open and the server has accepted, it should just work.

### Other
- Currently, blocking on the server is ephemeral, because it's only stored in memory. Restarting the server will reset the list of blocked IP-addresses
- The viewer resizes it's window to fit the stream window by default, this can be disabled with the appropriately named button.
- The port 10063 is used for a TCP connection that defines the size of the window among other things.
- The port 10064 is used for a UDP 'connection' that is used for the actual screen footage.
- Make sure these ports allowed to be used.

### Start parameters
The applications support the following parameters on startup:

- Viewer, --connect (-c), Connect to server on startup
Example usage: `Client.exe --connect 192.168.1.10:10063`, `Client.exe --connect 192.168.1.10`

- Server, --listen (-l), Start accepting connections on startup
Example usage: `Server.exe --listen 0.0.0.0:10063`, `Server.exe --listen 0.0.0.0`

## Contributing
ANY contribution is welcome! No need to worry about correctly formatting an issue or similar.

### Extensions
If you plan on contributing to anything related to connecting the applications, or something related to after a connection has been initiated, then this extension is very useful:  [Switchstartup Projects](https://heptapod.host/thirteen/switchstartupproject/). The project contains a configuration for it, that lets you debug both the server and the client at the same time, and start them with parameters, as opposed to having to connect them manually.

## License
WindowStreamer is released under the GPL-3.0 License. See the LICENSE file for details.