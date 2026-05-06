using Gearbox.Application.Services;
using Gearbox.Domain.Entities;
using Microsoft.Extensions.Hosting;
namespace Gearbox.Application.BackgroundJobs.Email;

public class EmailBackgroundWorker : BackgroundService
{
    private readonly EmailQueue _queue;
    private readonly EmailTemplateService _templateService;
    private readonly EmailService _emailService;

    public EmailBackgroundWorker(
        EmailQueue queue,
        EmailTemplateService templateService,
        EmailService emailService)
    {
        _queue = queue;
        _templateService = templateService;
        _emailService = emailService;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                var job = await _queue.DequeueAsync(stoppingToken);

                string html = BuildEmail(job);

                await _emailService.SendAsync(
                    job.ToEmail,
                    job.Subject,
                    html
                );
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Email Worker Error: {ex.Message}");
            }
        }
    }

    private string BuildEmail(EmailJob job)
    {
        return job.Type switch
        {
            EmailType.Invoice =>
                _templateService.Invoice(
                    job.Data["customerName"].ToString()!,
                    job.Data["invoiceId"].ToString()!,
                    Convert.ToDecimal(job.Data["amount"])
                ),

            EmailType.PaymentReminder =>
                _templateService.PaymentReminder(
                    job.Data["customerName"].ToString()!,
                    Convert.ToDecimal(job.Data["dueAmount"])
                ),

            EmailType.LowStock =>
                _templateService.LowStock(
                    job.Data["partName"].ToString()!,
                    Convert.ToInt32(job.Data["stock"])
                ),

            EmailType.Appointment =>
                _templateService.AppointmentConfirmation(
                    job.Data["customerName"].ToString()!,
                    Convert.ToDateTime(job.Data["date"])
                ),

            EmailType.Loyalty =>
                _templateService.LoyaltyDiscount(
                    job.Data["customerName"].ToString()!
                ),

            EmailType.PasswordReset =>
                _templateService.PasswordReset(job.Data["link"].ToString()!),

            EmailType.EmailVerification =>
                _templateService.EmailVerification(job.Data["link"].ToString()!),

            _ => throw new Exception("Unknown email type")
        };
    }
}