using FluentValidation;
using MsHuyenLC.Application.DTOs.Learning.ThongBao;
using MsHuyenLC.Application.Interfaces;
using MsHuyenLC.Application.Interfaces.Services.Learning;
using MsHuyenLC.Domain.Entities.Learning;

namespace MsHuyenLC.Application.Services.Learning;

public class NotificationService : INotificationService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IValidator<ThongBaoRequest> _validator;

    public NotificationService(
        IUnitOfWork unitOfWork,
        IValidator<ThongBaoRequest> validator)
    {
        _unitOfWork = unitOfWork;
        _validator = validator;
    }

    public async Task<ThongBao?> GetByIdAsync(string id)
    {
        return await _unitOfWork.ThongBaos.GetByIdAsync(id);
    }

    public async Task<IEnumerable<ThongBao>> GetAllAsync()
    {
        return await _unitOfWork.ThongBaos.GetAllAsync();
    }

    public async Task<ThongBao> CreateAsync(ThongBaoRequest request)
    {
        await _validator.ValidateAndThrowAsync(request);

        var thongBao = new ThongBao
        {
            TieuDe = request.TieuDe,
            NoiDung = request.NoiDung,
            NguoiGuiId = request.NguoiGuiId,
            NguoiNhanId = request.NguoiNhanId,
            NgayTao = DateTime.UtcNow
        };

        var result = await _unitOfWork.ThongBaos.AddAsync(thongBao);
        await _unitOfWork.SaveChangesAsync();
        return result;
    }

    public async Task<bool> DeleteAsync(string id)
    {
        var thongBao = await _unitOfWork.ThongBaos.GetByIdAsync(id);
        if (thongBao == null)
            return false;

        await _unitOfWork.ThongBaos.DeleteAsync(thongBao);
        await _unitOfWork.SaveChangesAsync();
        return true;
    }

    public async Task<IEnumerable<ThongBao>> GetBySenderIdAsync(string senderId)
    {
        if (!Guid.TryParse(senderId, out var id))
            return Enumerable.Empty<ThongBao>();

        return await _unitOfWork.ThongBaos.GetAllAsync(t => t.NguoiGuiId == id);
    }

    public async Task<IEnumerable<ThongBao>> GetByReceiverIdAsync(string receiverId)
    {
        if (!Guid.TryParse(receiverId, out var id))
            return Enumerable.Empty<ThongBao>();

        return await _unitOfWork.ThongBaos.GetAllAsync(t => t.NguoiNhanId == id);
    }

    public async Task<int> CountAsync()
    {
        return await _unitOfWork.ThongBaos.CountAsync();
    }
}
