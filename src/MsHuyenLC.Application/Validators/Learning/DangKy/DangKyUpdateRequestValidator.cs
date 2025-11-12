using FluentValidation;
using MsHuyenLC.Application.DTOs.Learning.DangKy;
using MsHuyenLC.Application.Interfaces;

namespace MsHuyenLC.Application.Validators.Learning;

public class DangKyUpdateRequestValidator : AbstractValidator<DangKyUpdateRequest>
{
    private readonly IUnitOfWork _unitOfWork;

    public DangKyUpdateRequestValidator(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;

        When(x => x.KhoaHocId.HasValue, () =>
        {
            RuleFor(x => x.KhoaHocId!.Value)
                .MustAsync(KhoaHocExists).WithMessage("Khóa học không tồn tại");
        });

        When(x => x.HocVienId.HasValue, () =>
        {
            RuleFor(x => x.HocVienId!.Value)
                .MustAsync(HocVienExists).WithMessage("Học viên không tồn tại");
        });

        When(x => x.LopHocId.HasValue, () =>
        {
            RuleFor(x => x.LopHocId!.Value)
                .MustAsync(LopHocExists).WithMessage("Lớp học không tồn tại");
        });

        When(x => x.NgayXepLop.HasValue && x.NgayDangKy.HasValue, () =>
        {
            RuleFor(x => x.NgayXepLop!.Value)
                .Must((model, ngayXepLop) => ngayXepLop >= model.NgayDangKy!.Value)
                .WithMessage("Ngày xếp lớp phải sau hoặc bằng ngày đăng ký");
        });

        When(x => x.TrangThai.HasValue, () =>
        {
            RuleFor(x => x.TrangThai!.Value)
                .IsInEnum().WithMessage("Trạng thái không hợp lệ");
        });
    }

    private async Task<bool> KhoaHocExists(Guid khoaHocId, CancellationToken cancellationToken)
    {
        return await _unitOfWork.KhoaHocs.ExistsAsync(x => x.Id == khoaHocId);
    }

    private async Task<bool> HocVienExists(Guid hocVienId, CancellationToken cancellationToken)
    {
        return await _unitOfWork.HocViens.ExistsAsync(x => x.Id == hocVienId);
    }

    private async Task<bool> LopHocExists(Guid lopHocId, CancellationToken cancellationToken)
    {
        return await _unitOfWork.LopHocs.ExistsAsync(x => x.Id == lopHocId);
    }
}
