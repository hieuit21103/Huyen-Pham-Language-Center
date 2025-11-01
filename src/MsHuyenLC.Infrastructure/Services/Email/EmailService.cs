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

    public EmailService()
    {
        _smtpHost = Environment.GetEnvironmentVariable("Smtp__Host") ?? "smtp.gmail.com";
        _smtpPort = int.Parse(Environment.GetEnvironmentVariable("Smtp__Port") ?? "587");
        _smtpUsername = Environment.GetEnvironmentVariable("Smtp__Username") ?? "your-email@gmail.com";
        _smtpPassword = Environment.GetEnvironmentVariable("Smtp__Password") ?? "your-app-password";
        _fromEmail = Environment.GetEnvironmentVariable("Smtp__From") ?? "noreply@mshuyenlc.com";
        _enableSsl = bool.Parse(Environment.GetEnvironmentVariable("Smtp__EnableSsl") ?? "true");
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
            IsBodyHtml = true
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
        string temporaryPassword)
    {
        var subject = "Tài khoản của bạn đã được tạo - HPLC";
        var body = EmailTemplateHelper.GetAccountCreationTemplate(fullName, username, temporaryPassword);

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
        var subject = "Yêu cầu đặt lại mật khẩu - HPLC";
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
        var subject = "Mật khẩu đã được thay đổi - HPLC";
        var body = EmailTemplateHelper.GetPasswordChangedTemplate(fullName, DateOnly.FromDateTime(DateTime.UtcNow));

        await SendEmailAsync(to, subject, body);
    }

    /// <summary>
    /// Gửi email chào mừng học viên mới
    /// </summary>
    public async Task SendWelcomeStudentEmailAsync(
        string to,
        string fullName,
        string courseName,
        DateOnly startDate)
    {
        var subject = "Chào mừng bạn đến với khóa học - HPLC";
        var body = EmailTemplateHelper.GetWelcomeStudentTemplate(fullName, courseName, startDate);

        await SendEmailAsync(to, subject, body);
    }
}
