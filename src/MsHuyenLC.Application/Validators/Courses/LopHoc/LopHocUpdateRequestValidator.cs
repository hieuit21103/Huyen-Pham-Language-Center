using FluentValidation;
using MsHuyenLC.Application.DTOs.Courses.LopHoc;

namespace MsHuyenLC.Application.Validators.Courses;

public class LopHocUpdateRequestValidator : AbstractValidator<LopHocUpdateRequest>
{
    public LopHocUpdateRequestValidator()
    {
        RuleFor(x => x.TenLop)
            .NotEmpty().WithMessage("Tên lớp không được để trống")
            .Length(3, 100).WithMessage("Tên lớp phải từ 3-100 ký tự");

        RuleFor(x => x.SiSoToiDa)
            .GreaterThan(0).WithMessage("Sĩ số tối đa phải lớn hơn 0")
            .LessThanOrEqualTo(200).WithMessage("Sĩ số tối đa không được vượt quá 200");

        RuleFor(x => x.TrangThai)
            .IsInEnum().WithMessage("Trạng thái không hợp lệ");
    }
}
