using FluentValidation;
using MsHuyenLC.Application.DTOs.Learning.DangKyTuVan;
using MsHuyenLC.Application.Interfaces;

namespace MsHuyenLC.Application.Validators.Learning;

public class DangKyTuVanCreateRequestValidator : AbstractValidator<DangKyTuVanCreateRequest>
{
    private readonly IUnitOfWork _unitOfWork;

    public DangKyTuVanCreateRequestValidator(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;

        RuleFor(x => x.HoTen)
            .NotEmpty().WithMessage("Họ tên không được để trống")
            .MinimumLength(3).WithMessage("Họ tên phải có ít nhất 3 ký tự")
            .MaximumLength(100).WithMessage("Họ tên không được quá 100 ký tự")
            .Matches(@"^[\p{L}\s]+$").WithMessage("Họ tên chỉ được chứa chữ cái và khoảng trắng");

        RuleFor(x => x.GioiTinh)
            .IsInEnum().WithMessage("Giới tính không hợp lệ");

        RuleFor(x => x.Email)
            .NotEmpty().WithMessage("Email không được để trống")
            .EmailAddress().WithMessage("Email không hợp lệ")
            .MaximumLength(100).WithMessage("Email không được quá 100 ký tự");

        RuleFor(x => x.SoDienThoai)
            .NotEmpty().WithMessage("Số điện thoại không được để trống")
            .Matches(@"^(0[3|5|7|8|9])+([0-9]{8})$").WithMessage("Số điện thoại không hợp lệ");

        When(x => !string.IsNullOrWhiteSpace(x.NoiDung), () =>
        {
            RuleFor(x => x.NoiDung)
                .MaximumLength(1000).WithMessage("Nội dung không được quá 1000 ký tự");
        });

        RuleFor(x => x.KhoaHocId)
            .NotEmpty().WithMessage("Khóa học không được để trống")
            .MustAsync(KhoaHocExists).WithMessage("Khóa học không tồn tại");
    }

    private async Task<bool> KhoaHocExists(Guid khoaHocId, CancellationToken cancellationToken)
    {
        return await _unitOfWork.KhoaHocs.ExistsAsync(x => x.Id == khoaHocId);
    }
}
