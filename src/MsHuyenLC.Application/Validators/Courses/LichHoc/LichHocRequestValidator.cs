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

        RuleFor(x => x.Thu)
            .IsInEnum().WithMessage("Thứ không hợp lệ");

        RuleFor(x => x.GioBatDau)
            .LessThan(x => x.GioKetThuc).WithMessage("Giờ bắt đầu phải nhỏ hơn giờ kết thúc");

        RuleFor(x => x.GioKetThuc)
            .GreaterThan(x => x.GioBatDau).WithMessage("Giờ kết thúc phải lớn hơn giờ bắt đầu");
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
