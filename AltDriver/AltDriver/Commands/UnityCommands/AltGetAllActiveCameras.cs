using System.Threading.Tasks;

namespace Altom.AltDriver.Commands
{
    public class AltGetAllActiveCameras : AltBaseFindObjects
    {
        private readonly AltGetAllActiveCamerasParams cmdParams;
        public AltGetAllActiveCameras(IDriverCommunication commHandler) : base(commHandler)
        {

            cmdParams = new AltGetAllActiveCamerasParams();
        }
        public Task<System.Collections.Generic.List<AltObject>> Execute()
        {
            CommHandler.Send(cmdParams);
            return ReceiveListOfAltObjects(cmdParams);
        }
    }
}