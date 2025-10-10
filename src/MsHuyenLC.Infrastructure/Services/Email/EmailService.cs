using System.Net.Mail;
using MsHuyenLC.Application.Interfaces.Email;
using MsHuyenLC.Infrastructure.Templates;

namespace MsHuyenLC.Infrastructure.Services.Email;

public class EmailService : IEmailService
{
    private readonly string _smtpHost;
    private readonly int _smtpPort;
    private readonly string _smtpUsername;
    private readonly string _smtpPassword;
    private readonly string _fromEmail;
    private readonly bool _enableSsl;

    public EmailService(
        string smtpHost = "smtp.gmail.com",
        int smtpPort = 587,
        string smtpUsername = "your-email@gmail.com",
        string smtpPassword = "your-app-password",
        string fromEmail = "noreply@mshuyenlc.com",
        bool enableSsl = true)
    {
        _smtpHost = smtpHost;
        _smtpPort = smtpPort;
        _smtpUsername = smtpUsername;
        _smtpPassword = smtpPassword;
        _fromEmail = fromEmail;
        _enableSsl = enableSsl;
    }

    public async Task SendEmailAsync(string to, string subject, string body)
    {
        using var client = new SmtpClient(_smtpHost)
        {
            Port = _smtpPort,
            Credentials = new System.Net.NetworkCredential(_smtpUsername, _smtpPassword),
            EnableSsl = _enableSsl,
        };

        var mailMessage = new MailMessage(_fromEmail, to, subject, body)
        {
            IsBodyHtml = true // Quan trọng: Cho phép HTML content
        };

        await client.SendMailAsync(mailMessage);
    }

    /// <summary>
    /// Gửi email cấp tài khoản cho người dùng mới
    /// </summary>
    public async Task SendAccountCreationEmailAsync(
        string to,
        string fullName,
        string username,
        string temporaryPassword,
        string loginUrl)
    {
        var subject = "🎉 Tài khoản của bạn đã được tạo - Ms. Huyền LC";
        var body = EmailTemplateHelper.GetAccountCreationTemplate(fullName, username, temporaryPassword, loginUrl);

        await SendEmailAsync(to, subject, body);
    }

    /// <summary>
    /// Gửi email đặt lại mật khẩu
    /// </summary>
    public async Task SendPasswordResetEmailAsync(
        string to,
        string fullName,
        string resetLink,
        int expiryMinutes = 30)
    {
        var subject = "🔒 Yêu cầu đặt lại mật khẩu - Ms. Huyền LC";
        var body = EmailTemplateHelper.GetPasswordResetTemplate(fullName, resetLink, expiryMinutes);

        await SendEmailAsync(to, subject, body);
    }

    /// <summary>
    /// Gửi email xác nhận đổi mật khẩu thành công
    /// </summary>
    public async Task SendPasswordChangedConfirmationEmailAsync(
        string to,
        string fullName)
    {
        var subject = "✅ Mật khẩu đã được thay đổi - Ms. Huyền LC";
        var body = EmailTemplateHelper.GetPasswordChangedTemplate(fullName, DateTime.Now);

        await SendEmailAsync(to, subject, body);
    }

    /// <summary>
    /// Gửi email chào mừng học viên mới
    /// </summary>
    public async Task SendWelcomeStudentEmailAsync(
        string to,
        string fullName,
        string courseName,
        DateTime startDate)
    {
        var subject = "🎓 Chào mừng bạn đến với khóa học - Ms. Huyền LC";
        var body = EmailTemplateHelper.GetWelcomeStudentTemplate(fullName, courseName, startDate);

        await SendEmailAsync(to, subject, body);
    }
}
