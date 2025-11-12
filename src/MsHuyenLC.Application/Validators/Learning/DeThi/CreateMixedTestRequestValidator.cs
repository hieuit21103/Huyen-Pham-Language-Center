using FluentValidation;
using MsHuyenLC.Application.DTOs.Learning.DeThi;
using MsHuyenLC.Application.Interfaces;

namespace MsHuyenLC.Application.Validators.Learning;

public class CreateMixedTestRequestValidator : AbstractValidator<CreateMixedTestRequest>
{
    private readonly IUnitOfWork _unitOfWork;

    public CreateMixedTestRequestValidator(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;

        RuleFor(x => x.TenDe)
            .NotEmpty().WithMessage("Tên đề không được để trống")
            .MinimumLength(3).WithMessage("Tên đề phải có ít nhất 3 ký tự")
            .MaximumLength(200).WithMessage("Tên đề không được quá 200 ký tự");

        RuleFor(x => x.ThoiGianLamBai)
            .GreaterThan(0).WithMessage("Thời gian làm bài phải lớn hơn 0")
            .LessThanOrEqualTo(300).WithMessage("Thời gian làm bài không được vượt quá 300 phút");

        RuleFor(x => x.LoaiDeThi)
            .IsInEnum().WithMessage("Loại đề thi không hợp lệ");

        RuleFor(x => x)
            .Must(x => x.NhomCauHoiIds.Count > 0 || x.CauHoiDocLapIds.Count > 0)
            .WithMessage("Phải có ít nhất 1 nhóm câu hỏi hoặc 1 câu hỏi độc lập");

        When(x => !string.IsNullOrWhiteSpace(x.KyThiId), () =>
        {
            RuleFor(x => x.KyThiId)
                .Must(BeValidGuid).WithMessage("ID kỳ thi không hợp lệ")
                .MustAsync(KyThiExists).WithMessage("Kỳ thi không tồn tại");
        });

        When(x => x.NhomCauHoiIds.Count > 0, () =>
        {
            RuleForEach(x => x.NhomCauHoiIds)
                .Must(BeValidGuid).WithMessage("ID nhóm câu hỏi không hợp lệ")
                .MustAsync(NhomCauHoiExists).WithMessage("Nhóm câu hỏi không tồn tại");
        });

        When(x => x.CauHoiDocLapIds.Count > 0, () =>
        {
            RuleForEach(x => x.CauHoiDocLapIds)
                .Must(BeValidGuid).WithMessage("ID câu hỏi không hợp lệ")
                .MustAsync(CauHoiExists).WithMessage("Câu hỏi không tồn tại");
        });
    }

    private bool BeValidGuid(string? id)
    {
        return Guid.TryParse(id, out _);
    }

    private async Task<bool> KyThiExists(string? kyThiId, CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(kyThiId) || !Guid.TryParse(kyThiId, out var id))
            return false;

        return await _unitOfWork.KyThis.ExistsAsync(x => x.Id == id);
    }

    private async Task<bool> NhomCauHoiExists(string nhomCauHoiId, CancellationToken cancellationToken)
    {
        if (!Guid.TryParse(nhomCauHoiId, out var id))
            return false;

        return await _unitOfWork.NhomCauHois.ExistsAsync(x => x.Id == id);
    }

    private async Task<bool> CauHoiExists(string cauHoiId, CancellationToken cancellationToken)
    {
        if (!Guid.TryParse(cauHoiId, out var id))
            return false;

        return await _unitOfWork.CauHois.ExistsAsync(x => x.Id == id);
    }
}
