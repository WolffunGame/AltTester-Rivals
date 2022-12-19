using System;
using System.Threading.Tasks;

namespace Altom.AltDriver.Commands
{
    public class AltKeysDown : AltBaseCommand
    {
        AltKeysDownParams cmdParams;

        public AltKeysDown(IDriverCommunication commHandler, AltKeyCode[] keyCodes, float power) : base(commHandler)
        {
            this.cmdParams = new AltKeysDownParams(keyCodes, power);
        }
        public async Task Execute()
        {
            await CommHandler.Send(cmdParams);
            var data = await CommHandler.Recvall<string>(cmdParams);
            ValidateResponse("Ok", data);
        }
    }
}