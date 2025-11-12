using FluentValidation;
using MsHuyenLC.Application.DTOs.Learning.DangKy;
using MsHuyenLC.Application.Interfaces;

namespace MsHuyenLC.Application.Validators.Learning;

public class DangKyCreateRequestValidator : AbstractValidator<DangKyCreateRequest>
{
    private readonly IUnitOfWork _unitOfWork;

    public DangKyCreateRequestValidator(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;

        RuleFor(x => x.KhoaHocId)
            .NotEmpty().WithMessage("Khóa học không được để trống")
            .MustAsync(KhoaHocExists).WithMessage("Khóa học không tồn tại");

        RuleFor(x => x.HocVienId)
            .NotEmpty().WithMessage("Học viên không được để trống")
            .MustAsync(HocVienExists).WithMessage("Học viên không tồn tại")
            .MustAsync(NotAlreadyRegistered).WithMessage("Học viên đã đăng ký khóa học này");

        RuleFor(x => x.TrangThai)
            .IsInEnum().WithMessage("Trạng thái không hợp lệ");

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

    private async Task<bool> NotAlreadyRegistered(DangKyCreateRequest request, Guid hocVienId, CancellationToken cancellationToken)
    {
        var exists = await _unitOfWork.DangKys.ExistsAsync(x => x.HocVienId == hocVienId && x.KhoaHocId == request.KhoaHocId);
        return !exists;
    }
}
