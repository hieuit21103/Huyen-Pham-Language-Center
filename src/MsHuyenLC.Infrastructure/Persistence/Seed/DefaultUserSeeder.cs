using Microsoft.AspNetCore.Identity;
using MsHuyenLC.Application.Interfaces.Services.Auth;
using MsHuyenLC.Infrastructure.Persistence;

namespace MsHuyenLC.Infrastructure.Persistence.Seed;

public class DefaultUserSeeder(IPasswordHasher passwordHasher)
{
    protected readonly IPasswordHasher _passwordHasher = passwordHasher;
    public async Task SeedAsync(ApplicationDbContext context)
    {
        if (!context.TaiKhoans.Any())
        {
            context.TaiKhoans.Add(new TaiKhoan
            {
                TenDangNhap = "admin",
                MatKhau = _passwordHasher.HashPassword("Admin@123"),
                VaiTro = 0
            });
        }

        await context.SaveChangesAsync();
    }
}
