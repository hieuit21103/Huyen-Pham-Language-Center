using MsHuyenLC.Application.DTOs.System.CauHinhHeThong;
using MsHuyenLC.Domain.Entities.System;

namespace MsHuyenLC.Application.Interfaces.Services.System;

public interface ISystemConfigService
{
    Task<CauHinhHeThong?> GetByIdAsync(string id);
    Task<IEnumerable<CauHinhHeThong>> GetAllAsync();
    Task<CauHinhHeThong> CreateAsync(CauHinhHeThongRequest request);
    Task<CauHinhHeThong?> UpdateAsync(string id, CauHinhHeThongUpdateRequest request);
    Task<bool> DeleteAsync(string id);
    Task<CauHinhHeThong?> GetByNameAsync(string name);
    Task<int> CountAsync();
}
