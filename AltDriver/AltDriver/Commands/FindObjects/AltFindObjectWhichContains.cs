using System.Threading.Tasks;

namespace Altom.AltDriver.Commands
{
    public class AltFindObjectWhichContains : AltBaseFindObjects
    {
        AltFindObjectParams cmdParams;

        public AltFindObjectWhichContains(IDriverCommunication commHandler, By by, string value, By cameraBy, string cameraValue, bool enabled) : base(commHandler)
        {
            cmdParams = new AltFindObjectParams(SetPathContains(by, value), cameraBy, SetPath(cameraBy, cameraValue), enabled);
        }
        public Task<AltObject> Execute()
        {
            CommHandler.Send(cmdParams);
            return ReceiveAltObject(cmdParams);
        }
    }
}