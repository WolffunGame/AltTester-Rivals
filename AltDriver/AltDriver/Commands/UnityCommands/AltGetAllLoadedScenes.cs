using System.Collections.Generic;
using System.Threading.Tasks;

namespace Altom.AltDriver.Commands
{
    internal class AltGetAllLoadedScenes : AltBaseCommand
    {
        private readonly AltGetAllLoadedScenesParams cmdParams;
        public AltGetAllLoadedScenes(IDriverCommunication commHandler) : base(commHandler)
        {
            cmdParams = new AltGetAllLoadedScenesParams();
        }
        public Task<List<string>> Execute()
        {
            CommHandler.Send(cmdParams);
            return CommHandler.Recvall<List<string>>(cmdParams);
        }
    }
}