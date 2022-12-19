using System.Threading.Tasks;

namespace Altom.AltDriver.Commands
{
    public class AltClickElement : AltCommandReturningAltElement
    {
        AltClickElementParams cmdParams;

        public AltClickElement(IDriverCommunication commHandler, AltObject altObject, int count, float interval,
            bool wait) : base(commHandler)
        {
            cmdParams = new AltClickElementParams(
                altObject,
                count,
                interval,
                wait);
        }

        public async Task<AltObject> Execute()
        {
            await CommHandler.Send(cmdParams);
            var element = await ReceiveAltObject(cmdParams);

            if (cmdParams.wait)
            {
                var data = await CommHandler.Recvall<string>(cmdParams);
                ValidateResponse("Finished", data);
            }

            return element;
        }
    }
}