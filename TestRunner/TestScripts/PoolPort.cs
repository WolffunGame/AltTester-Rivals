using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;

namespace Wolffun.Automation.Tests
{
    /// <summary>
    /// Pool of computer ports
    /// </summary>
    public static class PoolPort
    {
        private static readonly ConcurrentDictionary<string, PortData> Ports = new ConcurrentDictionary<string, PortData>();
        private static int _startPort = 10000;
        private const string TestDevice = "TestDevice";

        public static void Add(string testDeviceName, out int newPort)
        {
            if(Ports.ContainsKey(TestDevice))
            {
                newPort = Ports[TestDevice].Port;
                
                Ports.AddOrUpdate(testDeviceName, Ports[TestDevice], (key, value) => Ports[TestDevice]);
                Ports.TryRemove(TestDevice, out _);
                return;
            }
            
            if (Ports.ContainsKey(testDeviceName))
            {
                newPort = Ports[testDeviceName].Port;
                return;
            }
            
            int port = -1;
            if (Ports.Count == 0)
            {
                port = _startPort;
            }
            else
            {
                port = Ports.Values.Max(x => x.Port) + 1;
            }

            var added = Ports.TryAdd(testDeviceName, new PortData(port));
            if (!added)
            {
                throw new Exception("Failed to add port to pool");
            }
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
            if (!Ports.ContainsKey(testDeviceName))
            {
                throw new Exception("Test device name not found");
            }

            Ports[testDeviceName].IsRunningTest = false;
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
                    port = _startPort;
                    _startPort++;
                }
                else
                {
                    port = Ports.Last().Value.Port + 1;
                }
                
                var newPort = new PortData() { Port = port, IsRunningTest = true };
                Ports.TryAdd(TestDevice, newPort);
                
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