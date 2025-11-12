using FluentValidation;
using MsHuyenLC.Application.DTOs.Users.TaiKhoan;

namespace MsHuyenLC.Application.Validators.Users;

public class TaiKhoanUpdateRequestValidator : AbstractValidator<TaiKhoanUpdateRequest>
{
    public TaiKhoanUpdateRequestValidator()
    {
        RuleFor(x => x.Email)
            .EmailAddress().WithMessage("Email không hợp lệ")
            .When(x => !string.IsNullOrEmpty(x.Email));

        RuleFor(x => x.Sdt)
            .Matches(@"^(0|\+84)[0-9]{9}$").WithMessage("Số điện thoại không hợp lệ")
            .When(x => !string.IsNullOrEmpty(x.Sdt));

        RuleFor(x => x.VaiTro)
            .IsInEnum().WithMessage("Vai trò không hợp lệ");

        RuleFor(x => x.TrangThai)
            .IsInEnum().WithMessage("Trạng thái không hợp lệ");
    }
}
