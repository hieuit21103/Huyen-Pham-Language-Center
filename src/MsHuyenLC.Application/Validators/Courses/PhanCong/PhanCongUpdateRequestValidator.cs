using FluentValidation;
using MsHuyenLC.Application.DTOs.Courses.PhanCong;
using MsHuyenLC.Application.Interfaces;

namespace MsHuyenLC.Application.Validators.Courses;

public class PhanCongUpdateRequestValidator : AbstractValidator<PhanCongUpdateRequest>
{
    private readonly IUnitOfWork _unitOfWork;

    public PhanCongUpdateRequestValidator(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;

        RuleFor(x => x.GiaoVienId)
            .MustAsync(GiaoVienExists).WithMessage("Giáo viên không tồn tại");

        RuleFor(x => x.LopHocId)
            .MustAsync(LopHocExists).WithMessage("Lớp học không tồn tại");

        RuleFor(x => x)
            .MustAsync(NotAlreadyAssigned).WithMessage("Giáo viên đã được phân công cho lớp học này rồi");
    }

    private async Task<bool> GiaoVienExists(Guid? giaoVienId, CancellationToken cancellationToken)
    {
        return await _unitOfWork.GiaoViens.ExistsAsync(g => g.Id == giaoVienId);
    }

    private async Task<bool> LopHocExists(Guid? lopHocId, CancellationToken cancellationToken)
    {
        return await _unitOfWork.LopHocs.ExistsAsync(l => l.Id == lopHocId);
    }

    private async Task<bool> NotAlreadyAssigned(PhanCongUpdateRequest request, CancellationToken cancellationToken)
    {
        return !await _unitOfWork.PhanCongs.ExistsAsync(p => 
            p.GiaoVienId == request.GiaoVienId && p.LopHocId == request.LopHocId);
    }
}
