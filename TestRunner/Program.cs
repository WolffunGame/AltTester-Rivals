using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Altom.AltDriver.Logging;
using AltWebSocketSharp.Server;
using NUnit.Framework.Interfaces;
using Renci.SshNet;
using Renci.SshNet.Common;
using TestRunner.Services;
using Wolffun.Automation.Tests;

namespace TestRunner
{
    internal class Program
    {
        private const string Host = "127.0.0.1";
        private const int Port = 1111;
        public static void Main(string[] args)
        {
            //Game instance connects to this server and sends a request to the server
            //Server sends a response to start the test and the game instance starts the test
            //Server sends a response to end the test and the game instance ends the test

            //create a websocket listener locally
            var server = new WebSocketServer(System.Net.IPAddress.Loopback, Port, false);
            server.AddWebSocketService<ActivationService>("/activation");
            server.Start();

            if(server.IsListening)
                Console.WriteLine($"Server is listening on port {Port}");
            
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


        public static string PortForward()
        {
            var connectionInfo = new ConnectionInfo("sftp.foo.com",
                "guest",
                new PasswordAuthenticationMethod("guest", "pwd"),
                new PrivateKeyAuthenticationMethod("rsa.key"));
            using (var client = new SshClient(connectionInfo))
            {
                client.Connect();
                var portForward = new ForwardedPortLocal("localhost", 13001, "localhost", 13000);
                client.AddForwardedPort(portForward);
                portForward.Start();
                return portForward.BoundHost;
            }
            
        }
    }
}