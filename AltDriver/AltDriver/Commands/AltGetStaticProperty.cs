using System.Collections.Generic;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace Altom.AltDriver.Commands
{
    public class AltGetStaticProperty<T> : AltBaseCommand
    {
        AltGetObjectComponentPropertyParams cmdParams;
        public AltGetStaticProperty(IDriverCommunication commHandler, string componentName, string propertyName, string assemblyName, int maxDepth) : base(commHandler)
        {
            cmdParams = new AltGetObjectComponentPropertyParams(null, componentName, propertyName, assemblyName, maxDepth);
        }
        public async Task<T> Execute()
        {
            await CommHandler.Send(cmdParams);
            T data = await CommHandler.Recvall<T>(cmdParams);
            return data;
        }
    }
}