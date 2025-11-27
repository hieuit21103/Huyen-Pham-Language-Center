using MsHuyenLC.Domain.Entities.Users;

namespace MsHuyenLC.Application.Interfaces.Repositories.Users;

public interface IUserRepository : IGenericRepository<TaiKhoan>
{
    Task<TaiKhoan?> GetByUsernameAsync(string username);
    Task<TaiKhoan?> GetByEmailAsync(string email);
    Task<bool> IsUsernameExistsAsync(string username);
    Task<bool> IsEmailExistsAsync(string email);
}