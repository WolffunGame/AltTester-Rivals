using System.Threading.Tasks;

namespace Altom.AltDriver.Commands
{
    public class AltGetServerVersion : AltBaseCommand
    {
        public AltGetServerVersion(IDriverCommunication commHandler) : base(commHandler)
        {
        }
        public Task<string> Execute()
        {
            var cmdParams = new AltGetServerVersionParams();
            CommHandler.Send(cmdParams);
            return CommHandler.Recvall<string>(cmdParams);
        }
    }
}