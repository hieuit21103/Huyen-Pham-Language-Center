using FluentValidation;
using MsHuyenLC.Application.DTOs.Auth;

namespace MsHuyenLC.Application.Validators.Auth;

public class ResetPasswordRequestValidator : AbstractValidator<ResetPasswordRequest>
{
    public ResetPasswordRequestValidator()
    {
        RuleFor(x => x.Email)
            .NotEmpty().WithMessage("Email không được để trống")
            .EmailAddress().WithMessage("Email không hợp lệ");

        RuleFor(x => x.ReturnUrl)
            .NotEmpty().WithMessage("Return URL không được để trống")
            .Must(BeValidUrl).WithMessage("Return URL không hợp lệ");
    }

    private bool BeValidUrl(string url)
    {
        return Uri.TryCreate(url, UriKind.Absolute, out _);
    }
}
