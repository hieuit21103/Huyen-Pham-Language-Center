using FluentValidation;
using MsHuyenLC.Application.DTOs.Users.GiaoVien;
using MsHuyenLC.Application.Interfaces;
using MsHuyenLC.Domain.Enums;

namespace MsHuyenLC.Application.Validators.Users;

public class GiaoVienRequestValidator : AbstractValidator<GiaoVienRequest>
{
    private readonly IUnitOfWork _unitOfWork;

    public GiaoVienRequestValidator(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;

        RuleFor(x => x.HoTen)
            .NotEmpty().WithMessage("Họ tên không được để trống")
            .Length(3, 100).WithMessage("Họ tên phải từ 3-100 ký tự")
            .Matches(@"^[\p{L}\s]+$").WithMessage("Họ tên chỉ được chứa chữ cái và khoảng trắng");

        RuleFor(x => x.ChuyenMon)
            .MaximumLength(200).WithMessage("Chuyên môn không được vượt quá 200 ký tự")
            .When(x => !string.IsNullOrEmpty(x.ChuyenMon));

        RuleFor(x => x.TrinhDo)
            .MaximumLength(100).WithMessage("Trình độ không được vượt quá 100 ký tự")
            .When(x => !string.IsNullOrEmpty(x.TrinhDo));

        RuleFor(x => x.KinhNghiem)
            .MaximumLength(500).WithMessage("Kinh nghiệm không được vượt quá 500 ký tự")
            .When(x => !string.IsNullOrEmpty(x.KinhNghiem));

        RuleFor(x => x.TaiKhoanId)
            .NotEmpty().WithMessage("Tài khoản không được để trống")
            .MustAsync(TaiKhoanExists).WithMessage("Tài khoản không tồn tại")
            .MustAsync(IsUniqueTaiKhoanId).WithMessage("Tài khoản đã được liên kết với giáo viên khác")
            .MustAsync(IsTeacherAccount).WithMessage("Tài khoản không có vai trò giáo viên");
    }

    private async Task<bool> TaiKhoanExists(Guid taiKhoanId, CancellationToken cancellationToken)
    {
        return await _unitOfWork.TaiKhoans.ExistsAsync(t => t.Id == taiKhoanId);
    }

    private async Task<bool> IsUniqueTaiKhoanId(Guid taiKhoanId, CancellationToken cancellationToken)
    {
        var existingGiaoVien = await _unitOfWork.GiaoViens.GetAllAsync(gv => gv.TaiKhoanId == taiKhoanId);
        return existingGiaoVien == null || !existingGiaoVien.Any();
    }

    private async Task<bool> IsTeacherAccount(Guid taiKhoanId, CancellationToken cancellationToken)
    {
        var taiKhoan = await _unitOfWork.TaiKhoans.GetByIdAsync(taiKhoanId.ToString());
        return taiKhoan != null && taiKhoan.VaiTro == VaiTro.giaovien;
    }
}
