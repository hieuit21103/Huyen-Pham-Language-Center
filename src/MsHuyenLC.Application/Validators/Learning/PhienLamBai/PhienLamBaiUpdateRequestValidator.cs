using FluentValidation;
using MsHuyenLC.Application.DTOs.Learning.PhienLamBai;

namespace MsHuyenLC.Application.Validators.Learning;

public class PhienLamBaiUpdateRequestValidator : AbstractValidator<PhienLamBaiUpdateRequest>
{
    public PhienLamBaiUpdateRequestValidator()
    {
        When(x => x.Diem.HasValue, () =>
        {
            RuleFor(x => x.Diem)
                .GreaterThanOrEqualTo(0).WithMessage("Điểm phải lớn hơn hoặc bằng 0")
                .LessThanOrEqualTo(10).WithMessage("Điểm không được vượt quá 10");
        });
    }
}
