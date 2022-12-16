using AltWebSocketSharp.Server;

namespace TestRunner
{
    internal class Program
    {
        public static void Main(string[] args)
        {
            //Game instance connects to this server and sends a request to the server
            //Server sends a response to start the test and the game instance starts the test
            //Server sends a response to end the test and the game instance ends the test
            
            //create a websocket listener
            var server = new WebSocketServer(8080);
            server.AddWebSocketService<TestService>("/test");
            server.Start();
        }
    }
}