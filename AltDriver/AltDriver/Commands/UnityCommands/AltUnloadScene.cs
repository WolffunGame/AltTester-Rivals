using System.Threading.Tasks;

namespace Altom.AltDriver.Commands
{
    public class AltUnloadScene : AltBaseCommand
    {
        AltUnloadSceneParams cmdParams;
        public AltUnloadScene(IDriverCommunication commHandler, string sceneName) : base(commHandler)
        {
            cmdParams = new AltUnloadSceneParams(sceneName);
        }
        public async Task Execute()
        {
           await CommHandler.Send(cmdParams);

            var data =await CommHandler.Recvall<string>(cmdParams);
            ValidateResponse("Ok", data);

            data =await CommHandler.Recvall<string>(cmdParams);
            ValidateResponse("Scene Unloaded", data);
        }
    }
}