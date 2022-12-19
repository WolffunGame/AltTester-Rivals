using System;
using System.Net;
using System.Net.Sockets;

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

            return "127.0.0.1";
        }
        
        public static string GetLocalIPAddress()
        {
            var host = Dns.GetHostEntry(Dns.GetHostName());
            foreach (var ip in host.AddressList)
            {
                if (ip.AddressFamily == AddressFamily.InterNetwork)
                {
                    return ip.ToString();
                }
            }
            throw new Exception("No network adapters with an IPv4 address in the system!");
        }
    }
}