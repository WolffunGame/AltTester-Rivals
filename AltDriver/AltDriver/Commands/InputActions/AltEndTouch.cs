using System.Threading.Tasks;

namespace Altom.AltDriver.Commands
{
    public class AltEndTouch : AltBaseCommand
    {
        AltEndTouchParams cmdParams;

        public AltEndTouch(IDriverCommunication commHandler, int fingerId) : base(commHandler)
        {
            this.cmdParams = new AltEndTouchParams(fingerId);
        }

        public async Task Execute()
        {
            await CommHandler.Send(cmdParams);
            var data = await CommHandler.Recvall<string>(cmdParams);
            ValidateResponse("Ok", data);
        }
    }
}