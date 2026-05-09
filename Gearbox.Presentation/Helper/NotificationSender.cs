using Gearbox.Application.DTOs;
using Gearbox.Application.Interfaces;
using Gearbox.Presentation.Hubs;
using Microsoft.AspNetCore.SignalR;

namespace Gearbox.Presentation.Helper;

public class NotificationSender : INotificationSender
{
    private readonly IHubContext<NotificationHub> _hubContext;

    public NotificationSender(IHubContext<NotificationHub> hubContext)
    {
        _hubContext = hubContext;
    }

    public async Task SendToUser(string userId, NotificationDto notification)
    {
        await _hubContext.Clients.User(userId)
            .SendAsync("ReceiveNotification", notification);
    }

    public async Task SendToRole(string role, NotificationDto notification)
    {
        await _hubContext.Clients.Group(role)
            .SendAsync("ReceiveNotification", notification);
    }

    public async Task Broadcast(NotificationDto notification)
    {
        await _hubContext.Clients.All
            .SendAsync("ReceiveNotification", notification);
    }
}