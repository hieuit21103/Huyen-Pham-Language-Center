# 📧 Email Templates - Hướng dẫn sử dụng

## 📁 Cấu trúc thư mục

```
Templates/
└── Email/
    ├── AccountCreation.html      - Template cấp tài khoản
    ├── PasswordReset.html        - Template đặt lại mật khẩu
    ├── PasswordChanged.html      - Template xác nhận đổi mật khẩu
    └── WelcomeStudent.html       - Template chào mừng học viên
```

## Thay thế Placeholder


```csharp
using MsHuyenLC.Infrastructure.Templates;

// 1. Email cấp tài khoản
var accountEmail = EmailTemplateHelper.GetAccountCreationTemplate(
    fullName: "Nguyễn Văn A",
    username: "nguyenvana",
    password: "TempPass123!",
    loginUrl: "https://mshuyenlc.com/login"
);

// 2. Email đặt lại mật khẩu
var resetEmail = EmailTemplateHelper.GetPasswordResetTemplate(
    fullName: "Nguyễn Văn A",
    resetLink: "https://mshuyenlc.com/reset-password?token=abc123",
    expiryMinutes: 30
);

// 3. Email xác nhận đổi mật khẩu
var changedEmail = EmailTemplateHelper.GetPasswordChangedTemplate(
    fullName: "Nguyễn Văn A",
    changedDate: DateTime.Now
);

// 4. Email chào mừng học viên
var welcomeEmail = EmailTemplateHelper.GetWelcomeStudentTemplate(
    fullName: "Nguyễn Văn A",
    courseName: "Tiếng Anh Giao Tiếp Cơ Bản",
    startDate: new DateTime(2025, 10, 15)
);
```

## 📋 Danh sách Placeholder cho từng template

### 1. AccountCreation.html
- `{{FullName}}` - Tên đầy đủ của người dùng
- `{{Username}}` - Tên đăng nhập
- `{{Password}}` - Mật khẩu tạm thời
- `{{LoginUrl}}` - Link đăng nhập

### 2. PasswordReset.html
- `{{FullName}}` - Tên đầy đủ của người dùng
- `{{ResetLink}}` - Link đặt lại mật khẩu
- `{{ExpiryMinutes}}` - Số phút hết hạn (mặc định: 30)

### 3. PasswordChanged.html
- `{{FullName}}` - Tên đầy đủ của người dùng
- `{{ChangedDate}}` - Thời gian đổi mật khẩu (format: dd/MM/yyyy HH:mm:ss)

### 4. WelcomeStudent.html
- `{{FullName}}` - Tên đầy đủ của học viên
- `{{CourseName}}` - Tên khóa học
- `{{StartDate}}` - Ngày bắt đầu khóa học (format: dd/MM/yyyy)

## 🚀 Sử dụng với EmailService

```csharp
// Inject EmailService
private readonly IEmailService _emailService;

public async Task CreateUserAccount()
{
    // Gửi email cấp tài khoản
    await _emailService.SendAccountCreationEmailAsync(
        to: "user@example.com",
        fullName: "Nguyễn Văn A",
        username: "nguyenvana",
        temporaryPassword: "TempPass123!",
        loginUrl: "https://mshuyenlc.com/login"
    );
}

public async Task RequestPasswordReset()
{
    // Gửi email đặt lại mật khẩu
    await _emailService.SendPasswordResetEmailAsync(
        to: "user@example.com",
        fullName: "Nguyễn Văn A",
        resetLink: "https://mshuyenlc.com/reset-password?token=abc123",
        expiryMinutes: 30
    );
}

public async Task ConfirmPasswordChanged()
{
    // Gửi email xác nhận đổi mật khẩu
    await _emailService.SendPasswordChangedConfirmationEmailAsync(
        to: "user@example.com",
        fullName: "Nguyễn Văn A"
    );
}

public async Task WelcomeNewStudent()
{
    // Gửi email chào mừng học viên
    await _emailService.SendWelcomeStudentEmailAsync(
        to: "student@example.com",
        fullName: "Nguyễn Văn A",
        courseName: "Tiếng Anh Giao Tiếp Cơ Bản",
        startDate: new DateTime(2025, 10, 15)
    );
}
```

