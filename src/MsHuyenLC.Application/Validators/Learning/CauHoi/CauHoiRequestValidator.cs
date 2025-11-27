using FluentValidation;
using MsHuyenLC.Application.DTOs.Learning.CauHoi;
using MsHuyenLC.Domain.Enums;

namespace MsHuyenLC.Application.Validators.Learning;

public class CauHoiRequestValidator : AbstractValidator<CauHoiRequest>
{
    public CauHoiRequestValidator()
    {
        RuleFor(x => x.NoiDungCauHoi)
            .NotEmpty().WithMessage("Nội dung câu hỏi không được để trống")
            .MaximumLength(2000).WithMessage("Nội dung câu hỏi không được vượt quá 2000 ký tự");

        RuleFor(x => x.KyNang)
            .IsInEnum().WithMessage("Kỹ năng không hợp lệ");

        RuleFor(x => x.CapDo)
            .IsInEnum().WithMessage("Cấp độ không hợp lệ");

        RuleFor(x => x.DoKho)
            .IsInEnum().WithMessage("Độ khó không hợp lệ");

        RuleFor(x => x.UrlHinhAnh)
            .MaximumLength(500).WithMessage("URL hình ảnh không được vượt quá 500 ký tự")
            .When(x => !string.IsNullOrEmpty(x.UrlHinhAnh));

        RuleFor(x => x.UrlAmThanh)
            .MaximumLength(500).WithMessage("URL âm thanh không được vượt quá 500 ký tự")
            .When(x => !string.IsNullOrEmpty(x.UrlAmThanh));

        RuleFor(x => x.LoiThoai)
            .MaximumLength(1000).WithMessage("Lời thoại không được vượt quá 1000 ký tự")
            .When(x => !string.IsNullOrEmpty(x.LoiThoai));

        RuleFor(x => x.CacDapAn)
            .NotEmpty().WithMessage("Câu hỏi phải có ít nhất 1 đáp án")
            .Must(x => x != null && x.Count >= 2).WithMessage("Câu hỏi phải có ít nhất 2 đáp án")
            .When(x => x.KyNang != KyNang.Viet);
    }
}
