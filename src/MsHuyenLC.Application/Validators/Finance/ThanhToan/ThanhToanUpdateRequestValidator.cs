using FluentValidation;
using MsHuyenLC.Application.DTOs.Finance.ThanhToan;

namespace MsHuyenLC.Application.Validators.Finance;

public class ThanhToanUpdateRequestValidator : AbstractValidator<ThanhToanUpdateRequest>
{
    public ThanhToanUpdateRequestValidator()
    {
        RuleFor(x => x.TrangThai)
            .IsInEnum().WithMessage("Trạng thái thanh toán không hợp lệ");

        When(x => !string.IsNullOrWhiteSpace(x.MaThanhToan), () =>
        {
            RuleFor(x => x.MaThanhToan)
                .MaximumLength(100).WithMessage("Mã thanh toán không được quá 100 ký tự");
        });

        When(x => !string.IsNullOrWhiteSpace(x.GhiChu), () =>
        {
            RuleFor(x => x.GhiChu)
                .MaximumLength(500).WithMessage("Ghi chú không được quá 500 ký tự");
        });

        When(x => !string.IsNullOrWhiteSpace(x.ThongTinNganHang), () =>
        {
            RuleFor(x => x.ThongTinNganHang)
                .MaximumLength(200).WithMessage("Thông tin ngân hàng không được quá 200 ký tự");
        });

        When(x => !string.IsNullOrWhiteSpace(x.MaGiaoDichNganHang), () =>
        {
            RuleFor(x => x.MaGiaoDichNganHang)
                .MaximumLength(100).WithMessage("Mã giao dịch ngân hàng không được quá 100 ký tự");
        });

        When(x => !string.IsNullOrWhiteSpace(x.CongThanhToan), () =>
        {
            RuleFor(x => x.CongThanhToan)
                .MaximumLength(50).WithMessage("Cổng thanh toán không được quá 50 ký tự");
        });
    }
}
