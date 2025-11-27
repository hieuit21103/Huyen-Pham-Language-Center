using FluentValidation;
using MsHuyenLC.Application.DTOs.Learning.KyThi;
using MsHuyenLC.Application.Interfaces;

namespace MsHuyenLC.Application.Validators.Learning.KyThi;

public class JoinExamRequestValidator : AbstractValidator<JoinExamRequest>
{
    private readonly IUnitOfWork _unitOfWork;

    public JoinExamRequestValidator(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;

        RuleFor(x => x.KyThiId)
            .NotEmpty().WithMessage("ID kỳ thi không được để trống")
            .MustAsync(KyThiExists).WithMessage("Kỳ thi không tồn tại");

        RuleFor(x => x.HocVienId)
            .NotEmpty().WithMessage("ID học viên không được để trống")
            .MustAsync(HocVienExists).WithMessage("Học viên không tồn tại");
    }

    private async Task<bool> KyThiExists(Guid kyThiId, CancellationToken cancellationToken)
    {
        return await _unitOfWork.KyThis.ExistsAsync(k => k.Id == kyThiId);
    }

    private async Task<bool> HocVienExists(Guid hocVienId, CancellationToken cancellationToken)
    {
        return await _unitOfWork.HocViens.ExistsAsync(h => h.Id == hocVienId);
    }
}
