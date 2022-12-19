using System.Threading.Tasks;

namespace Altom.AltDriver.Commands
{
    public class AltSetKeyPLayerPref : AltBaseCommand
    {
        AltSetKeyPlayerPrefParams cmdParams;

        public AltSetKeyPLayerPref(IDriverCommunication commHandler, string keyName, int intValue) : base(commHandler)
        {
            cmdParams = new AltSetKeyPlayerPrefParams(keyName, intValue);
        }

        public AltSetKeyPLayerPref(IDriverCommunication commHandler, string keyName, float floatValue) : base(
            commHandler)
        {
            cmdParams = new AltSetKeyPlayerPrefParams(keyName, floatValue);
        }

        public AltSetKeyPLayerPref(IDriverCommunication commHandler, string keyName, string stringValue) : base(
            commHandler)
        {
            cmdParams = new AltSetKeyPlayerPrefParams(keyName, stringValue);
        }

        public async Task Execute()
        {
            await CommHandler.Send(cmdParams);
            var data = await CommHandler.Recvall<string>(cmdParams);
            ValidateResponse("Ok", data);
        }
    }
}