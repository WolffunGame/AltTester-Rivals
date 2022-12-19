using System.Threading.Tasks;

namespace Altom.AltDriver.Commands
{
    public class AltDeletePlayerPref : AltBaseCommand
    {
        AltDeletePlayerPrefParams cmdParams;

        public AltDeletePlayerPref(IDriverCommunication commHandler) : base(commHandler)
        {
            this.cmdParams = new AltDeletePlayerPrefParams();
        }

        public async Task Execute()
        {
            await CommHandler.Send(cmdParams);
            var data = await CommHandler.Recvall<string>(cmdParams);
            ValidateResponse("Ok", data);
        }
    }
}