## ⚙️ Cấu hình SMTP

Để gửi email, cần cấu hình SMTP server trong `appsettings.json`:

```json
{
  "EmailSettings": {
    "SmtpHost": "smtp.gmail.com",
    "SmtpPort": 587,
    "SmtpUsername": "your-email@gmail.com",
    "SmtpPassword": "your-app-password",
    "FromEmail": "noreply@mshuyenlc.com",
    "EnableSsl": true
  }
}
```

### Lưu ý khi sử dụng Gmail:
1. Bật "2-Step Verification" trong tài khoản Google
2. Tạo "App Password" tại: https://myaccount.google.com/apppasswords
3. Sử dụng App Password thay vì mật khẩu thông thường

## 🎨 Tùy chỉnh Template

Bạn có thể chỉnh sửa trực tiếp các file HTML trong thư mục `Templates/Email/`:

1. Mở file template cần chỉnh sửa
2. Thay đổi nội dung, màu sắc, font chữ trong thẻ `<style>`
3. Thêm hoặc xóa các placeholder theo nhu cầu
4. Cập nhật `EmailTemplateHelper.cs` nếu thêm placeholder mới

## 📝 Ví dụ thực tế

```csharp
public class UserController : ControllerBase
{
    private readonly IEmailService _emailService;
    
    public UserController(IEmailService emailService)
    {
        _emailService = emailService;
    }
    
    [HttpPost("register")]
    public async Task<IActionResult> Register(RegisterDto dto)
    {
        // Tạo user mới
        var user = await _userService.CreateUser(dto);
        
        // Gửi email cấp tài khoản
        await _emailService.SendAccountCreationEmailAsync(
            to: user.Email,
            fullName: user.FullName,
            username: user.Username,
            temporaryPassword: generatedPassword,
            loginUrl: $"{Request.Scheme}://{Request.Host}/login"
        );
        
        return Ok(new { message = "Tài khoản đã được tạo. Vui lòng kiểm tra email." });
    }
    
    [HttpPost("forgot-password")]
    public async Task<IActionResult> ForgotPassword(ForgotPasswordDto dto)
    {
        var user = await _userService.FindByEmail(dto.Email);
        if (user == null)
            return NotFound("Email không tồn tại");
        
        // Tạo reset token
        var resetToken = await _userService.GeneratePasswordResetToken(user.Id);
        var resetLink = $"{Request.Scheme}://{Request.Host}/reset-password?token={resetToken}";
        
        // Gửi email đặt lại mật khẩu
        await _emailService.SendPasswordResetEmailAsync(
            to: user.Email,
            fullName: user.FullName,
            resetLink: resetLink,
            expiryMinutes: 30
        );
        
        return Ok(new { message = "Link đặt lại mật khẩu đã được gửi đến email của bạn." });
    }
}
```

## 🔧 Troubleshooting

### Lỗi: Template file not found
- Đảm bảo các file HTML được copy vào thư mục output khi build
- Thêm vào file `.csproj`:
```xml
<ItemGroup>
  <None Update="Templates\Email\*.html">
    <CopyToOutputDirectory>Always</CopyToOutputDirectory>
  </None>
</ItemGroup>
```

### Email không hiển thị đúng định dạng
- Đảm bảo `IsBodyHtml = true` trong `MailMessage`
- Kiểm tra CSS inline (một số email client không hỗ trợ external CSS)

### Không gửi được email
- Kiểm tra cấu hình SMTP
- Kiểm tra firewall/antivirus
- Với Gmail: đảm bảo đã bật "Less secure app access" hoặc sử dụng App Password

## 📚 Tài liệu tham khảo

- [Microsoft Docs - Send Email](https://learn.microsoft.com/en-us/dotnet/api/system.net.mail.smtpclient)
- [Gmail SMTP Settings](https://support.google.com/mail/answer/7126229)
- [HTML Email Best Practices](https://www.campaignmonitor.com/css/)
