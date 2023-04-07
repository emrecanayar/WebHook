using Airline.WebAPI.Models;

namespace Airline.WebAPI.MessageBus
{
    public interface IMessageBus
    {
        void Publish(NotificationMessage notificationMessage);
    }
}
