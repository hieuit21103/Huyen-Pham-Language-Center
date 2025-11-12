using FluentValidation;
using MsHuyenLC.Application.DTOs.Finance.VNPay;

namespace MsHuyenLC.Application.Validators.Finance;

public class VNPayCallbackRequestValidator : AbstractValidator<VNPayCallbackRequest>
{
    public VNPayCallbackRequestValidator()
    {
        RuleFor(x => x.vnp_TmnCode)
            .NotEmpty().WithMessage("Mã terminal không được để trống");

        RuleFor(x => x.vnp_Amount)
            .NotEmpty().WithMessage("Số tiền không được để trống")
            .Must(BeValidAmount).WithMessage("Số tiền không hợp lệ");

        RuleFor(x => x.vnp_BankCode)
            .NotEmpty().WithMessage("Mã ngân hàng không được để trống");

        RuleFor(x => x.vnp_PayDate)
            .NotEmpty().WithMessage("Ngày thanh toán không được để trống")
            .Must(BeValidPayDate).WithMessage("Ngày thanh toán không hợp lệ");

        RuleFor(x => x.vnp_OrderInfo)
            .NotEmpty().WithMessage("Thông tin đơn hàng không được để trống");

        RuleFor(x => x.vnp_ResponseCode)
            .NotEmpty().WithMessage("Mã phản hồi không được để trống");

        RuleFor(x => x.vnp_TransactionStatus)
            .NotEmpty().WithMessage("Trạng thái giao dịch không được để trống");

        RuleFor(x => x.vnp_TxnRef)
            .NotEmpty().WithMessage("Mã tham chiếu không được để trống");

        RuleFor(x => x.vnp_SecureHash)
            .NotEmpty().WithMessage("Chữ ký bảo mật không được để trống");
    }

    private bool BeValidAmount(string amount)
    {
        return long.TryParse(amount, out var value) && value > 0;
    }

    private bool BeValidPayDate(string payDate)
    {
        return DateTime.TryParseExact(payDate, "yyyyMMddHHmmss", null, 
            global::System.Globalization.DateTimeStyles.None, out _);
    }
}
