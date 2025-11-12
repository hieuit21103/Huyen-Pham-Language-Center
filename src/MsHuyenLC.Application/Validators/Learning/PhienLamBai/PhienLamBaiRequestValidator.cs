using FluentValidation;
using MsHuyenLC.Application.DTOs.Learning.PhienLamBai;
using MsHuyenLC.Application.Interfaces;
using MsHuyenLC.Domain.Entities.Learning.OnlineExam;

namespace MsHuyenLC.Application.Validators.Learning;

public class PhienLamBaiRequestValidator : AbstractValidator<PhienLamBaiRequest>
{
    private readonly IUnitOfWork _unitOfWork;

    public PhienLamBaiRequestValidator(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;

        RuleFor(x => x.DeThiId)
            .NotEmpty().WithMessage("Đề thi không được để trống")
            .MustAsync(DeThiExists).WithMessage("Đề thi không tồn tại");

        RuleFor(x => x.HocVienId)
            .NotEmpty().WithMessage("Học viên không được để trống")
            .MustAsync(HocVienExists).WithMessage("Học viên không tồn tại");
    }

    public async Task<bool> DeThiExists(string deThiId, CancellationToken cancellationToken)
    {
        return await _unitOfWork.DeThis.ExistsAsync(x => x.Id.ToString() == deThiId);
    }

    public async Task<bool> HocVienExists(string hocVienId, CancellationToken cancellationToken)
    {
        return await _unitOfWork.HocViens.ExistsAsync(x => x.Id.ToString() == hocVienId);
    }
}
