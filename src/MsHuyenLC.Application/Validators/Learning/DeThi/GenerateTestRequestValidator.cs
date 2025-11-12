using FluentValidation;
using MsHuyenLC.Application.DTOs.Learning.DeThi;
using MsHuyenLC.Application.Interfaces;

namespace MsHuyenLC.Application.Validators.Learning;

public class GenerateTestRequestValidator : AbstractValidator<GenerateTestRequest>
{
    private readonly IUnitOfWork _unitOfWork;

    public GenerateTestRequestValidator(IUnitOfWork unitOfWork)
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

        RuleFor(x => x.LoaiCauHoi)
            .IsInEnum().WithMessage("Loại câu hỏi không hợp lệ");

        RuleFor(x => x.KyNang)
            .IsInEnum().WithMessage("Kỹ năng không hợp lệ");

        RuleFor(x => x.CapDo)
            .IsInEnum().WithMessage("Cấp độ không hợp lệ");

        RuleFor(x => x.DoKho)
            .IsInEnum().WithMessage("Độ khó không hợp lệ");

        When(x => x.KyThiId.HasValue, () =>
        {
            RuleFor(x => x.KyThiId!.Value)
                .MustAsync(KyThiExists).WithMessage("Kỳ thi không tồn tại");
        });
    }

    private async Task<bool> KyThiExists(Guid kyThiId, CancellationToken cancellationToken)
    {
        return await _unitOfWork.KyThis.ExistsAsync(x => x.Id == kyThiId);
    }
}
