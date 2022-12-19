using System.Threading.Tasks;

namespace Altom.AltDriver.Commands
{
    public class AltPressKeys : AltBaseCommand
    {
        AltPressKeyboardKeysParams cmdParams;
        public AltPressKeys(IDriverCommunication commHandler, AltKeyCode[] keyCodes, float power, float duration, bool wait) : base(commHandler)
        {
            cmdParams = new AltPressKeyboardKeysParams(keyCodes, power, duration, wait);
        }
        public async Task Execute()
        {
            await CommHandler.Send(cmdParams);
            var data = await CommHandler.Recvall<string>(cmdParams);
            ValidateResponse("Ok", data);

            if (cmdParams.wait)
            {
                foreach (AltKeyCode key in cmdParams.keyCodes)
                {
                    data = await CommHandler.Recvall<string>(cmdParams);
                    ValidateResponse("Finished", data);
                }
            }
        }
    }
}