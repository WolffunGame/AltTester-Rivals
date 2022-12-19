using System.Threading.Tasks;

namespace Altom.AltDriver.Commands
{
    public class AltFindObjectAtCoordinates : AltCommandReturningAltElement
    {
        AltFindObjectAtCoordinatesParams cmdParams;

        public AltFindObjectAtCoordinates(IDriverCommunication commHandler, AltVector2 coordinates) : base(commHandler)
        {
            cmdParams = new AltFindObjectAtCoordinatesParams(coordinates);
        }

        public Task<AltObject> Execute()
        {
            CommHandler.Send(cmdParams);
            return ReceiveAltObject(cmdParams);
        }
    }
}