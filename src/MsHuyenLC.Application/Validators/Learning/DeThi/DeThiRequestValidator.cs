using FluentValidation;
using MsHuyenLC.Domain.Entities.Learning;

namespace MsHuyenLC.Application.Validators.Learning;

public class DeThiRequestValidator : AbstractValidator<DeThiRequest>
{
    public DeThiRequestValidator()
    {
        RuleFor(x => x.TenDe)
            .NotEmpty().WithMessage("Tên đề không được để trống")
            .Length(3, 200).WithMessage("Tên đề phải từ 3-200 ký tự");

        RuleFor(x => x.TongCauHoi)
            .GreaterThan(0).WithMessage("Tổng số câu hỏi phải lớn hơn 0")
            .LessThanOrEqualTo(200).WithMessage("Tổng số câu hỏi không được vượt quá 200");

        RuleFor(x => x.LoaiDeThi)
            .IsInEnum().WithMessage("Loại đề thi không hợp lệ");

        RuleFor(x => x.ThoiGianLamBai)
            .GreaterThan(0).WithMessage("Thời gian làm bài phải lớn hơn 0")
            .LessThanOrEqualTo(300).WithMessage("Thời gian làm bài không được vượt quá 300 phút");

        RuleFor(x => x.CauHoiIds)
            .NotEmpty().WithMessage("Đề thi phải có ít nhất 1 câu hỏi")
            .Must((request, cauHoiIds) => cauHoiIds.Count == request.TongCauHoi)
            .WithMessage("Số lượng câu hỏi không khớp với tổng số câu hỏi");
    }
}
