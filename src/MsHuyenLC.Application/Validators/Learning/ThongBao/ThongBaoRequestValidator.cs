using FluentValidation;
using MsHuyenLC.Application.DTOs.Learning.ThongBao;
using MsHuyenLC.Application.Interfaces;

namespace MsHuyenLC.Application.Validators.Learning;

public class ThongBaoRequestValidator : AbstractValidator<ThongBaoRequest>
{
    private readonly IUnitOfWork _unitOfWork;

    public ThongBaoRequestValidator(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;

        RuleFor(x => x.NguoiGuiId)
            .NotEmpty().WithMessage("Người gửi không được để trống")
            .MustAsync(NguoiGuiExists).WithMessage("Người gửi không tồn tại");

        RuleFor(x => x.NguoiNhanId)
            .MustAsync(NguoiNhanExists).WithMessage("Người nhận không tồn tại")
            .When(x => x.NguoiNhanId.HasValue && x.NguoiNhanId != Guid.Empty);

        RuleFor(x => x.TieuDe)
            .NotEmpty().WithMessage("Tiêu đề không được để trống")
            .Length(3, 200).WithMessage("Tiêu đề phải từ 3-200 ký tự");

        RuleFor(x => x.NoiDung)
            .NotEmpty().WithMessage("Nội dung không được để trống")
            .MaximumLength(2000).WithMessage("Nội dung không được vượt quá 2000 ký tự");
    }

    private async Task<bool> NguoiGuiExists(Guid nguoiGuiId, CancellationToken cancellationToken)
    {
        return await _unitOfWork.TaiKhoans.ExistsAsync(t => t.Id == nguoiGuiId);
    }

    private async Task<bool> NguoiNhanExists(Guid? nguoiNhanId, CancellationToken cancellationToken)
    {
        if (!nguoiNhanId.HasValue || nguoiNhanId == Guid.Empty) return true;
        return await _unitOfWork.TaiKhoans.ExistsAsync(t => t.Id == nguoiNhanId.Value);
    }
}
