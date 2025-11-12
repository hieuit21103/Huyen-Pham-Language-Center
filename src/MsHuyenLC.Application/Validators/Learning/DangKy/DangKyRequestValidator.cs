using FluentValidation;
using MsHuyenLC.Application.DTOs.Learning.DangKy;
using MsHuyenLC.Application.Interfaces;

namespace MsHuyenLC.Application.Validators.Learning;

public class DangKyRequestValidator : AbstractValidator<DangKyRequest>
{
    private readonly IUnitOfWork _unitOfWork;

    public DangKyRequestValidator(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;

        RuleFor(x => x.HocVienId)
            .NotEmpty().WithMessage("Học viên không được để trống")
            .MustAsync(HocVienExists).WithMessage("Học viên không tồn tại");

        RuleFor(x => x.KhoaHocId)
            .NotEmpty().WithMessage("Khóa học không được để trống")
            .MustAsync(KhoaHocExists).WithMessage("Khóa học không tồn tại");

        RuleFor(x => x)
            .MustAsync(NotAlreadyRegistered).WithMessage("Học viên đã đăng ký khóa học này rồi");
    }

    private async Task<bool> HocVienExists(Guid hocVienId, CancellationToken cancellationToken)
    {
        return await _unitOfWork.HocViens.ExistsAsync(h => h.Id == hocVienId);
    }

    private async Task<bool> KhoaHocExists(Guid khoaHocId, CancellationToken cancellationToken)
    {
        return await _unitOfWork.KhoaHocs.ExistsAsync(k => k.Id == khoaHocId);
    }

    private async Task<bool> NotAlreadyRegistered(DangKyRequest request, CancellationToken cancellationToken)
    {
        return !await _unitOfWork.DangKys.ExistsAsync(d => 
            d.HocVienId == request.HocVienId && d.KhoaHocId == request.KhoaHocId);
    }
}
