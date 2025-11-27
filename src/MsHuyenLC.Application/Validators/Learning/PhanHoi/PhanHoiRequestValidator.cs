using FluentValidation;
using MsHuyenLC.Application.DTOs.Learning.PhanHoi;
using MsHuyenLC.Application.Interfaces;

namespace MsHuyenLC.Application.Validators.Learning;

public class PhanHoiRequestValidator : AbstractValidator<PhanHoiRequest>
{
    private readonly IUnitOfWork _unitOfWork;

    public PhanHoiRequestValidator(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;

        RuleFor(x => x.HocVienId)
            .NotEmpty().WithMessage("Học viên không được để trống")
            .MustAsync(HocVienExists).WithMessage("Học viên không tồn tại");

        RuleFor(x => x.LoaiPhanHoi)
            .NotEmpty().WithMessage("Loại phản hồi không được để trống")
            .MaximumLength(50).WithMessage("Loại phản hồi không được vượt quá 50 ký tự");

        RuleFor(x => x.TieuDe)
            .NotEmpty().WithMessage("Tiêu đề không được để trống")
            .Length(3, 200).WithMessage("Tiêu đề phải từ 3-200 ký tự");

        RuleFor(x => x.NoiDung)
            .NotEmpty().WithMessage("Nội dung không được để trống")
            .MaximumLength(2000).WithMessage("Nội dung không được vượt quá 2000 ký tự");
    }

    private async Task<bool> HocVienExists(Guid hocVienId, CancellationToken cancellationToken)
    {
        return await _unitOfWork.HocViens.ExistsAsync(h => h.Id == hocVienId);
    }
}
