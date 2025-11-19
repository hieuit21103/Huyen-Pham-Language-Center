using FluentValidation;
using MsHuyenLC.Application.DTOs.Learning.DeThi;
using MsHuyenLC.Application.Interfaces;

public class GenerateExamRequestValidator : AbstractValidator<GenerateExamRequest>
{
    private readonly IUnitOfWork _unitOfWork;

    public GenerateExamRequestValidator(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;

        RuleFor(x => x.KyThiId)
            .NotEmpty().WithMessage("Kỳ thi ID không được để trống")
            .MustAsync(ExistsKyThi).WithMessage("Kỳ thi không tồn tại");

        RuleFor(x => x.HocVienId)
            .NotEmpty().WithMessage("Học viên ID không được để trống");

        RuleFor(x => x.NguoiTaoId)
            .NotEmpty().WithMessage("Người tạo ID không được để trống");
    }

    private async Task<bool> ExistsKyThi(Guid kyThiId, CancellationToken cancellationToken)
    {
        return await _unitOfWork.KyThis.ExistsAsync(x => x.Id == kyThiId);
    }
}