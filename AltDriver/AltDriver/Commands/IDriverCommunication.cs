using System;
using System.Threading.Tasks;
using Altom.AltDriver.Notifications;

namespace Altom.AltDriver.Commands
{
    public interface IDriverCommunication
    {
        Task Send(CommandParams param);
        Task<T> Recvall<T>(CommandParams param);
        void AddNotificationListener<T>(NotificationType notificationType, Action<T> callback, bool overwrite);
        void RemoveNotificationListener(NotificationType notificationType);
        void Connect();
        void Close();
        void SetCommandTimeout(int timeout);
        void SetDelayAfterCommand(float delay);
        float GetDelayAfterCommand();
        Task SleepFor(float time);
    }
}
