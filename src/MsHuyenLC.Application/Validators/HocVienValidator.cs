using FluentValidation;
using MsHuyenLC.Domain.Entities.Users;

namespace MsHuyenLC.Application.Validators;

public class HocVienValidator : AbstractValidator<HocVien>
{
    public HocVienValidator()
    {
        // Kiểm tra họ tên
        RuleFor(x => x.HoTen)
            .NotEmpty().WithMessage("Họ tên không được để trống")
            .MinimumLength(2).WithMessage("Họ tên phải có ít nhất 2 ký tự")
            .MaximumLength(100).WithMessage("Họ tên không được vượt quá 100 ký tự")
            .Matches(@"^[\p{L}\s]+$").WithMessage("Họ tên chỉ được chứa chữ cái và khoảng trắng");

        // Kiểm tra ngày sinh
        RuleFor(x => x.NgaySinh)
            .LessThan(DateTime.Now.Date).WithMessage("Ngày sinh phải trong quá khứ")
            .GreaterThan(DateTime.Now.AddYears(-100)).WithMessage("Ngày sinh không hợp lệ")
            .When(x => x.NgaySinh.HasValue);

        // Kiểm tra tuổi tối thiểu (ví dụ: 6 tuổi)
        RuleFor(x => x.NgaySinh)
            .Must(ngaySinh => ngaySinh.HasValue && 
                              DateTime.Now.Year - ngaySinh.Value.Year >= 6)
            .WithMessage("Học viên phải đủ 6 tuổi")
            .When(x => x.NgaySinh.HasValue);

        // Kiểm tra địa chỉ
        RuleFor(x => x.DiaChi)
            .MaximumLength(500).WithMessage("Địa chỉ không được vượt quá 500 ký tự")
            .When(x => !string.IsNullOrWhiteSpace(x.DiaChi));

        // Kiểm tra ngày đăng ký
        RuleFor(x => x.NgayDangKy)
            .LessThanOrEqualTo(DateTime.Now).WithMessage("Ngày đăng ký không được trong tương lai");
    }
}
