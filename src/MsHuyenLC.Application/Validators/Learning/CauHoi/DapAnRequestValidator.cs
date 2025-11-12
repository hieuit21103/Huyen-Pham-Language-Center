using FluentValidation;
using MsHuyenLC.Application.DTOs.Learning.CauHoi;

namespace MsHuyenLC.Application.Validators.Learning;

public class DapAnRequestValidator : AbstractValidator<DapAnRequest>
{
    public DapAnRequestValidator()
    {
        RuleFor(x => x.Nhan)
            .NotEmpty().WithMessage("Nhãn đáp án không được để trống")
            .MaximumLength(10).WithMessage("Nhãn đáp án không được quá 10 ký tự")
            .Matches(@"^[A-Z]$").WithMessage("Nhãn đáp án phải là chữ cái in hoa (A, B, C, D,...)");

        RuleFor(x => x.NoiDung)
            .NotEmpty().WithMessage("Nội dung đáp án không được để trống")
            .MaximumLength(500).WithMessage("Nội dung đáp án không được quá 500 ký tự");

        When(x => !string.IsNullOrWhiteSpace(x.GiaiThich), () =>
        {
            RuleFor(x => x.GiaiThich)
                .MaximumLength(1000).WithMessage("Giải thích không được quá 1000 ký tự");
        });
    }
}
