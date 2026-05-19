using Gearbox.Domain.Entities;
using Gearbox.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace Gearbox.Application.BackgroundJobs.Email;

public class PaymentReminderBackgroundWorker : BackgroundService
{
    private static readonly TimeSpan CheckInterval = TimeSpan.FromDays(1);
    private readonly EmailQueue _emailQueue;
    private readonly IServiceScopeFactory _scopeFactory;

    public PaymentReminderBackgroundWorker(EmailQueue emailQueue, IServiceScopeFactory scopeFactory)
    {
        _emailQueue = emailQueue;
        _scopeFactory = scopeFactory;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        await QueueOverduePaymentRemindersAsync(stoppingToken);

        using var timer = new PeriodicTimer(CheckInterval);
        while (await timer.WaitForNextTickAsync(stoppingToken))
        {
            await QueueOverduePaymentRemindersAsync(stoppingToken);
        }
    }

    private async Task QueueOverduePaymentRemindersAsync(CancellationToken cancellationToken)
    {
        using var scope = _scopeFactory.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
        var cutoff = DateTime.UtcNow.AddMonths(-1);

        var customers = await context.Customers
            .AsNoTracking()
            .Include(customer => customer.User)
            .Include(customer => customer.SalesServicesInvoices)
            .Where(customer => customer.SalesServicesInvoices.Any(invoice =>
                !invoice.PaymentStatus && invoice.CreatedAt <= cutoff))
            .ToListAsync(cancellationToken);

        foreach (var customer in customers)
        {
            var dueAmount = customer.SalesServicesInvoices
                .Where(invoice => !invoice.PaymentStatus && invoice.CreatedAt <= cutoff)
                .Sum(invoice => invoice.TotalAmount);

            if (string.IsNullOrWhiteSpace(customer.User.Email) || dueAmount <= 0) continue;

            var customerName = $"{customer.User.FirstName} {customer.User.LastName}".Trim();
            if (string.IsNullOrWhiteSpace(customerName))
            {
                customerName = customer.User.UserName ?? "Customer";
            }

            _emailQueue.Enqueue(new EmailJob
            {
                ToEmail = customer.User.Email,
                Subject = "Gearbox payment reminder",
                Type = EmailType.PaymentReminder,
                Data = new Dictionary<string, object>
                {
                    { "customerName", customerName },
                    { "dueAmount", dueAmount }
                }
            });
        }
    }
}
