using Microsoft.AspNetCore.Identity;
using MsHuyenLC.Application.Interfaces.Auth;
using MsHuyenLC.Infrastructure.Persistence;

namespace MsHuyenLC.Infrastructure.Persistence.Seed;

public class DefaultUserSeeder(MsHuyenLC.Application.Interfaces.Auth.IPasswordHasher passwordHasher)
{
    protected readonly MsHuyenLC.Application.Interfaces.Auth.IPasswordHasher _passwordHasher = passwordHasher;
    public async Task SeedAsync(ApplicationDbContext context)
    {
        if (!context.TaiKhoans.Any())
        {
            context.TaiKhoans.Add(new TaiKhoan
            {
                TenDangNhap = "admin",
                MatKhau = _passwordHasher.HashPassword("Admin@123")
            });
        }

        await context.SaveChangesAsync();
    }
}
