using FluentValidation;
using MsHuyenLC.Application.DTOs.Users.HocVien;

namespace MsHuyenLC.Application.Validators.Users;

public class HocVienUpdateRequestValidator : AbstractValidator<HocVienUpdateRequest>
{
    public HocVienUpdateRequestValidator()
    {
        RuleFor(x => x.HoTen)
            .NotEmpty().WithMessage("Họ tên không được để trống")
            .Length(3, 100).WithMessage("Họ tên phải từ 3-100 ký tự")
            .Matches(@"^[\p{L}\s]+$").WithMessage("Họ tên chỉ được chứa chữ cái và khoảng trắng");

        RuleFor(x => x.NgaySinh)
            .LessThan(DateOnly.FromDateTime(DateTime.Now)).WithMessage("Ngày sinh phải nhỏ hơn ngày hiện tại")
            .GreaterThan(DateOnly.FromDateTime(DateTime.Now.AddYears(-100))).WithMessage("Ngày sinh không hợp lệ")
            .When(x => x.NgaySinh.HasValue);

        RuleFor(x => x.GioiTinh)
            .IsInEnum().WithMessage("Giới tính không hợp lệ")
            .When(x => x.GioiTinh.HasValue);

        RuleFor(x => x.DiaChi)
            .MaximumLength(200).WithMessage("Địa chỉ không được vượt quá 200 ký tự")
            .When(x => !string.IsNullOrEmpty(x.DiaChi));

        RuleFor(x => x.TrangThai)
            .IsInEnum().WithMessage("Trạng thái không hợp lệ");
    }
}
