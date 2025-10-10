using FluentValidation;
using MsHuyenLC.Domain.Entities.Courses;

namespace MsHuyenLC.Application.Validators;

public class KhoaHocValidator : AbstractValidator<KhoaHoc>
{
    public KhoaHocValidator()
    {
        // Kiểm tra tên khóa học
        RuleFor(x => x.TenKhoaHoc)
            .NotEmpty().WithMessage("Tên khóa học không được để trống")
            .MinimumLength(3).WithMessage("Tên khóa học phải có ít nhất 3 ký tự")
            .MaximumLength(200).WithMessage("Tên khóa học không được vượt quá 200 ký tự");

        // Kiểm tra học phí
        RuleFor(x => x.HocPhi)
            .GreaterThanOrEqualTo(0).WithMessage("Học phí không được âm");

        // Kiểm tra thời lượng
        RuleFor(x => x.ThoiLuong)
            .GreaterThan(0).WithMessage("Thời lượng khóa học phải lớn hơn 0");

        // Kiểm tra ngày khai giảng
        RuleFor(x => x.NgayKhaiGiang)
            .GreaterThanOrEqualTo(DateTime.Now.Date)
            .WithMessage("Ngày khai giảng không được trong quá khứ");

        // Kiểm tra mô tả nếu có
        RuleFor(x => x.MoTa)
            .MaximumLength(1000).WithMessage("Mô tả không được vượt quá 1000 ký tự")
            .When(x => !string.IsNullOrWhiteSpace(x.MoTa));
    }
}
