using System.Threading.Tasks;

namespace Altom.AltDriver.Commands
{
    public class AltGetScreenshotResponse
    {
        public AltVector2 scaleDifference;
        public AltVector3 textureSize;
        public byte[] compressedImage;

    }
    public class AltGetScreenshot : AltCommandReturningAltElement
    {
        AltGetScreenshotParams cmdParams;


        public AltGetScreenshot(IDriverCommunication commHandler, AltVector2 size, int screenShotQuality) : base(commHandler)
        {
            cmdParams = new AltGetScreenshotParams(size, screenShotQuality);
        }
        public virtual Task<AltTextureInformation> Execute()
        {
            CommHandler.Send(cmdParams);
            return ReceiveScreenshot(cmdParams);
        }

        protected async Task<AltTextureInformation> ReceiveScreenshot(CommandParams commandParams)
        {
            var data = await CommHandler.Recvall<string>(commandParams);
            ValidateResponse("Ok", data);

            var imageData = await CommHandler.Recvall<AltGetScreenshotResponse>(commandParams);
            byte[] decompressedImage = DecompressScreenshot(imageData.compressedImage);
            return new AltTextureInformation(decompressedImage, imageData.scaleDifference, imageData.textureSize);
        }
    }


    public class AltGetHighlightObjectScreenshot : AltGetScreenshot
    {

        AltHighlightObjectScreenshotParams cmdParams;

        public AltGetHighlightObjectScreenshot(IDriverCommunication commHandler, int id, AltColor color, float width, AltVector2 size, int screenShotQuality) : base(commHandler, size, screenShotQuality)
        {
            cmdParams = new AltHighlightObjectScreenshotParams(id, color, width, size, screenShotQuality);
        }

        public override Task<AltTextureInformation> Execute()
        {
            CommHandler.Send(cmdParams);
            return ReceiveScreenshot(cmdParams);
        }
    }


    // public class AltGetHighlightObjectFromCoordinatesScreenshot : AltGetScreenshot
    // {
    //     AltHighlightObjectFromCoordinatesScreenshotParams cmdParams;
    //
    //     public AltGetHighlightObjectFromCoordinatesScreenshot(IDriverCommunication commHandler, AltVector2 coordinates, AltColor color, float width, AltVector2 size, int screenShotQuality) : base(commHandler, size, screenShotQuality)
    //     {
    //         cmdParams = new AltHighlightObjectFromCoordinatesScreenshotParams(coordinates, color, width, size, screenShotQuality);
    //     }
    //     public Task<AltTextureInformation> Execute()
    //     { 
    //         CommHandler.Send(cmdParams);
    //
    //         // selectedObject = ReceiveAltObject(cmdParams);
    //         // if (selectedObject != null && selectedObject.name.Equals("Null") && selectedObject.id == 0)
    //         // {
    //         //     selectedObject = null;
    //         // }
    //         return ReceiveScreenshot(cmdParams);
    //
    //     }
    // }

}