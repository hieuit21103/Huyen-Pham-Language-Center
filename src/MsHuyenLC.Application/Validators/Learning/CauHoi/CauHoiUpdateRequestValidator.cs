using FluentValidation;
using MsHuyenLC.Application.DTOs.Learning.CauHoi;
using MsHuyenLC.Domain.Enums;

namespace MsHuyenLC.Application.Validators.Learning;

public class CauHoiUpdateRequestValidator : AbstractValidator<CauHoiUpdateRequest>
{
    public CauHoiUpdateRequestValidator()
    {
        When(x => !string.IsNullOrWhiteSpace(x.NoiDungCauHoi), () =>
        {
            RuleFor(x => x.NoiDungCauHoi)
                .MaximumLength(2000).WithMessage("Nội dung câu hỏi không được quá 2000 ký tự");
        });

        When(x => x.KyNang.HasValue, () =>
        {
            RuleFor(x => x.KyNang!.Value)
                .IsInEnum().WithMessage("Kỹ năng không hợp lệ");

            When(x => x.KyNang != KyNang.Viet && x.CacDapAn != null, () =>
            {
                RuleFor(x => x.CacDapAn)
                    .Must(dapAn => dapAn!.Count >= 2).WithMessage("Câu hỏi phải có ít nhất 2 đáp án")
                    .Must(dapAn => dapAn!.Count(d => d.Dung) == 1).WithMessage("Phải có đúng 1 đáp án đúng");
            });
        });

        When(x => !string.IsNullOrWhiteSpace(x.UrlHinhAnh), () =>
        {
            RuleFor(x => x.UrlHinhAnh)
                .MaximumLength(500).WithMessage("URL hình ảnh không được quá 500 ký tự");
        });

        When(x => !string.IsNullOrWhiteSpace(x.UrlAmThanh), () =>
        {
            RuleFor(x => x.UrlAmThanh)
                .MaximumLength(500).WithMessage("URL âm thanh không được quá 500 ký tự");
        });

        When(x => !string.IsNullOrWhiteSpace(x.LoiThoai), () =>
        {
            RuleFor(x => x.LoiThoai)
                .MaximumLength(2000).WithMessage("Lời thoại không được quá 2000 ký tự");
        });

        When(x => x.CapDo.HasValue, () =>
        {
            RuleFor(x => x.CapDo!.Value)
                .IsInEnum().WithMessage("Cấp độ không hợp lệ");
        });

        When(x => x.DoKho.HasValue, () =>
        {
            RuleFor(x => x.DoKho!.Value)
                .IsInEnum().WithMessage("Độ khó không hợp lệ");
        });

        When(x => x.CacDapAn != null && x.CacDapAn.Count > 0, () =>
        {
            RuleForEach(x => x.CacDapAn)
                .SetValidator(new DapAnRequestValidator());
        });
    }
}
