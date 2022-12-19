using System.Threading.Tasks;

namespace Altom.AltDriver.Commands
{
    public class AltMoveMouse : AltBaseCommand
    {
        AltMoveMouseParams cmdParams;

        public AltMoveMouse(IDriverCommunication commHandler, AltVector2 coordinates, float duration, bool wait) :
            base(commHandler)
        {
            cmdParams = new AltMoveMouseParams(coordinates, duration, wait);
        }

        public async Task Execute()
        {
            await CommHandler.Send(cmdParams);
            var data = await CommHandler.Recvall<string>(cmdParams);
            ValidateResponse("Ok", data);
            if (cmdParams.wait)
            {
                data = await CommHandler.Recvall<string>(cmdParams);
                ValidateResponse("Finished", data);
            }
        }
    }
}