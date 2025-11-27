using FluentValidation;
using MsHuyenLC.Application.DTOs.Learning.NhomCauHoi;

namespace MsHuyenLC.Application.Validators.Learning;

public class NhomCauHoiRequestValidator : AbstractValidator<NhomCauHoiRequest>
{
    public NhomCauHoiRequestValidator()
    {
        RuleFor(x => x.TieuDe)
            .NotEmpty().WithMessage("Tiêu đề không được để trống")
            .Length(3, 200).WithMessage("Tiêu đề phải từ 3-200 ký tự");

        RuleFor(x => x.NoiDung)
            .MaximumLength(2000).WithMessage("Nội dung không được vượt quá 2000 ký tự")
            .When(x => !string.IsNullOrEmpty(x.NoiDung));

        RuleFor(x => x.UrlAmThanh)
            .MaximumLength(500).WithMessage("URL âm thanh không được vượt quá 500 ký tự")
            .When(x => !string.IsNullOrEmpty(x.UrlAmThanh));

        RuleFor(x => x.UrlHinhAnh)
            .MaximumLength(500).WithMessage("URL hình ảnh không được vượt quá 500 ký tự")
            .When(x => !string.IsNullOrEmpty(x.UrlHinhAnh));

        RuleFor(x => x.SoLuongCauHoi)
            .GreaterThan(0).WithMessage("Số lượng câu hỏi phải lớn hơn 0")
            .LessThanOrEqualTo(50).WithMessage("Số lượng câu hỏi không được vượt quá 50");

        RuleFor(x => x.DoKho)
            .IsInEnum().WithMessage("Độ khó không hợp lệ");

        RuleFor(x => x.CapDo)
            .IsInEnum().WithMessage("Cấp độ không hợp lệ");
    }
}
