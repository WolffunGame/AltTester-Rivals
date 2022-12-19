using System;
using System.Threading.Tasks;
using Altom.AltDriver.Notifications;

namespace Altom.AltDriver.Commands
{
    public class RemoveNotificationListener : AltBaseCommand
    {
        private readonly DeactivateNotification cmdParams;

        public RemoveNotificationListener(IDriverCommunication commHandler, NotificationType notificationType) : base(commHandler)
        {
            this.cmdParams = new DeactivateNotification(notificationType);
        }
        public async Task Execute()
        {
            this.CommHandler.RemoveNotificationListener(cmdParams.NotificationType);
            await this.CommHandler.Send(this.cmdParams);
            var data = await this.CommHandler.Recvall<string>(this.cmdParams);
            ValidateResponse("Ok", data);
        }
    }
}
