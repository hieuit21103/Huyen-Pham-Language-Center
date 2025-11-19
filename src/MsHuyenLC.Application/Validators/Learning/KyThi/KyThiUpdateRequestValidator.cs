using FluentValidation;
using MsHuyenLC.Application.DTOs.Learning.KyThi;
using MsHuyenLC.Application.Interfaces;

namespace MsHuyenLC.Application.Validators.Learning;

public class KyThiUpdateRequestValidator : AbstractValidator<KyThiUpdateRequest>
{
    private readonly IUnitOfWork _unitOfWork;

    public KyThiUpdateRequestValidator(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;

        RuleFor(x => x.TenKyThi)
            .NotEmpty().WithMessage("Tên kỳ thi không được để trống")
            .MinimumLength(3).WithMessage("Tên kỳ thi phải có ít nhất 3 ký tự")
            .MaximumLength(200).WithMessage("Tên kỳ thi không được quá 200 ký tự");

        RuleFor(x => x.NgayThi)
            .NotEmpty().WithMessage("Ngày thi không được để trống");

        RuleFor(x => x.GioBatDau)
            .NotEmpty().WithMessage("Giờ bắt đầu không được để trống");

        RuleFor(x => x.GioKetThuc)
            .NotEmpty().WithMessage("Giờ kết thúc không được để trống")
            .Must((model, gioKetThuc) => gioKetThuc > model.GioBatDau)
            .WithMessage("Giờ kết thúc phải sau giờ bắt đầu");

        RuleFor(x => x.ThoiLuong)
            .GreaterThan(0).WithMessage("Thời lượng phải lớn hơn 0")
            .LessThanOrEqualTo(300).WithMessage("Thời lượng không được vượt quá 300 phút");

        When(x => x.LopHocId.HasValue, () =>
        {
            RuleFor(x => x.LopHocId)
                .MustAsync((id, ct) => LopHocExists(id!.Value, ct))
                .WithMessage("Lớp học không tồn tại");
        });

        When(x => x.TrangThai.HasValue, () =>
        {
            RuleFor(x => x.TrangThai)
                .IsInEnum().WithMessage("Trạng thái không hợp lệ");
        });
    }

    private async Task<bool> LopHocExists(Guid lopHocId, CancellationToken cancellationToken)
    {
        return await _unitOfWork.LopHocs.ExistsAsync(x => x.Id == lopHocId);
    }
}
