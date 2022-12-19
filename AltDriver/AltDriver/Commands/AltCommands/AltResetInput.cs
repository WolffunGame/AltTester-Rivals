using System.Threading.Tasks;

namespace Altom.AltDriver.Commands
{
    public class AltResetInput : AltBaseCommand
    {
        public AltResetInput(IDriverCommunication communicationHandler) : base(communicationHandler)
        {
        }

        public async Task Execute()
        {
            var cmdParams = new AltResetInputParams();
            await this.CommHandler.Send(cmdParams);
            var data = await this.CommHandler.Recvall<string>(cmdParams);
            ValidateResponse("Ok", data);
        }
    }
}