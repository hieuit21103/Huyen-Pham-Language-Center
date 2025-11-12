using System.Data;
using FluentValidation;
using MsHuyenLC.Application.DTOs.Courses.LichHoc;
using MsHuyenLC.Application.Interfaces;

namespace MsHuyenLC.Application.Validators.Courses;

public class LichHocUpdateRequestValidator : AbstractValidator<LichHocUpdateRequest>
{
    private readonly IUnitOfWork _unitOfWork;

    public LichHocUpdateRequestValidator(IUnitOfWork unitOfWork)
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
            .NotEmpty().WithMessage("Giờ bắt đầu không được để trống");

        RuleFor(x => x.GioKetThuc)
            .NotEmpty().WithMessage("Giờ kết thúc không được để trống")
            .Must((model, gioKetThuc) => gioKetThuc > model.GioBatDau)
            .WithMessage("Giờ kết thúc phải sau giờ bắt đầu");

        RuleFor(x => x.TuNgay)
            .NotEmpty().WithMessage("Từ ngày không được để trống");

        RuleFor(x => x.DenNgay)
            .NotEmpty().WithMessage("Đến ngày không được để trống")
            .Must((model, denNgay) => denNgay >= model.TuNgay)
            .WithMessage("Đến ngày phải sau hoặc bằng từ ngày");
    }

    private async Task<bool> PhongHocExists(Guid phongHocId, CancellationToken cancellationToken)
    {
        return await _unitOfWork.PhongHocs.ExistsAsync(x => x.Id == phongHocId);
    }

    private async Task<bool> LopHocExists(Guid lopHocId, CancellationToken cancellationToken)
    {
        return await _unitOfWork.LopHocs.ExistsAsync(x => x.Id == lopHocId);
    }
}
