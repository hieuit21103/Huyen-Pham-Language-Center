using FluentValidation;
using MsHuyenLC.Application.DTOs.Auth;

namespace MsHuyenLC.Application.Validators.Auth;

public class LoginRequestValidator : AbstractValidator<LoginRequest>
{
    public LoginRequestValidator()
    {
        RuleFor(x => x.TenDangNhap)
            .NotEmpty().WithMessage("Tên đăng nhập không được để trống")
            .Length(3, 50).WithMessage("Tên đăng nhập phải từ 3-50 ký tự");

        RuleFor(x => x.MatKhau)
            .NotEmpty().WithMessage("Mật khẩu không được để trống");
    }
}
