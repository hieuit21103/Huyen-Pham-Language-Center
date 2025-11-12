using FluentValidation;
using MsHuyenLC.Application.DTOs.Learning.DangKyKhach;
using MsHuyenLC.Application.Interfaces;

namespace MsHuyenLC.Application.Validators.Learning;

public class DangKyKhachUpdateRequestValidator : AbstractValidator<DangKyKhachUpdateRequest>
{
    private readonly IUnitOfWork _unitOfWork;

    public DangKyKhachUpdateRequestValidator(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;

        When(x => !string.IsNullOrWhiteSpace(x.HoTen), () =>
        {
            RuleFor(x => x.HoTen)
                .MinimumLength(3).WithMessage("Họ tên phải có ít nhất 3 ký tự")
                .MaximumLength(100).WithMessage("Họ tên không được quá 100 ký tự")
                .Matches(@"^[\p{L}\s]+$").WithMessage("Họ tên chỉ được chứa chữ cái và khoảng trắng");
        });

        RuleFor(x => x.GioiTinh)
            .IsInEnum().WithMessage("Giới tính không hợp lệ");

        When(x => !string.IsNullOrWhiteSpace(x.Email), () =>
        {
            RuleFor(x => x.Email)
                .EmailAddress().WithMessage("Email không hợp lệ")
                .MaximumLength(100).WithMessage("Email không được quá 100 ký tự");
        });

        When(x => !string.IsNullOrWhiteSpace(x.SoDienThoai), () =>
        {
            RuleFor(x => x.SoDienThoai)
                .Matches(@"^(0[3|5|7|8|9])+([0-9]{8})$").WithMessage("Số điện thoại không hợp lệ");
        });

        When(x => !string.IsNullOrWhiteSpace(x.NoiDung), () =>
        {
            RuleFor(x => x.NoiDung)
                .MaximumLength(1000).WithMessage("Nội dung không được quá 1000 ký tự");
        });

        When(x => x.KhoaHocId.HasValue, () =>
        {
            RuleFor(x => x.KhoaHocId!.Value)
                .MustAsync(KhoaHocExists).WithMessage("Khóa học không tồn tại");
        });

        When(x => x.TrangThai.HasValue, () =>
        {
            RuleFor(x => x.TrangThai!.Value)
                .IsInEnum().WithMessage("Trạng thái không hợp lệ");
        });

        When(x => x.KetQua.HasValue, () =>
        {
            RuleFor(x => x.KetQua!.Value)
                .IsInEnum().WithMessage("Kết quả không hợp lệ");
        });
    }

    private async Task<bool> KhoaHocExists(Guid khoaHocId, CancellationToken cancellationToken)
    {
        return await _unitOfWork.KhoaHocs.ExistsAsync(x => x.Id == khoaHocId);
    }
}
