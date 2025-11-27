using FluentValidation;
using MsHuyenLC.Application.DTOs.Users.GiaoVu;
using MsHuyenLC.Application.Interfaces;

namespace MsHuyenLC.Application.Validators.Users;

public class GiaoVuUpdateRequestValidator : AbstractValidator<GiaoVuUpdateRequest>
{
    private readonly IUnitOfWork _unitOfWork;

    public GiaoVuUpdateRequestValidator(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;

        RuleFor(x => x.HoTen)
            .Length(3, 100).WithMessage("Họ tên phải từ 3-100 ký tự")
            .Matches(@"^[\p{L}\s]+$").WithMessage("Họ tên chỉ được chứa chữ cái và khoảng trắng")
            .When(x => !string.IsNullOrEmpty(x.HoTen));

        RuleFor(x => x.BoPhan)
            .MaximumLength(100).WithMessage("Bộ phận không được vượt quá 100 ký tự")
            .When(x => !string.IsNullOrEmpty(x.BoPhan));
    }

}
