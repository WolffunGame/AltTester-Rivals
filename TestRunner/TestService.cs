using AltWebSocketSharp;
using AltWebSocketSharp.Server;

namespace TestRunner
{
    public class TestService : WebSocketBehavior
    {
        protected override void OnMessage(MessageEventArgs e)
        {
            //Receive message request from client and then start the test
            TestRunner.RunTestFromCommandLine();
            
            Send("Hello World!");
        }
    }
}