using FluentValidation;
using MsHuyenLC.Application.DTOs.Learning.NhomCauHoi;
using MsHuyenLC.Application.Interfaces;

namespace MsHuyenLC.Application.Validators.Learning;

public class NhomCauHoiChiTietRequestValidator : AbstractValidator<NhomCauHoiChiTietRequest>
{
    private readonly IUnitOfWork _unitOfWork;

    public NhomCauHoiChiTietRequestValidator(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;

        RuleFor(x => x.NhomId)
            .NotEmpty().WithMessage("Nhóm câu hỏi không được để trống")
            .MustAsync(NhomExists).WithMessage("Nhóm câu hỏi không tồn tại");

        RuleFor(x => x.CauHoiId)
            .NotEmpty().WithMessage("Câu hỏi không được để trống")
            .MustAsync(CauHoiExists).WithMessage("Câu hỏi không tồn tại");

        RuleFor(x => x.ThuTu)
            .GreaterThan(0).WithMessage("Thứ tự phải lớn hơn 0");
    }

    private async Task<bool> NhomExists(Guid nhomId, CancellationToken cancellationToken)
    {
        return await _unitOfWork.NhomCauHois.ExistsAsync(x => x.Id == nhomId);
    }

    private async Task<bool> CauHoiExists(Guid cauHoiId, CancellationToken cancellationToken)
    {
        return await _unitOfWork.CauHois.ExistsAsync(x => x.Id == cauHoiId);
    }
}
