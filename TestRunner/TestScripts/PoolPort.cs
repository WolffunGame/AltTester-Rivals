using System;
using System.Collections.Generic;
using System.Linq;

namespace Wolffun.Automation.Tests
{
    /// <summary>
    /// Pool of computer ports
    /// </summary>
    public static class PoolPort
    {
        private static readonly Dictionary<string, PortData> Ports = new Dictionary<string, PortData>();
        private static int startPort = 10000;

        public static void Add(string testDeviceName, out int newPort)
        {
            if (Ports.ContainsKey(testDeviceName))
            {
                newPort = Ports[testDeviceName].Port;
                return;
            }
            
            int port = -1;
            if (Ports.Count == 0)
            {
                port = startPort;
            }
            else
            {
                port = Ports.Values.Max(x => x.Port) + 1;
            }

            Ports.Add(testDeviceName, new PortData(port));
            newPort = port;
        }
        
        public static int GetPortByName(string testDeviceName)
        {
            if (!Ports.ContainsKey(testDeviceName))
            {
                throw new Exception("Test device name not found");
            }

            return Ports[testDeviceName].Port;
        }

        public static void Remove(string testDeviceName)
        {
            Ports.Remove(testDeviceName);
        }

        public static int GetAvailablePort()
        {
            int port = -1;
            foreach (var portData in Ports)
            {
                if (!portData.Value.IsRunningTest)
                {
                    port = portData.Value.Port;
                    portData.Value.IsRunningTest = true;
                    break;
                }
            }

            if (port == -1)
            {
                if(Ports.Count == 0)
                {
                    port = startPort;
                    startPort++;
                }
                else
                {
                    port = Ports.Last().Value.Port + 1;
                }
                
                var newPort = new PortData() { Port = port, IsRunningTest = true };
                
            }
            
            return port;
        }
    }

    public class PortData
    {
        public int Port;
        public bool IsRunningTest;
        
        public PortData()
        {
            
        }
        
        public PortData(int port)
        {
            Port = port;
            IsRunningTest = false;
        }
    }
}