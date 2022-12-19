using System;
using AltWebSocketSharp;
using AltWebSocketSharp.Server;
using Newtonsoft.Json;
using Wolffun.Automation.Tests;

namespace TestRunner.Services
{
    public class ActivationService : WebSocketBehavior
    {
        private const string ActivateEvent = "activation";
        public const string TestParentSample = "Wolffun.Automation.Tests.TestLoginNotHaveTutorial";

        private string _host = "127.0.0.1";

        protected override void OnMessage(MessageEventArgs e)
        {
            var message = JsonConvert.DeserializeObject<ActivationMessage>(e.Data);
            if (message == null)
            {
                return;
            }

            if (message.Status == ActivationProgress.Connecting)
            {
                Console.WriteLine($"Device {message.DeviceName} is connecting to server");

                PoolPort.Add(message.DeviceName, out var port);
                var data = new ActivationMessage()
                {
                    DeviceHost = _host, DevicePort = port, DeviceName = message.DeviceName,
                    Status = ActivationProgress.Connecting
                };

                var json = JsonConvert.SerializeObject(data);
                Send(json);

                Console.WriteLine("Sent: " + json);
            }
            else if (message.Status == ActivationProgress.Starting)
            {
                var data = new ActivationMessage()
                {
                    DeviceHost = _host, DevicePort = PoolPort.GetPortByName(message.DeviceName),
                    DeviceName = message.DeviceName,
                    Status = ActivationProgress.Starting
                };

                var json = JsonConvert.SerializeObject(data);
                Send(json);
                Console.WriteLine("Sent: " + json);

                //wait for 2 seconds
                System.Threading.Thread.Sleep(2000);

                Console.WriteLine(
                    $"Start test at device: {data.DeviceName}, port: {data.DevicePort}, host: {data.DeviceHost}, status: {data.Status}, test: {TestParentSample}");
                TestRunner.RunTestByParent(TestParentSample);
            }
        }

        protected override void OnOpen()
        {
            base.OnOpen();
            //Sessions.Broadcast("Connected");
        }

        protected override void OnClose(CloseEventArgs e)
        {
            base.OnClose(e);
            Sessions.Broadcast($"AutomationActivateService is closed: {e.Reason}");
        }

        protected override void OnError(ErrorEventArgs e)
        {
            base.OnError(e);
            Sessions.Broadcast($"AutomationActivateService is error: {e.Message}");
        }
    }

    public class ActivationMessage
    {
        [JsonProperty(Order = 1)] public int DevicePort;
        [JsonProperty(Order = 2)] public string DeviceHost;
        [JsonProperty(Order = 3)] public string DeviceName;
        [JsonProperty(Order = 4)] public ActivationProgress Status;
    }

    [Flags]
    public enum ActivationProgress : byte
    {
        Idle = 0,
        Connecting = 1,
        Starting = 2
    }
}