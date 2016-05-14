[![npm](https://img.shields.io/npm/l/express.svg?maxAge=2592000?style=plastic)]()
# mChat
A proxy server based chat application using NetComms library for networking and WPF as a front-end.

##Server
The Server project is a simple central proxy server that all clients connect to.  The Server console currently has 4 commands:

`start` - Initializes the server and waits for connections

`stop` - Stops server

`list` - Lists currently connected user id's

`say [text]` - Broadcasts a message from the server to all connected clients

`exit` - Stops server and terminates the process

When a client sends a message, the message goes to the server and then is broadcast back to all connected clients from the server.  Sadly, the NetworkComm library used in this project only supports broadcasting to all clients, so that will probably have to be changed in the future.

[![server](http://i.imgur.com/39elTJZ.png)]()

##Client
The client application is written in WPF and uses the Material Design in XAML Toolkit for the interface.
Upon opening the client, a random name is generated for the client id using a Markov Chain to create a random word.
Currently, only text can be sent over the chat, but if a youtube link is sent, the video it links to will be rendered as an embedded video in the chat view --- Currently fixing the sizing problem with that.
The IP address that the client connects to is currently hard coded as `localhost`, but can be changed by modifying the line 

`c.Connect("localhost", 2020, name);`

in the MainWindow.xaml.cs file if your server is remote.

[![client](http://i.imgur.com/cUs4iR2.png)]()
