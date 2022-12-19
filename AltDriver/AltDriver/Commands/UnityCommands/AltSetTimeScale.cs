using System.Threading.Tasks;
using Newtonsoft.Json;

namespace Altom.AltDriver.Commands
{
    public class AltSetTimeScale : AltBaseCommand
    {
        AltSetTimeScaleParams cmdParams;

        public AltSetTimeScale(IDriverCommunication commHandler, float timeScale) : base(commHandler)
        {
            cmdParams = new AltSetTimeScaleParams(timeScale);
        }

        public async Task Execute()
        {
            await CommHandler.Send(cmdParams);
            var data = await CommHandler.Recvall<string>(cmdParams);
            ValidateResponse("Ok", data);
        }
    }
}