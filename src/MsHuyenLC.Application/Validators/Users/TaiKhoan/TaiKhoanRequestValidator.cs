using FluentValidation;
using MsHuyenLC.Application.DTOs.Users.TaiKhoan;
using MsHuyenLC.Application.Interfaces;

namespace MsHuyenLC.Application.Validators.Users;

public class TaiKhoanRequestValidator : AbstractValidator<TaiKhoanRequest>
{
    private readonly IUnitOfWork _unitOfWork;

    public TaiKhoanRequestValidator(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;

        RuleFor(x => x.TenDangNhap)
            .NotEmpty().WithMessage("Tên đăng nhập không được để trống")
            .Length(3, 50).WithMessage("Tên đăng nhập phải từ 3-50 ký tự")
            .Matches(@"^[a-zA-Z0-9_]+$").WithMessage("Tên đăng nhập chỉ được chứa chữ, số và dấu gạch dưới")
            .MustAsync(BeUniqueUsername).WithMessage("Tên đăng nhập đã tồn tại");

        RuleFor(x => x.MatKhau)
            .NotEmpty().WithMessage("Mật khẩu không được để trống")
            .MinimumLength(8).WithMessage("Mật khẩu phải ít nhất 8 ký tự")
            .Matches(@"[A-Z]").WithMessage("Mật khẩu phải có ít nhất 1 chữ hoa")
            .Matches(@"[a-z]").WithMessage("Mật khẩu phải có ít nhất 1 chữ thường")
            .Matches(@"\d").WithMessage("Mật khẩu phải có ít nhất 1 số");

        RuleFor(x => x.Email)
            .EmailAddress().WithMessage("Email không hợp lệ")
            .When(x => !string.IsNullOrEmpty(x.Email))
            .MustAsync(BeUniqueEmail).WithMessage("Email đã tồn tại")
            .When(x => !string.IsNullOrEmpty(x.Email));

        RuleFor(x => x.Sdt)
            .Matches(@"^(0|\+84)[0-9]{9}$").WithMessage("Số điện thoại không hợp lệ")
            .When(x => !string.IsNullOrEmpty(x.Sdt));

        RuleFor(x => x.VaiTro)
            .IsInEnum().WithMessage("Vai trò không hợp lệ");
    }

    private async Task<bool> BeUniqueUsername(string username, CancellationToken cancellationToken)
    {
        return !await _unitOfWork.TaiKhoans.ExistsAsync(u => u.TenDangNhap == username);
    }

    private async Task<bool> BeUniqueEmail(string? email, CancellationToken cancellationToken)
    {
        if (string.IsNullOrEmpty(email)) return true;
        return !await _unitOfWork.TaiKhoans.ExistsAsync(u => u.Email == email);
    }
}
