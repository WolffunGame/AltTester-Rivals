using System.Threading.Tasks;

namespace Altom.AltDriver.Commands
{
    public class AltTilt : AltBaseCommand
    {
        AltTiltParams cmdParams;

        public AltTilt(IDriverCommunication commHandler, AltVector3 acceleration, float duration, bool wait) : base(
            commHandler)
        {
            cmdParams = new AltTiltParams(acceleration, duration, wait);
        }

        public async Task Execute()
        {
            await CommHandler.Send(cmdParams);
            string data = await CommHandler.Recvall<string>(cmdParams);
            ValidateResponse("Ok", data);
            if (cmdParams.wait)
            {
                data = await CommHandler.Recvall<string>(cmdParams);
                ValidateResponse("Finished", data);
            }
        }
    }
}