using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;

namespace WolffunTester.TestRunner
{
    /// <summary>
    /// Pool of computer ports
    /// </summary>
    public static class PoolConnectionInfo
    {
        public static readonly ConcurrentDictionary<int, ConnectionInfo> ConnectionInfos =
            new ConcurrentDictionary<int, ConnectionInfo>();

        private static int _startPort = 10000;
        private const int DeviceId = 1;

        private const string fileName = "Devices.txt";

        public static void Add(int deviceId, string address, out int newPort)
        {
            if (ConnectionInfos.ContainsKey(DeviceId))
            {
                newPort = ConnectionInfos[DeviceId].Port;

                ConnectionInfos[DeviceId].Address = address;
                ConnectionInfos.AddOrUpdate(deviceId, ConnectionInfos[DeviceId],
                    (key, value) => ConnectionInfos[DeviceId]);
                ConnectionInfos.TryRemove(DeviceId, out _);
                
                SaveDevices();
                return;
            }

            if (ConnectionInfos.ContainsKey(deviceId))
            {
                newPort = ConnectionInfos[deviceId].Port;
                ConnectionInfos[deviceId].Address = address;
                
                SaveDevices();
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

            var added = ConnectionInfos.TryAdd(deviceId, new ConnectionInfo(address, port));
            if (!added)
            {
                throw new Exception("Failed to add port to pool");
            }

            newPort = port;
            SaveDevices();
        }

        public static int GetPortById(int id)
        {
            if (!ConnectionInfos.ContainsKey(id))
            {
                throw new Exception("Test device name not found");
            }

            return ConnectionInfos[id].Port;
        }

        public static void Remove(int id)
        {
            if (!ConnectionInfos.ContainsKey(id))
            {
                throw new Exception("Test device name not found");
            }

            ConnectionInfos[id].IsRunningTest = false;
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
                if (ConnectionInfos.Count == 0)
                {
                    connectionInfo.Port = _startPort;
                    _startPort++;
                }
                else
                {
                    connectionInfo.Port = ConnectionInfos.Last().Value.Port + 1;
                }

                connectionInfo.IsRunningTest = true;
                ConnectionInfos.TryAdd(DeviceId, connectionInfo);
            }
            SaveDevices();
            return connectionInfo;
        }

        private static void SaveDevices()
        {
            var lines = new List<string>();
            //align as a table 50 spaces every column
            lines.Add("DeviceId".PadRight(30) + "| Address".PadRight(30) + "| Port".PadRight(30));
            lines.Add("".PadRight(120, '-'));
            foreach (var portData in ConnectionInfos)
            {
                lines.Add(portData.Key.ToString().PadRight(30) + "| " + portData.Value.Address.PadRight(30) + "| " + portData.Value.Port.ToString().PadRight(30));
            }

            //overwrites the file
            System.IO.File.WriteAllLines(fileName, lines);
        }
    }

    public class ConnectionInfo
    {
        public int Id { get; private set; }

        public int Port;
        public string Address;
        public bool IsRunningTest;
        public string TestDeviceName;
        public string TestDeviceId;
        public string PlatformName;

        public ConnectionInfo()
        {
            //merge port, address, test device id into Id
            Id = Port.GetHashCode() ^ Address.GetHashCode() ^ TestDeviceId.GetHashCode();
        }

        public ConnectionInfo(string address, int port)
        {
            Port = port;
            Address = address;
            IsRunningTest = false;
        }
    }
}