using System;
using System.Net;
using System.Net.Sockets;
using NUnit.Framework;
using Renci.SshNet;
using Renci.SshNet.Common;

namespace Wolffun.Automation.Tests
{
    public class TestsHelper
    {
        //port pool
        private static int _port = 1000;

        public static int GetPort()
        {
            return _port++;
        }

        public static int GetAltDriverPort()
        {
            string port = System.Environment.GetEnvironmentVariable("ALTDRIVER_PORT");
            if (!string.IsNullOrEmpty(port))
            {
                return int.Parse(port);
            }

            return 13000;
        }

        public static string GetAltDriverHost()
        {
            string host = System.Environment.GetEnvironmentVariable("ALTDRIVER_HOST");
            if (!string.IsNullOrEmpty(host))
            {
                return host;
            }

            return GetLocalIPAddress().ToString();
        }

        public static IPAddress GetLocalIPAddress()
        {
            var host = Dns.GetHostEntry(Dns.GetHostName());
            foreach (var ip in host.AddressList)
            {
                if (ip.AddressFamily == AddressFamily.InterNetwork)
                {
                    return ip;
                }
            }

            throw new Exception("No network adapters with an IPv4 address in the system!");
        }
        
        //run methods as command line
        public static void RunCommand(string command, string arguments)
        {
            var process = new System.Diagnostics.Process();
            process.StartInfo.FileName = command;
            process.StartInfo.Arguments = arguments;
            process.StartInfo.UseShellExecute = false;
            process.StartInfo.RedirectStandardOutput = true;
            process.StartInfo.RedirectStandardError = true;
            process.Start();
            process.WaitForExit();
        }
        
        //ssh.net tunnel
        public static void CreateTunnel(string host, string username, string password, string boundHost, uint boundPort, string tunnelHost, uint tunnelPort)
        {
            using (var client = new SshClient(host, username, password))
            {
                var port1 = new ForwardedPortLocal(boundHost, boundPort, tunnelHost, tunnelPort);
                client.AddForwardedPort(port1);
                port1.Exception += delegate (object sender, ExceptionEventArgs e)
                {
                    Assert.Fail(e.Exception.ToString());
                };
                port1.Start();

                // System.Threading.Tasks.Parallel.For(0, 100,
                //     (counter) =>
                //     {
                //         var start = DateTime.Now;
                //         var req = HttpWebRequest.Create("http://localhost:8084");
                //         using (var response = req.GetResponse())
                //         {
                //             var data = ReadStream(response.GetResponseStream());
                //             var end = DateTime.Now;
                //
                //             Console.WriteLine(string.Format("Request# {2}: Lenght: {0} Time: {1}", data.Length, (end - start), counter));
                //         }
                //     }
                // );
            }
        }
    }
}