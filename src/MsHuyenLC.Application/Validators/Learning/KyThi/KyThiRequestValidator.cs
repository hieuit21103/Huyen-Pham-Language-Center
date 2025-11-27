using FluentValidation;
using MsHuyenLC.Application.DTOs.Learning.KyThi;
using MsHuyenLC.Application.Interfaces;

namespace MsHuyenLC.Application.Validators.Learning;

public class KyThiRequestValidator : AbstractValidator<KyThiRequest>
{
    private readonly IUnitOfWork _unitOfWork;

    public KyThiRequestValidator(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;

        RuleFor(x => x.TenKyThi)
            .NotEmpty().WithMessage("Tên kỳ thi không được để trống")
            .Length(3, 200).WithMessage("Tên kỳ thi phải từ 3-200 ký tự");

        RuleFor(x => x.NgayThi)
            .GreaterThanOrEqualTo(DateOnly.FromDateTime(DateTime.Now.AddDays(-1)))
            .WithMessage("Ngày thi phải từ ngày hiện tại trở đi");

        RuleFor(x => x.GioBatDau)
            .LessThan(x => x.GioKetThuc).WithMessage("Giờ bắt đầu phải nhỏ hơn giờ kết thúc");

        RuleFor(x => x.GioKetThuc)
            .GreaterThan(x => x.GioBatDau).WithMessage("Giờ kết thúc phải lớn hơn giờ bắt đầu");

        RuleFor(x => x.ThoiLuong)
            .GreaterThan(0).WithMessage("Thời lượng phải lớn hơn 0")
            .LessThanOrEqualTo(300).WithMessage("Thời lượng không được vượt quá 300 phút");

        RuleFor(x => x.LopHocId)
            .NotEmpty().WithMessage("Lớp học không được để trống")
            .MustAsync(LopHocExists).WithMessage("Lớp học không tồn tại");
    }

    private async Task<bool> LopHocExists(Guid lopHocId, CancellationToken cancellationToken)
    {
        return await _unitOfWork.LopHocs.ExistsAsync(l => l.Id == lopHocId);
    }
}
