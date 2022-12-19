using System.Threading.Tasks;
using Newtonsoft.Json;

namespace Altom.AltDriver.Commands
{
    public class AltGetCurrentScene : AltBaseFindObjects
    {
        private readonly AltGetCurrentSceneParams cmdParams;

        public AltGetCurrentScene(IDriverCommunication commHandler) : base(commHandler)
        {
            cmdParams = new AltGetCurrentSceneParams();
        }

        public async Task<string> Execute()
        {
            await CommHandler.Send(cmdParams);
            var altObject = await ReceiveAltObject(cmdParams);
            return altObject.name;
        }
    }
}