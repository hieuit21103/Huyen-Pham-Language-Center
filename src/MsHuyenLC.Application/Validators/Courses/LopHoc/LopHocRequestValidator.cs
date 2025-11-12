using FluentValidation;
using MsHuyenLC.Application.DTOs.Courses.LopHoc;
using MsHuyenLC.Application.Interfaces;

namespace MsHuyenLC.Application.Validators.Courses;

public class LopHocRequestValidator : AbstractValidator<LopHocRequest>
{
    private readonly IUnitOfWork _unitOfWork;

    public LopHocRequestValidator(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;

        RuleFor(x => x.TenLop)
            .NotEmpty().WithMessage("Tên lớp không được để trống")
            .Length(3, 100).WithMessage("Tên lớp phải từ 3-100 ký tự");

        RuleFor(x => x.KhoaHocId)
            .NotEmpty().WithMessage("Khóa học không được để trống")
            .MustAsync(KhoaHocExists).WithMessage("Khóa học không tồn tại");

        RuleFor(x => x.SiSoToiDa)
            .GreaterThan(0).WithMessage("Sĩ số tối đa phải lớn hơn 0")
            .LessThanOrEqualTo(200).WithMessage("Sĩ số tối đa không được vượt quá 200");
    }

    private async Task<bool> KhoaHocExists(Guid khoaHocId, CancellationToken cancellationToken)
    {
        return await _unitOfWork.KhoaHocs.ExistsAsync(k => k.Id == khoaHocId);
    }
}
