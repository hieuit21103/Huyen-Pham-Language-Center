using FluentValidation;
using MsHuyenLC.Application.DTOs.Learning.DeThi;
using MsHuyenLC.Application.Interfaces;

namespace MsHuyenLC.Application.Validators.Learning;

public class DeThiUpdateRequestValidator : AbstractValidator<DeThiUpdateRequest>
{
    private readonly IUnitOfWork _unitOfWork;

    public DeThiUpdateRequestValidator(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;

        RuleFor(x => x.TenDe)
            .NotEmpty().WithMessage("Tên đề không được để trống")
            .MinimumLength(3).WithMessage("Tên đề phải có ít nhất 3 ký tự")
            .MaximumLength(200).WithMessage("Tên đề không được quá 200 ký tự");

        RuleFor(x => x.TongCauHoi)
            .GreaterThan(0).WithMessage("Tổng câu hỏi phải lớn hơn 0")
            .LessThanOrEqualTo(200).WithMessage("Tổng câu hỏi không được vượt quá 200");

        RuleFor(x => x.ThoiGianLamBai)
            .GreaterThan(0).WithMessage("Thời gian làm bài phải lớn hơn 0")
            .LessThanOrEqualTo(300).WithMessage("Thời gian làm bài không được vượt quá 300 phút");

        RuleFor(x => x.LoaiDeThi)
            .IsInEnum().WithMessage("Loại đề thi không hợp lệ");

        When(x => x.KyThiId.HasValue, () =>
        {
            RuleFor(x => x.KyThiId!.Value)
                .MustAsync(KyThiExists).WithMessage("Kỳ thi không tồn tại");
        });

        RuleFor(x => x.CauHoiIds)
            .NotEmpty().WithMessage("Danh sách câu hỏi không được để trống")
            .Must((model, cauHoiIds) => cauHoiIds.Count == model.TongCauHoi)
            .WithMessage("Số lượng câu hỏi phải khớp với tổng câu hỏi");

        RuleForEach(x => x.CauHoiIds)
            .Must(BeValidGuid).WithMessage("ID câu hỏi không hợp lệ")
            .MustAsync(CauHoiExists).WithMessage("Câu hỏi không tồn tại");
    }

    private bool BeValidGuid(string id)
    {
        return Guid.TryParse(id, out _);
    }

    private async Task<bool> KyThiExists(Guid kyThiId, CancellationToken cancellationToken)
    {
        return await _unitOfWork.KyThis.ExistsAsync(x => x.Id == kyThiId);
    }

    private async Task<bool> CauHoiExists(string cauHoiId, CancellationToken cancellationToken)
    {
        if (!Guid.TryParse(cauHoiId, out var id))
            return false;

        return await _unitOfWork.CauHois.ExistsAsync(x => x.Id == id);
    }
}
