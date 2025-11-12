using FluentValidation;
using MsHuyenLC.Application.DTOs.Learning.PhanHoi;
using MsHuyenLC.Application.Interfaces;
using MsHuyenLC.Application.Interfaces.Services.Learning;
using MsHuyenLC.Domain.Entities.Learning;

namespace MsHuyenLC.Application.Services.Learning;

public class FeedbackService : IFeedbackService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IValidator<PhanHoiRequest> _validator;

    public FeedbackService(
        IUnitOfWork unitOfWork,
        IValidator<PhanHoiRequest> validator)
    {
        _unitOfWork = unitOfWork;
        _validator = validator;
    }

    public async Task<PhanHoi?> GetByIdAsync(string id)
    {
        return await _unitOfWork.PhanHois.GetByIdAsync(id);
    }

    public async Task<IEnumerable<PhanHoi>> GetAllAsync()
    {
        return await _unitOfWork.PhanHois.GetAllAsync();
    }

    public async Task<PhanHoi> CreateAsync(PhanHoiRequest request)
    {
        await _validator.ValidateAndThrowAsync(request);

        var phanHoi = new PhanHoi
        {
            LoaiPhanHoi = request.LoaiPhanHoi,
            TieuDe = request.TieuDe,
            NoiDung = request.NoiDung,
            HocVienId = request.HocVienId,
            NgayTao = DateOnly.FromDateTime(DateTime.UtcNow)
        };

        var result = await _unitOfWork.PhanHois.AddAsync(phanHoi);
        await _unitOfWork.SaveChangesAsync();
        return result;
    }

    public async Task<bool> DeleteAsync(string id)
    {
        var phanHoi = await _unitOfWork.PhanHois.GetByIdAsync(id);
        if (phanHoi == null)
            return false;

        await _unitOfWork.PhanHois.DeleteAsync(phanHoi);
        await _unitOfWork.SaveChangesAsync();
        return true;
    }

    public async Task<IEnumerable<PhanHoi>> GetByStudentIdAsync(string studentId)
    {
        if (!Guid.TryParse(studentId, out var id))
            return Enumerable.Empty<PhanHoi>();

        return await _unitOfWork.PhanHois.GetAllAsync(p => p.HocVienId == id);
    }

    public async Task<int> CountAsync()
    {
        return await _unitOfWork.PhanHois.CountAsync();
    }
}
