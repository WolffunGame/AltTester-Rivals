using System.Threading.Tasks;

namespace Altom.AltDriver.Commands
{
    public class AltDragObject : AltCommandReturningAltElement
    {
        AltDragObjectParams cmdParams;
        public AltDragObject(IDriverCommunication commHandler, AltVector2 position, AltObject altObject) : base(commHandler)
        {
            cmdParams = new AltDragObjectParams(altObject, position);
        }
        public Task<AltObject> Execute()
        {
            CommHandler.Send(cmdParams);
            return ReceiveAltObject(cmdParams);
        }
    }
}