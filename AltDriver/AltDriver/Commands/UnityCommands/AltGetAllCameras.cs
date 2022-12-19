using System.Threading.Tasks;

namespace Altom.AltDriver.Commands
{
    public class AltGetAllCameras : AltBaseFindObjects
    {
        private readonly AltGetAllCamerasParams cmdParams;
        public AltGetAllCameras(IDriverCommunication commHandler) : base(commHandler)
        {
            cmdParams = new AltGetAllCamerasParams();
        }
        public Task<System.Collections.Generic.List<AltObject>> Execute()
        {
            CommHandler.Send(cmdParams);
            return ReceiveListOfAltObjects(cmdParams);
        }
    }
}