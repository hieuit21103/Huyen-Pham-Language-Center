using FluentValidation;
using MsHuyenLC.Application.DTOs.Finance.VNPay;

namespace MsHuyenLC.Application.Validators.Finance;

public class VNPayCreatePaymentRequestValidator : AbstractValidator<VNPayCreatePaymentRequest>
{
    public VNPayCreatePaymentRequestValidator()
    {
        RuleFor(x => x.OrderId)
            .NotEmpty().WithMessage("Mã đơn hàng không được để trống");

        RuleFor(x => x.Amount)
            .GreaterThan(0).WithMessage("Số tiền phải lớn hơn 0")
            .LessThanOrEqualTo(1000000000).WithMessage("Số tiền không được vượt quá 1 tỷ");

        RuleFor(x => x.OrderInfo)
            .NotEmpty().WithMessage("Thông tin đơn hàng không được để trống")
            .MaximumLength(200).WithMessage("Thông tin đơn hàng không được quá 200 ký tự");

        RuleFor(x => x.ReturnUrl)
            .NotEmpty().WithMessage("URL trả về không được để trống")
            .Must(BeValidUrl).WithMessage("URL trả về không hợp lệ");

        When(x => !string.IsNullOrWhiteSpace(x.IpAddress), () =>
        {
            RuleFor(x => x.IpAddress)
                .Matches(@"^(\d{1,3}\.){3}\d{1,3}$").WithMessage("Địa chỉ IP không hợp lệ");
        });

        When(x => !string.IsNullOrWhiteSpace(x.BankCode), () =>
        {
            RuleFor(x => x.BankCode)
                .MaximumLength(20).WithMessage("Mã ngân hàng không được quá 20 ký tự");
        });
    }

    private bool BeValidUrl(string url)
    {
        return Uri.TryCreate(url, UriKind.Absolute, out var uriResult)
            && (uriResult.Scheme == Uri.UriSchemeHttp || uriResult.Scheme == Uri.UriSchemeHttps);
    }
}
