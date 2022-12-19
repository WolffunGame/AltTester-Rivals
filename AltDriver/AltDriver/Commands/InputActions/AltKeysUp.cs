using System.Threading.Tasks;

namespace Altom.AltDriver.Commands
{
    public class AltKeysUp : AltBaseCommand
    {
        AltKeysUpParams cmdParams;

        public AltKeysUp(IDriverCommunication commHandler, AltKeyCode[] keyCodes) : base(commHandler)
        {
            this.cmdParams = new AltKeysUpParams(keyCodes);
        }
        public async Task Execute()
        {
            await CommHandler.Send(cmdParams);
            var data = await CommHandler.Recvall<string>(cmdParams);
            ValidateResponse("Ok", data);
        }
    }
}