using FluentValidation;
using MsHuyenLC.Application.DTOs.Learning.DeThi;
using MsHuyenLC.Application.Interfaces;

namespace MsHuyenLC.Application.Validators.Learning;

public class GenerateTestWithDifficultyRequestValidator : AbstractValidator<GenerateTestWithDifficultyRequest>
{
    private readonly IUnitOfWork _unitOfWork;

    public GenerateTestWithDifficultyRequestValidator(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;

        RuleFor(x => x.TenDe)
            .NotEmpty().WithMessage("Tên đề không được để trống")
            .MinimumLength(3).WithMessage("Tên đề phải có ít nhất 3 ký tự")
            .MaximumLength(200).WithMessage("Tên đề không được quá 200 ký tự");

        RuleFor(x => x.SoCauDe)
            .GreaterThanOrEqualTo(0).WithMessage("Số câu dễ phải lớn hơn hoặc bằng 0")
            .LessThanOrEqualTo(100).WithMessage("Số câu dễ không được vượt quá 100");

        RuleFor(x => x.SoCauTrungBinh)
            .GreaterThanOrEqualTo(0).WithMessage("Số câu trung bình phải lớn hơn hoặc bằng 0")
            .LessThanOrEqualTo(100).WithMessage("Số câu trung bình không được vượt quá 100");

        RuleFor(x => x.SoCauKho)
            .GreaterThanOrEqualTo(0).WithMessage("Số câu khó phải lớn hơn hoặc bằng 0")
            .LessThanOrEqualTo(100).WithMessage("Số câu khó không được vượt quá 100");

        RuleFor(x => x)
            .Must(x => (x.SoCauDe + x.SoCauTrungBinh + x.SoCauKho) > 0)
            .WithMessage("Tổng số câu hỏi phải lớn hơn 0")
            .Must(x => (x.SoCauDe + x.SoCauTrungBinh + x.SoCauKho) <= 200)
            .WithMessage("Tổng số câu hỏi không được vượt quá 200");

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
