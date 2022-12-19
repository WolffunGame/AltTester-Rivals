using System;
using System.Threading.Tasks;
using Altom.AltDriver.Notifications;

namespace Altom.AltDriver.Commands
{
    public class AddNotificationListener<T> : AltBaseCommand
    {
        private readonly ActivateNotification cmdParams;
        private readonly Action<T> callback;
        private readonly bool overwrite;

        public AddNotificationListener(IDriverCommunication commHandler, NotificationType notificationType,
            Action<T> callback, bool overwrite) : base(commHandler)
        {
            this.cmdParams = new ActivateNotification(notificationType);
            this.callback = callback;
            this.overwrite = overwrite;
        }

        public async Task Execute()
        {
            this.CommHandler.AddNotificationListener(cmdParams.NotificationType, callback, overwrite);
            await this.CommHandler.Send(this.cmdParams);
            var data = await this.CommHandler.Recvall<string>(this.cmdParams);
            ValidateResponse("Ok", data);
        }
    }
}