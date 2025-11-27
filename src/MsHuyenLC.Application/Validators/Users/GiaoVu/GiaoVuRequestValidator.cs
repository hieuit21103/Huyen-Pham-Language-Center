using FluentValidation;
using MsHuyenLC.Application.DTOs.Users.GiaoVu;
using MsHuyenLC.Application.Interfaces;

namespace MsHuyenLC.Application.Validators.Users;

public class GiaoVuRequestValidator : AbstractValidator<GiaoVuRequest>
{
    private readonly IUnitOfWork _unitOfWork;

    public GiaoVuRequestValidator(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;

        RuleFor(x => x.HoTen)
            .NotEmpty().WithMessage("Họ tên không được để trống")
            .Length(3, 100).WithMessage("Họ tên phải từ 3-100 ký tự")
            .Matches(@"^[\p{L}\s]+$").WithMessage("Họ tên chỉ được chứa chữ cái và khoảng trắng");

        RuleFor(x => x.BoPhan)
            .MaximumLength(100).WithMessage("Bộ phận không được vượt quá 100 ký tự")
            .When(x => !string.IsNullOrEmpty(x.BoPhan));

        RuleFor(x => x.TaiKhoanId)
            .NotEmpty().WithMessage("Tài khoản không được để trống")
            .MustAsync(TaiKhoanExists).WithMessage("Tài khoản không tồn tại")
            .MustAsync(IsUniqueTaiKhoanId).WithMessage("Tài khoản đã được liên kết với giáo vụ khác");
    }

    private async Task<bool> TaiKhoanExists(Guid taiKhoanId, CancellationToken cancellationToken)
    {
        return await _unitOfWork.TaiKhoans.ExistsAsync(t => t.Id == taiKhoanId);
    }
     
    private async Task<bool> IsUniqueTaiKhoanId(Guid taiKhoanId, CancellationToken cancellationToken)
    {
        var existingGiaoVu = await _unitOfWork.GiaoVus.GetAllAsync(gv => gv.TaiKhoanId == taiKhoanId);
        return existingGiaoVu == null || !existingGiaoVu.Any();
    }
}
