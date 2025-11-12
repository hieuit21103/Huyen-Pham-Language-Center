using FluentValidation;
using MsHuyenLC.Application.DTOs.System.CauHinhHeThong;

namespace MsHuyenLC.Application.Validators.System;

public class CauHinhHeThongUpdateRequestValidator : AbstractValidator<CauHinhHeThongUpdateRequest>
{
    public CauHinhHeThongUpdateRequestValidator()
    {
        When(x => !string.IsNullOrWhiteSpace(x.Ten), () =>
        {
            RuleFor(x => x.Ten)
                .MinimumLength(3).WithMessage("Tên cấu hình phải có ít nhất 3 ký tự")
                .MaximumLength(100).WithMessage("Tên cấu hình không được quá 100 ký tự");
        });

        When(x => !string.IsNullOrWhiteSpace(x.GiaTri), () =>
        {
            RuleFor(x => x.GiaTri)
                .MaximumLength(500).WithMessage("Giá trị không được quá 500 ký tự");
        });
    }
}
