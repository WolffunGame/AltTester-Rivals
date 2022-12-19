using System.Collections.Generic;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace Altom.AltDriver.Commands
{
    public class AltCommandReturningAltElement : AltBaseCommand
    {
        public AltCommandReturningAltElement(IDriverCommunication commHandler) : base(commHandler)
        {
        }

        protected async Task<AltObject> ReceiveAltObject(CommandParams cmdParams)
        {
            var altElement = await CommHandler.Recvall<AltObject>(cmdParams);
            if (altElement != null) altElement.CommHandler = CommHandler;

            return altElement;
        }
        protected async Task<List<AltObject>> ReceiveListOfAltObjects(CommandParams cmdParams)
        {
            var altElements = await CommHandler.Recvall<List<AltObject>>(cmdParams);

            foreach (var altElement in altElements)
            {
                altElement.CommHandler = CommHandler;
            }

            return altElements;
        }
    }
}