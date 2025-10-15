using MsHuyenLC.Infrastructure.Persistence;
using MsHuyenLC.Application.Interfaces.Auth;
using System.Linq.Expressions;

namespace MsHuyenLC.Infrastructure.Repositories;

public class UserRepository : IUserRepository
{
    private readonly ApplicationDbContext _context;

    public UserRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<TaiKhoan>> GetAllAsync(
        int PageNumber = 1,
        int PageSize = 10,
        Expression<Func<TaiKhoan, bool>>? Filter = null,
        Func<IQueryable<TaiKhoan>, IOrderedQueryable<TaiKhoan>>? OrderBy = null,
        params Expression<Func<TaiKhoan, object>>[] Includes)
    {
        IQueryable<TaiKhoan> query = _context.TaiKhoans;

        if (Filter != null)
        {
            query = query.Where(Filter);
        }

        foreach (var include in Includes)
        {
            query = query.Include(include);
        }

        if (OrderBy != null)
        {
            query = OrderBy(query);
        }

        return await query.Skip((PageNumber - 1) * PageSize).Take(PageSize).ToListAsync();
    }

    public async Task<TaiKhoan?> GetByUsernameAsync(string username)
    {
        return await _context.TaiKhoans.FirstOrDefaultAsync(u => u.TenDangNhap == username);
    }

    public async Task<TaiKhoan?> GetByEmailAsync(string email)
    {
        return await _context.TaiKhoans.FirstOrDefaultAsync(u => u.Email == email);
    }

    public async Task<TaiKhoan?> GetByIdAsync(string userId)
    {
        return await _context.TaiKhoans.FindAsync(Guid.Parse(userId));
    }

    public async Task<TaiKhoan> CreateAsync(TaiKhoan user)
    {
        await _context.TaiKhoans.AddAsync(user);
        return user;
    }

    public async Task UpdateAsync(TaiKhoan user)
    {
        _context.TaiKhoans.Update(user);
    }

    public async Task DeleteAsync(string userId)
    {
        var user = await GetByIdAsync(userId);
        if (user != null)
        {
            _context.TaiKhoans.Remove(user);
        }
    }

    public async Task<bool> UserExistsAsync(string username, string email)
    {
        return await _context.TaiKhoans.AnyAsync(u => u.TenDangNhap == username || u.Email == email);
    }

    public async Task AssignRoleAsync(string userId, VaiTro role)
    {
        var user = await GetByIdAsync(userId);
        if (user != null)
        {
            user.VaiTro = role;
            await UpdateAsync(user);
        }
    }

    public async Task<IEnumerable<string>> GetUserRolesAsync(string userId)
    {
        var user = await GetByIdAsync(userId);
        if (user != null)
        {
            return new List<string> { user.VaiTro.ToString() };
        }
        return Enumerable.Empty<string>();
    }

    public async Task SaveChangesAsync()
    {
        await _context.SaveChangesAsync();
    }

}