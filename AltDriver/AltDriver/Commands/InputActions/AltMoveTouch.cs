using System.Threading.Tasks;

namespace Altom.AltDriver.Commands
{
    public class AltMoveTouch : AltBaseCommand
    {
        AltMoveTouchParams cmdParams;

        public AltMoveTouch(IDriverCommunication commHandler, int fingerId, AltVector2 coordinates) : base(commHandler)
        {
            this.cmdParams = new AltMoveTouchParams(fingerId, coordinates);
        }

        public async Task Execute()
        {
            await CommHandler.Send(cmdParams);
            var data = await CommHandler.Recvall<string>(cmdParams);
            ValidateResponse("Ok", data);
        }
    }
}