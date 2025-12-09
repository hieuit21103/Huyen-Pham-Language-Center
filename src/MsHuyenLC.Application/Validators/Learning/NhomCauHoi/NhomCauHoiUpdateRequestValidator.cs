using FluentValidation;
using MsHuyenLC.Application.DTOs.Learning.NhomCauHoi;

namespace MsHuyenLC.Application.Validators.Learning;

public class NhomCauHoiUpdateRequestValidator : AbstractValidator<NhomCauHoiUpdateRequest>
{
    public NhomCauHoiUpdateRequestValidator()
    {
        When(x => !string.IsNullOrWhiteSpace(x.TieuDe), () =>
        {
            RuleFor(x => x.TieuDe)
                .MinimumLength(3).WithMessage("Tiêu đề phải có ít nhất 3 ký tự")
                .MaximumLength(200).WithMessage("Tiêu đề không được quá 200 ký tự");
        });

        When(x => !string.IsNullOrWhiteSpace(x.NoiDung), () =>
        {
            RuleFor(x => x.NoiDung)
                .MaximumLength(2000).WithMessage("Nội dung không được quá 2000 ký tự");
        });

        When(x => x.SoLuongCauHoi.HasValue, () =>
        {
            RuleFor(x => x.SoLuongCauHoi)
                .GreaterThan(0).WithMessage("Số lượng câu hỏi phải lớn hơn 0")
                .LessThanOrEqualTo(50).WithMessage("Số lượng câu hỏi không được vượt quá 50");
        });

        When(x => !string.IsNullOrWhiteSpace(x.UrlAmThanh), () =>
        {
            RuleFor(x => x.UrlAmThanh)
                .MaximumLength(500).WithMessage("URL âm thanh không được quá 500 ký tự");
        });

        When(x => !string.IsNullOrWhiteSpace(x.UrlHinhAnh), () =>
        {
            RuleFor(x => x.UrlHinhAnh)
                .MaximumLength(500).WithMessage("URL hình ảnh không được quá 500 ký tự");
        });

        When(x => x.DoKho.HasValue, () =>
        {
            RuleFor(x => x.DoKho)
                .IsInEnum().WithMessage("Độ khó không hợp lệ");
        });

        When(x => x.CapDo.HasValue, () =>
        {
            RuleFor(x => x.CapDo)
                .IsInEnum().WithMessage("Cấp độ không hợp lệ");
        });

        When(x => x.KyNang.HasValue, () =>
        {
            RuleFor(x => x.KyNang)
                .IsInEnum().WithMessage("Kỹ năng không hợp lệ");
        });
    }
}
