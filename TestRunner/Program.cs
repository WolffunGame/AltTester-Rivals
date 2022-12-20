using System;
using System.Collections.Generic;
using Altom.AltDriver.Logging;
using AltWebSocketSharp.Server;
using NLog;
using TestRunner.Services;

namespace TestRunner
{
    internal class Program
    {
        private static readonly NLog.Logger Logger = NLog.LogManager.GetCurrentClassLogger();
        private const string Host = "127.0.0.1";
        private const int Port = 1111;

        public static void Main(string[] args)
        {
            //Game instance connects to this server and sends a request to the server
            //Server sends a response to start the test and the game instance starts the test
            //Server sends a response to end the test and the game instance ends the test
            
            NLog.LogManager.Setup().LoadConfiguration(builder => {
                builder.ForLogger().FilterMinLevel(LogLevel.Info).WriteToConsole();
                builder.ForLogger().FilterMinLevel(LogLevel.Info).WriteToFile(fileName: "ActivationServer.log");
            });

            //create a websocket listener locally
            var server = new WebSocketServer(System.Net.IPAddress.Loopback, Port, false);
            server.AddWebSocketService<ActivationService>("/activation");
            server.Start();


            if (server.IsListening)
                Logger.Info($"Server is listening on port {Port}");

            //loop forever until the typed exit
            while (true)
            {
                var input = Console.ReadLine();
                if (input == "exit")
                    break;
            }

            //Run NUnit tests by code
            //TestRunner.RunTestByParent(ActivationService.TestParentSample);
            //Console.ReadLine ();
        }
    }
}