using FluentValidation;
using MsHuyenLC.Application.DTOs.Courses.LichHoc;
using MsHuyenLC.Application.Interfaces;

namespace MsHuyenLC.Application.Validators.Courses;

public class LichHocRequestValidator : AbstractValidator<LichHocRequest>
{
    private readonly IUnitOfWork _unitOfWork;

    public LichHocRequestValidator(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;

        RuleFor(x => x.LopHocId)
            .NotEmpty().WithMessage("Lớp học không được để trống")
            .MustAsync(LopHocExists).WithMessage("Lớp học không tồn tại");

        RuleFor(x => x.PhongHocId)
            .NotEmpty().WithMessage("Phòng học không được để trống")
            .MustAsync(PhongHocExists).WithMessage("Phòng học không tồn tại");

        RuleFor(x => x.ThoiGianBieus)
            .NotEmpty().WithMessage("Phải có ít nhất một thời gian biểu")
            .Must(tgbs => tgbs != null && tgbs.Count > 0)
            .WithMessage("Phải có ít nhất một thời gian biểu");

        RuleForEach(x => x.ThoiGianBieus).ChildRules(tgb =>
        {
            tgb.RuleFor(x => x.Thu)
                .IsInEnum().WithMessage("Thứ không hợp lệ");

            tgb.RuleFor(x => x.GioBatDau)
                .NotNull().WithMessage("Giờ bắt đầu không được để trống")
                .Must((model, gioBatDau) => gioBatDau < model.GioKetThuc)
                .WithMessage("Giờ bắt đầu phải nhỏ hơn giờ kết thúc");

            tgb.RuleFor(x => x.GioKetThuc)
                .NotNull().WithMessage("Giờ kết thúc không được để trống")
                .Must((model, gioKetThuc) => gioKetThuc > model.GioBatDau)
                .WithMessage("Giờ kết thúc phải lớn hơn giờ bắt đầu");
        });
    }

    private async Task<bool> LopHocExists(Guid lopHocId, CancellationToken cancellationToken)
    {
        return await _unitOfWork.LopHocs.ExistsAsync(l => l.Id == lopHocId);
    }

    private async Task<bool> PhongHocExists(Guid phongHocId, CancellationToken cancellationToken)
    {
        return await _unitOfWork.PhongHocs.ExistsAsync(p => p.Id == phongHocId);
    }
}
