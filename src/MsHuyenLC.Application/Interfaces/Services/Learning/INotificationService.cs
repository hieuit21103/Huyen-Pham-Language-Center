using MsHuyenLC.Application.DTOs.Learning.ThongBao;
using MsHuyenLC.Domain.Entities.Learning;

namespace MsHuyenLC.Application.Interfaces.Services.Learning;

public interface INotificationService
{
    Task<ThongBao?> GetByIdAsync(string id);
    Task<IEnumerable<ThongBao>> GetAllAsync();
    Task<ThongBao> CreateAsync(ThongBaoRequest request);
    Task<bool> DeleteAsync(string id);
    Task<IEnumerable<ThongBao>> GetBySenderIdAsync(string senderId);
    Task<IEnumerable<ThongBao>> GetByReceiverIdAsync(string receiverId);
    Task<int> CountAsync();
}
