using FluentValidation;
using MsHuyenLC.Application.DTOs.Courses.KhoaHoc;

namespace MsHuyenLC.Application.Validators.Courses;

public class KhoaHocRequestValidator : AbstractValidator<KhoaHocRequest>
{
    public KhoaHocRequestValidator()
    {
        RuleFor(x => x.TenKhoaHoc)
            .NotEmpty().WithMessage("Tên khóa học không được để trống")
            .Length(3, 200).WithMessage("Tên khóa học phải từ 3-200 ký tự");

        RuleFor(x => x.MoTa)
            .MaximumLength(1000).WithMessage("Mô tả không được vượt quá 1000 ký tự")
            .When(x => !string.IsNullOrEmpty(x.MoTa));

        RuleFor(x => x.HocPhi)
            .GreaterThanOrEqualTo(0).WithMessage("Học phí không được âm")
            .LessThanOrEqualTo(1_000_000_000).WithMessage("Học phí không được vượt quá 1 tỷ");

        RuleFor(x => x.ThoiLuong)
            .GreaterThan(0).WithMessage("Thời lượng phải lớn hơn 0")
            .LessThanOrEqualTo(365).WithMessage("Thời lượng không được vượt quá 365 ngày");

        RuleFor(x => x.NgayKhaiGiang)
            .GreaterThanOrEqualTo(DateOnly.FromDateTime(DateTime.Now.AddDays(-1)))
            .WithMessage("Ngày khai giảng phải từ ngày hiện tại trở đi");
    }
}
