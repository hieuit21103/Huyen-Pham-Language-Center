using FluentValidation;
using MsHuyenLC.Application.DTOs.Learning.KetQuaHocTap;
using MsHuyenLC.Application.Interfaces;

namespace MsHuyenLC.Application.Validators.Learning;

public class KetQuaHocTapRequestValidator : AbstractValidator<KetQuaHocTapRequest>
{
    private readonly IUnitOfWork _unitOfWork;

    public KetQuaHocTapRequestValidator(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;

        RuleFor(x => x.HocVienId)
            .NotEmpty().WithMessage("Học viên không được để trống")
            .MustAsync(HocVienExists).WithMessage("Học viên không tồn tại");

        RuleFor(x => x.KyThiId)
            .NotEmpty().WithMessage("Kỳ thi không được để trống")
            .MustAsync(KyThiExists).WithMessage("Kỳ thi không tồn tại");

        RuleFor(x => x.DiemSo)
            .GreaterThanOrEqualTo(0).WithMessage("Điểm số phải lớn hơn hoặc bằng 0")
            .LessThanOrEqualTo(10).WithMessage("Điểm số không được vượt quá 10");
    }

    private async Task<bool> HocVienExists(Guid hocVienId, CancellationToken cancellationToken)
    {
        return await _unitOfWork.HocViens.ExistsAsync(x => x.Id == hocVienId);
    }

    private async Task<bool> KyThiExists(Guid kyThiId, CancellationToken cancellationToken)
    {
        return await _unitOfWork.KyThis.ExistsAsync(x => x.Id == kyThiId);
    }
}
