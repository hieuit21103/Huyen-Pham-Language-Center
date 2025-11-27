using FluentValidation;
using MsHuyenLC.Application.DTOs.Auth;

namespace MsHuyenLC.Application.Validators.Auth;

public class ChangePasswordRequestValidator : AbstractValidator<ChangePasswordRequest>
{
    public ChangePasswordRequestValidator()
    {
        RuleFor(x => x.MatKhauCu)
            .NotEmpty().WithMessage("Mật khẩu cũ không được để trống");

        RuleFor(x => x.MatKhauMoi)
            .NotEmpty().WithMessage("Mật khẩu mới không được để trống")
            .MinimumLength(8).WithMessage("Mật khẩu mới phải ít nhất 8 ký tự")
            .Matches(@"[A-Z]").WithMessage("Mật khẩu mới phải có ít nhất 1 chữ hoa")
            .Matches(@"[a-z]").WithMessage("Mật khẩu mới phải có ít nhất 1 chữ thường")
            .Matches(@"\d").WithMessage("Mật khẩu mới phải có ít nhất 1 số")
            .NotEqual(x => x.MatKhauCu).WithMessage("Mật khẩu mới phải khác mật khẩu cũ");
    }
}
