using FluentValidation;
using MsHuyenLC.Application.DTOs.Users.GiaoVien;

namespace MsHuyenLC.Application.Validators.Users;

public class GiaoVienUpdateRequestValidator : AbstractValidator<GiaoVienUpdateRequest>
{
    public GiaoVienUpdateRequestValidator()
    {
        RuleFor(x => x.HoTen)
            .NotEmpty().WithMessage("Họ tên không được để trống")
            .MaximumLength(100).WithMessage("Họ tên không được quá 100 ký tự")
            .Matches(@"^[\p{L}\s]+$").WithMessage("Họ tên chỉ được chứa chữ cái và khoảng trắng");

        When(x => !string.IsNullOrWhiteSpace(x.ChuyenMon), () =>
        {
            RuleFor(x => x.ChuyenMon)
                .MaximumLength(200).WithMessage("Chuyên môn không được quá 200 ký tự");
        });

        When(x => !string.IsNullOrWhiteSpace(x.TrinhDo), () =>
        {
            RuleFor(x => x.TrinhDo)
                .MaximumLength(100).WithMessage("Trình độ không được quá 100 ký tự");
        });

        When(x => !string.IsNullOrWhiteSpace(x.KinhNghiem), () =>
        {
            RuleFor(x => x.KinhNghiem)
                .MaximumLength(500).WithMessage("Kinh nghiệm không được quá 500 ký tự");
        });
    }
}
