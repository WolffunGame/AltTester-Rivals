using System.Threading.Tasks;

namespace Altom.AltDriver.Commands
{
    public class AltDeleteKeyPlayerPref : AltBaseCommand
    {
        AltDeleteKeyPlayerPrefParams cmdParams;

        public AltDeleteKeyPlayerPref(IDriverCommunication commHandler, string keyName) : base(commHandler)
        {
            this.cmdParams = new AltDeleteKeyPlayerPrefParams(keyName);
        }

        public async Task Execute()
        {
            await CommHandler.Send(cmdParams);
            var data = await CommHandler.Recvall<string>(cmdParams);
            ValidateResponse("Ok", data);
        }
    }
}