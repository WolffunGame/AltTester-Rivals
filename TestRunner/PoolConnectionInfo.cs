using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;

namespace Wolffun.Automation.Tests
{
    /// <summary>
    /// Pool of computer ports
    /// </summary>
    public static class PoolConnectionInfo
    {
        private static readonly ConcurrentDictionary<string, ConnectionInfo> ConnectionInfos = new ConcurrentDictionary<string, ConnectionInfo>();
        private static int _startPort = 10000;
        private const string TestDevice = "TestDevice";

        public static void Add(string testDeviceName, string address, out int newPort)
        {
            if(ConnectionInfos.ContainsKey(TestDevice))
            {
                newPort = ConnectionInfos[TestDevice].Port;
                
                ConnectionInfos[TestDevice].Address = address;
                ConnectionInfos.AddOrUpdate(testDeviceName, ConnectionInfos[TestDevice], (key, value) => ConnectionInfos[TestDevice]);
                ConnectionInfos.TryRemove(TestDevice, out _);
                return;
            }
            
            if (ConnectionInfos.ContainsKey(testDeviceName))
            {
                newPort = ConnectionInfos[testDeviceName].Port;
                ConnectionInfos[testDeviceName].Address = address;
                return;
            }
            
            int port = -1;
            if (ConnectionInfos.Count == 0)
            {
                port = _startPort;
            }
            else
            {
                port = ConnectionInfos.Values.Max(x => x.Port) + 1;
            }

            var added = ConnectionInfos.TryAdd(testDeviceName, new ConnectionInfo(address, port));
            if (!added)
            {
                throw new Exception("Failed to add port to pool");
            }
            newPort = port;
        }
        
        public static int GetPortByName(string testDeviceName)
        {
            if (!ConnectionInfos.ContainsKey(testDeviceName))
            {
                throw new Exception("Test device name not found");
            }

            return ConnectionInfos[testDeviceName].Port;
        }

        public static void Remove(string testDeviceName)
        {
            if (!ConnectionInfos.ContainsKey(testDeviceName))
            {
                throw new Exception("Test device name not found");
            }

            ConnectionInfos[testDeviceName].IsRunningTest = false;
        }

        public static ConnectionInfo GetAvailableConnectionInfo()
        {
            
            foreach (var portData in ConnectionInfos)
            {
                if (!portData.Value.IsRunningTest)
                {
                    portData.Value.IsRunningTest = true;
                    return portData.Value;
                }
            }
            
            ConnectionInfo connectionInfo = new ConnectionInfo();
            if (connectionInfo.Port == -1)
            {
                if(ConnectionInfos.Count == 0)
                {
                    connectionInfo.Port = _startPort;
                    _startPort++;
                }
                else
                {
                    connectionInfo.Port = ConnectionInfos.Last().Value.Port + 1;
                }
                
                connectionInfo.IsRunningTest = true;
                ConnectionInfos.TryAdd(TestDevice, connectionInfo);
                
            }
            
            return connectionInfo;
        }
    }

    public class ConnectionInfo
    {
        public int Port;
        public string Address;
        public bool IsRunningTest;
        
        public ConnectionInfo()
        {
            
        }
        
        public ConnectionInfo(string address, int port)
        {
            Port = port;
            Address = address;
            IsRunningTest = false;
        }
    }
}