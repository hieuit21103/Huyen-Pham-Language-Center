namespace MsHuyenLC.Application.Interfaces.Email;

public interface IEmailService
{
    Task SendEmailAsync(string to, string subject, string body);
    Task SendPasswordResetEmailAsync(string to, string fullName, string resetLink, int expiryMinutes);
    Task SendAccountCreationEmailAsync(string to, string fullName, string userName, string temporaryPassword, string loginUrl);
    Task SendPasswordChangedConfirmationEmailAsync(string to, string fullName);
    Task SendWelcomeStudentEmailAsync(string to, string fullName, string courseName, DateTime startDate);
}