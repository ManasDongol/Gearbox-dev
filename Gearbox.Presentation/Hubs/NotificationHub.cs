

using System.Security.Claims;
using Microsoft.AspNetCore.SignalR;

namespace Gearbox.Presentation.Hubs;


public class NotificationHub : Hub
{
    
    public override async Task OnConnectedAsync()
    {
        var userId = Context.User?.FindFirst("sub")?.Value;
        var role = Context.User?.FindFirst(ClaimTypes.Role)?.Value;

        if (!string.IsNullOrEmpty(userId))
            await Groups.AddToGroupAsync(Context.ConnectionId, userId);

        if (!string.IsNullOrEmpty(role))
            await Groups.AddToGroupAsync(Context.ConnectionId, role);

        await base.OnConnectedAsync();
    }

    public override async Task OnDisconnectedAsync(Exception? exception)
    {
        var userId = Context.UserIdentifier;

        if (!string.IsNullOrEmpty(userId))
        {
            await Groups.RemoveFromGroupAsync(Context.ConnectionId, userId);
        }

        await base.OnDisconnectedAsync(exception);
    }
}