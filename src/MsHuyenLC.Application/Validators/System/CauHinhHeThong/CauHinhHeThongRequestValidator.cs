using FluentValidation;
using MsHuyenLC.Application.DTOs.System.CauHinhHeThong;

namespace MsHuyenLC.Application.Validators.System;

public class CauHinhHeThongRequestValidator : AbstractValidator<CauHinhHeThongRequest>
{
    public CauHinhHeThongRequestValidator()
    {
        RuleFor(x => x.Ten)
            .NotEmpty().WithMessage("Tên cấu hình không được để trống")
            .MinimumLength(3).WithMessage("Tên cấu hình phải có ít nhất 3 ký tự")
            .MaximumLength(100).WithMessage("Tên cấu hình không được quá 100 ký tự");

        RuleFor(x => x.GiaTri)
            .NotEmpty().WithMessage("Giá trị không được để trống")
            .MaximumLength(500).WithMessage("Giá trị không được quá 500 ký tự");
    }
}
