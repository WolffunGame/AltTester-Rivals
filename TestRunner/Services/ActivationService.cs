using System;
using AltWebSocketSharp;
using AltWebSocketSharp.Server;
using Newtonsoft.Json;
using Wolffun.Automation.Tests;
using WolffunTester;
using WolffunTester.TestRunner;

namespace TestRunner.Services
{
    public class ActivationService : WebSocketBehavior
    {
        private static readonly NLog.Logger Logger = NLog.LogManager.GetCurrentClassLogger();
        
        public const string TestParentSample = "Wolffun.Automation.Tests.TestLoginNotHaveTutorial";
        private readonly TestRunner _testRunner = new TestRunner();
        //private string _host = "127.0.0.1";
        private ActivationMessage? _message;

        protected override void OnMessage(MessageEventArgs e)
        {
            _message = JsonConvert.DeserializeObject<ActivationMessage>(e.Data);
            if (_message == null)
            {
                return;
            }
            
            Logger.Info($"Message received: {_message}");
            
            if (_message.Status == ActivationProgress.Connecting)
            {
                Logger.Info($"Device {_message.DeviceName} is connecting to server");

                PoolConnectionInfo.Add(_message.Id, _message.DeviceHost, out var port);
                var data = new ActivationMessage()
                {
                    DeviceHost = _message.DeviceHost, DevicePort = port, DeviceName = _message.DeviceName,
                    Status = ActivationProgress.Connecting, Id = _message.Id
                };

                var json = JsonConvert.SerializeObject(data);
                Send(json);

                Logger.Info("Sent: " + json);
            }
            else if (_message.Status == ActivationProgress.Starting)
            {
                var data = new ActivationMessage()
                {
                    DeviceHost = _message.DeviceHost, DevicePort = PoolConnectionInfo.GetPortById(_message.Id),
                    DeviceName = _message.DeviceName,
                    Status = ActivationProgress.Starting, Id = _message.Id
                };

                var json = JsonConvert.SerializeObject(data);
                Send(json);
                Logger.Info("Sent: " + json);

                //wait for 2 seconds
                //System.Threading.Thread.Sleep(2000);

                Logger.Info(
                    $"Start test at device: {data.DeviceName}, port: {data.DevicePort}, host: {data.DeviceHost}, status: {data.Status}, test: {TestParentSample}");
                //TestRunner.RunTestByParent(TestParentSample);
                _testRunner.RunTestByParent(TestParentSample);
                _testRunner.OnTestRunFinished = s => { Logger.Info($"Finished test on device {_message.DeviceName} ,test progress: {s}"); };

            }else if (_message.Status == ActivationProgress.Stopping)
            {
                PoolConnectionInfo.Remove(_message.Id);
        
                Logger.Info($"Device {_message.DeviceName} is finished");
                _testRunner.StopTests();
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
        [JsonProperty(Order = 0)] public int Id;
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
        Starting = 2,
        Stopping = 3
    }
}