using System.Threading.Tasks;

namespace Altom.AltDriver.Commands
{
    public class AltPointerDownFromObject : AltCommandReturningAltElement
    {
        AltPointerDownFromObjectParams cmdParams;
        public AltPointerDownFromObject(IDriverCommunication commHandler, AltObject altObject) : base(commHandler)
        {
            this.cmdParams = new AltPointerDownFromObjectParams(altObject);
        }
        public Task<AltObject> Execute()
        {
            CommHandler.Send(cmdParams);
            return ReceiveAltObject(cmdParams);
        }
    }
}