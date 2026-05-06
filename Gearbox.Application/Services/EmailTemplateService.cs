
namespace Gearbox.Application.Services
{
    public class EmailTemplateService
    {
        //  Base style (optional but keeps consistency)
        private string Wrap(string title, string body)
        {
            return $"""
            <div style="font-family: Arial; background:#f7f9f9; padding:20px;">
                <div style="max-width:600px;margin:auto;background:white;border-radius:8px;padding:20px;border:1px solid #d8e6e6;">

                    <h2 style="color:#1a3a3a;margin-bottom:10px;">
                        {title}
                    </h2>

                    <div style="color:#111c1c;">
                        {body}
                    </div>

                    <hr style="margin-top:20px;border:0;border-top:1px solid #d8e6e6;" />

                    <p style="font-size:12px;color:#6b8080;">
                        Gearbox Inc.
                    </p>

                </div>
            </div>
            """;
        }

        // 🔐 Password Reset
        public string PasswordReset(string resetLink)
        {
            return Wrap("Password Reset", $"""
                <p>Click below to reset your password (valid for 15 minutes):</p>
                <a href="{resetLink}" 
                   style="display:inline-block;padding:10px 20px;background:#1a3a3a;color:white;text-decoration:none;border-radius:6px;">
                    Reset Password
                </a>
            """);
        }

        // ✅ Email Verification
        public string EmailVerification(string verifyLink)
        {
            return Wrap("Verify Account", $"""
                <p>Please verify your email (valid for 24 hours):</p>
                <a href="{verifyLink}" 
                   style="display:inline-block;padding:10px 20px;background:#1a3a3a;color:white;text-decoration:none;border-radius:6px;">
                    Verify Account
                </a>
            """);
        }

        // 🧾 Invoice
        public string Invoice(string customerName, string invoiceId, decimal amount)
        {
            return Wrap($"Invoice #{invoiceId}", $"""
                <p>Hello {customerName},</p>
                <p>Total Amount: <strong>Rs. {amount}</strong></p>
                <p>Thank you for your purchase.</p>
            """);
        }

        // ⚠ Low Stock
        public string LowStock(string partName, int remainingStock)
        {
            return Wrap("Low Stock Warning", $"""
                <p>Item: <strong>{partName}</strong></p>
                <p>Remaining Stock: <strong>{remainingStock}</strong></p>
            """);
        }

        // 💸 Payment Reminder
        public string PaymentReminder(string customerName, decimal dueAmount)
        {
            return Wrap("Payment Reminder", $"""
                <p>Hello {customerName},</p>
                <p>Due Amount: <strong>Rs. {dueAmount}</strong></p>
                <p>Please clear your balance as soon as possible.</p>
            """);
        }

        // 📅 Appointment
        public string AppointmentConfirmation(string customerName, DateTime date)
        {
            return Wrap("Appointment Confirmed", $"""
                <p>Hello {customerName},</p>
                <p>Your appointment is scheduled on:</p>
                <p><strong>{date}</strong></p>
            """);
        }

        // 🎉 Loyalty Discount
        public string LoyaltyDiscount(string customerName)
        {
            return Wrap("You earned a discount!", $"""
                <p>Congratulations {customerName}!</p>
                <p>You have earned a <strong>10% discount</strong>.</p>
            """);
        }
    }
}