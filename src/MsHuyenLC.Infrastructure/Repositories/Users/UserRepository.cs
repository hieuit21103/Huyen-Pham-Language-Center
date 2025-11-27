using MsHuyenLC.Application.Interfaces.Repositories.Users;
using MsHuyenLC.Infrastructure.Persistence;

namespace MsHuyenLC.Infrastructure.Repositories.Users;
public class UserRepository : GenericRepository<TaiKhoan>, IUserRepository
{
    private readonly DbSet<TaiKhoan> _dbSet;
    public UserRepository(ApplicationDbContext context) : base(context)
    {
        _dbSet = context.TaiKhoans;
    }

    public async Task<TaiKhoan?> GetByUsernameAsync(string username)
    {
        return await _dbSet.FirstOrDefaultAsync(u => u.TenDangNhap == username);
    }

    public async Task<TaiKhoan?> GetByEmailAsync(string email)
    {
        return await _dbSet.FirstOrDefaultAsync(u => u.Email == email);
    }

    public async Task<bool> IsUsernameExistsAsync(string username)
    {
        return await _dbSet.AnyAsync(u => u.TenDangNhap == username);
    }

    public async Task<bool> IsEmailExistsAsync(string email)
    {
        return await _dbSet.AnyAsync(u => u.Email == email);
    }
}