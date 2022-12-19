using System.Threading.Tasks;
using Newtonsoft.Json;

namespace Altom.AltDriver.Commands
{
    public class AltLoadScene : AltBaseCommand
    {
        AltLoadSceneParams cmdParams;

        public AltLoadScene(IDriverCommunication commHandler, string sceneName, bool loadSingle) : base(commHandler)
        {
            cmdParams = new AltLoadSceneParams(sceneName, loadSingle);
        }

        public async Task Execute()
        {
            await CommHandler.Send(cmdParams);

            var data = await CommHandler.Recvall<string>(cmdParams);
            ValidateResponse("Ok", data);

            data = await CommHandler.Recvall<string>(cmdParams);
            ValidateResponse("Scene Loaded", data);
        }
    }
}