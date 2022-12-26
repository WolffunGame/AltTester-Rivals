using AltWebSocketSharp;
using AltWebSocketSharp.Server;

namespace TestRunner.Services
{
    public class EchoService : WebSocketBehavior
    {
        private static readonly NLog.Logger Logger = NLog.LogManager.GetCurrentClassLogger();
        
        protected override void OnMessage(MessageEventArgs e)
        {
            Logger.Info($"Received message: {e.Data}");
            Send(e.Data);
        }
    }
}