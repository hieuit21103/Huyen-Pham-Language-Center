using FluentValidation;
using MsHuyenLC.Application.DTOs.Finance.ThanhToan;
using MsHuyenLC.Application.Interfaces;

namespace MsHuyenLC.Application.Validators.Finance;

public class ThanhToanRequestValidator : AbstractValidator<ThanhToanRequest>
{
    private readonly IUnitOfWork _unitOfWork;

    public ThanhToanRequestValidator(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;

        RuleFor(x => x.DangKyId)
            .NotEmpty().WithMessage("Đăng ký không được để trống")
            .MustAsync(DangKyExists).WithMessage("Đăng ký không tồn tại");

        RuleFor(x => x.SoTien)
            .GreaterThan(0).WithMessage("Số tiền phải lớn hơn 0")
            .LessThanOrEqualTo(1000000000).WithMessage("Số tiền không được vượt quá 1 tỷ");
    }

    private async Task<bool> DangKyExists(Guid dangKyId, CancellationToken cancellationToken)
    {
        return await _unitOfWork.DangKys.ExistsAsync(x => x.Id == dangKyId);
    }
}
