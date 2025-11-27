namespace MsHuyenLC.Infrastructure.Templates;

/// <summary>
/// Helper class để đọc và xử lý HTML email templates
/// </summary>
public static class EmailTemplateHelper
{
    private static readonly string TemplateBasePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Templates", "Email");

    /// <summary>
    /// Đọc template HTML và thay thế các placeholder
    /// </summary>
    /// <param name="templateFileName">Tên file template (VD: "AccountCreation.html")</param>
    /// <param name="replacements">Dictionary chứa các cặp placeholder và giá trị thay thế</param>
    /// <returns>HTML content đã được thay thế placeholder</returns>
    public static string GetEmailTemplate(string templateFileName, Dictionary<string, string> replacements)
    {
        var templatePath = Path.Combine(TemplateBasePath, templateFileName);
        
        if (!File.Exists(templatePath))
        {
            throw new FileNotFoundException($"Template file not found: {templatePath}");
        }

        var htmlContent = File.ReadAllText(templatePath);

        // Thay thế tất cả placeholder
        foreach (var replacement in replacements)
        {
            htmlContent = htmlContent.Replace($"{{{{{replacement.Key}}}}}", replacement.Value);
        }

        return htmlContent;
    }

    /// <summary>
    /// Lấy template email cấp tài khoản
    /// </summary>
    public static string GetAccountCreationTemplate(string fullName, string username, string password)
    {
        var replacements = new Dictionary<string, string>
        {
            { "FullName", fullName },
            { "Username", username },
            { "Password", password },
        };

        return GetEmailTemplate("AccountCreation.html", replacements);
    }

    /// <summary>
    /// Lấy template email đặt lại mật khẩu
    /// </summary>
    public static string GetPasswordResetTemplate(string fullName, string resetLink, int expiryMinutes = 30)
    {
        var replacements = new Dictionary<string, string>
        {
            { "FullName", fullName },
            { "ResetLink", resetLink },
            { "ExpiryMinutes", expiryMinutes.ToString() }
        };

        return GetEmailTemplate("PasswordReset.html", replacements);
    }

    /// <summary>
    /// Lấy template email xác nhận đổi mật khẩu
    /// </summary>
    public static string GetPasswordChangedTemplate(string fullName, DateOnly changedDate)
    {
        var replacements = new Dictionary<string, string>
        {
            { "FullName", fullName },
            { "ChangedDate", changedDate.ToString("dd/MM/yyyy") }
        };

        return GetEmailTemplate("PasswordChanged.html", replacements);
    }

    /// <summary>
    /// Lấy template email chào mừng học viên
    /// </summary>
    public static string GetWelcomeStudentTemplate(string fullName, string courseName, DateOnly startDate)
    {
        var replacements = new Dictionary<string, string>
        {
            { "FullName", fullName },
            { "CourseName", courseName },
            { "StartDate", startDate.ToString("dd/MM/yyyy") }
        };

        return GetEmailTemplate("WelcomeStudent.html", replacements);
    }
}
