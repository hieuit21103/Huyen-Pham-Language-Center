using FluentValidation;
using MsHuyenLC.Application.DTOs.Learning.NhomCauHoi;
using MsHuyenLC.Application.Interfaces;

namespace MsHuyenLC.Application.Validators.Learning;

public class ThemCauHoiRequestValidator : AbstractValidator<ThemCauHoiRequest>
{
    private readonly IUnitOfWork _unitOfWork;

    public ThemCauHoiRequestValidator(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;

        RuleFor(x => x.cauHoiId)
            .NotEmpty().WithMessage("Mã câu hỏi không được để trống")
            .Must(BeValidGuid).WithMessage("Mã câu hỏi không hợp lệ")
            .MustAsync(CauHoiExists).WithMessage("Câu hỏi không tồn tại");

        When(x => x.thuTu.HasValue, () =>
        {
            RuleFor(x => x.thuTu)
                .GreaterThan(0).WithMessage("Thứ tự phải lớn hơn 0");
        });
    }

    private bool BeValidGuid(string id)
    {
        return Guid.TryParse(id, out _);
    }

    private async Task<bool> CauHoiExists(string cauHoiId, CancellationToken cancellationToken)
    {
        if (!Guid.TryParse(cauHoiId, out var id))
            return false;

        return await _unitOfWork.CauHois.ExistsAsync(x => x.Id == id);
    }
}
