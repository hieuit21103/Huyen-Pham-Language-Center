using FluentValidation;
using MsHuyenLC.Application.DTOs.Courses.PhongHoc;

namespace MsHuyenLC.Application.Validators.Courses;

public class PhongHocRequestValidator : AbstractValidator<PhongHocRequest>
{
    public PhongHocRequestValidator()
    {
        RuleFor(x => x.TenPhong)
            .NotEmpty().WithMessage("Tên phòng không được để trống")
            .Length(2, 50).WithMessage("Tên phòng phải từ 2-50 ký tự");

        RuleFor(x => x.SoGhe)
            .GreaterThan(0).WithMessage("Số ghế phải lớn hơn 0")
            .LessThanOrEqualTo(500).WithMessage("Số ghế không được vượt quá 500");
    }
}
