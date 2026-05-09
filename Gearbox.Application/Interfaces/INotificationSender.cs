using Gearbox.Application.DTOs;

namespace Gearbox.Application.Interfaces;

public interface INotificationSender
{
    public Task SendToUser(string userId, NotificationDto notification);
    public Task SendToRole(string role, NotificationDto notification);
    public Task Broadcast(NotificationDto notification);
}