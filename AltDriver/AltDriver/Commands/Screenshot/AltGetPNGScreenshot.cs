using System.Threading.Tasks;

namespace Altom.AltDriver.Commands
{
    public class AltGetPNGScreenshot : AltBaseCommand
    {
        string path;
        AltGetPNGScreenshotParams cmdParams;

        public AltGetPNGScreenshot(IDriverCommunication commHandler, string path) : base(commHandler)
        {
            this.path = path;
            this.cmdParams = new AltGetPNGScreenshotParams();
        }

        public async Task Execute()
        {
            await CommHandler.Send(cmdParams);
            var message = await CommHandler.Recvall<string>(cmdParams);
            ValidateResponse("Ok", message);
            string screenshotData = await CommHandler.Recvall<string>(cmdParams);
            System.IO.File.WriteAllBytes(path, System.Convert.FromBase64String(screenshotData));
        }
    }
}