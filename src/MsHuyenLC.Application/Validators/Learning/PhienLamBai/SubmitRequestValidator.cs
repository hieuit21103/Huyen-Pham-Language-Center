using FluentValidation;
using MsHuyenLC.Application.DTOs.Learning.PhienLamBai;

namespace MsHuyenLC.Application.Validators.Learning;

public class SubmitRequestValidator : AbstractValidator<SubmitRequest>
{
    public SubmitRequestValidator()
    {
        RuleFor(x => x.DeThiId)
            .NotEmpty().WithMessage("Đề thi không được để trống")
            .Must(BeValidGuid).WithMessage("Đề thi không hợp lệ");

        RuleFor(x => x.TongCauHoi)
            .GreaterThan(0).WithMessage("Tổng số câu hỏi phải lớn hơn 0");

        RuleFor(x => x.CacTraLoi)
            .NotEmpty().WithMessage("Phải có ít nhất 1 câu trả lời")
            .Must((request, cacTraLoi) => cacTraLoi.Count <= request.TongCauHoi)
            .WithMessage("Số câu trả lời không được vượt quá tổng số câu hỏi");

        RuleFor(x => x.ThoiGianLamBai)
            .GreaterThan(0).WithMessage("Thời gian làm bài phải lớn hơn 0");
    }

    private bool BeValidGuid(string id)
    {
        return Guid.TryParse(id, out _);
    }
}
