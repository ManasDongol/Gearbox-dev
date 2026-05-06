namespace Gearbox.Domain.Entities;

public class EmailJob
{
    public string ToEmail { get; set; } = default!;
    public string Subject { get; set; } = default!;

    // identifies which template to use
    public EmailType Type { get; set; }

    // flexible data payload
    public Dictionary<string, object> Data { get; set; } = new();
}

public enum EmailType
{
    Invoice,
    PaymentReminder,
    LowStock,
    Appointment,
    Loyalty,
    PasswordReset,
    EmailVerification
}