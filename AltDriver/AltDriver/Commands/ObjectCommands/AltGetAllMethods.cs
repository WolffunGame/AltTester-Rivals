using System.Collections.Generic;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace Altom.AltDriver.Commands
{
    public class AltGetAllMethods : AltBaseCommand
    {
        AltGetAllMethodsParams cmdParams;
        public AltGetAllMethods(IDriverCommunication commHandler, AltComponent altComponent, AltMethodSelection methodSelection = AltMethodSelection.ALLMETHODS) : base(commHandler)
        {
            cmdParams = new AltGetAllMethodsParams(altComponent, methodSelection);
        }
        public Task<List<string>> Execute()
        {
            CommHandler.Send(cmdParams);
            return CommHandler.Recvall<List<string>>(cmdParams);
        }
    }
}