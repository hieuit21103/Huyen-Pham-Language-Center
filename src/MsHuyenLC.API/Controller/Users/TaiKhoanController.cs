using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using MsHuyenLC.Application.Interfaces.Auth;
using System.Security.Claims;
using MsHuyenLC.Application.DTOs.Users.TaiKhoan;

namespace MsHuyenLC.API.Controllers.Users;

[ApiController]
[Route("api/[controller]")]
[Authorize(Roles = "admin")]
public class TaiKhoanController : ControllerBase
{
    protected readonly IUserRepository _userRepository;

    public TaiKhoanController(IUserRepository userRepository)
    {
        _userRepository = userRepository;
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetUserById(string id)
    {
        if (string.IsNullOrEmpty(id)) return BadRequest(new { message = "Tài khoản không hợp lệ." });
        var user = await _userRepository.GetByIdAsync(id);
        if (user == null) return NotFound();

        return Ok(user);
    }

    [HttpGet]
    public async Task<IActionResult> GetAllUsers(
        [FromQuery] int pageNumber = 1,
        [FromQuery] int pageSize = 10,
        [FromQuery] string? sortBy = null,
        [FromQuery] string? sortOrder = "asc"
    )
    {
        Func<IQueryable<TaiKhoan>, IOrderedQueryable<TaiKhoan>>? orderBy = null;
        if (!string.IsNullOrEmpty(sortBy))
        {
            orderBy = BuildOrderBy(sortBy, sortOrder);
        }
        var users = await _userRepository.GetAllAsync(
            PageNumber: pageNumber,
            PageSize: pageSize,
            OrderBy: orderBy
        );
        return Ok(users);
    }

    protected virtual Func<IQueryable<TaiKhoan>, IOrderedQueryable<TaiKhoan>>? BuildOrderBy(string sortBy, string? sortOrder)
    {
        return sortBy?.ToLower() switch
        {
            "tendangnhap" => sortOrder?.ToLower() == "desc"
                ? (q => q.OrderByDescending(u => u.TenDangNhap))
                : (q => q.OrderBy(u => u.TenDangNhap)),
            "email" => sortOrder?.ToLower() == "desc"
                ? (q => q.OrderByDescending(u => u.Email))
                : (q => q.OrderBy(u => u.Email)),
            _ => sortOrder?.ToLower() == "desc"
                ? (q => q.OrderByDescending(u => u.Id))
                : (q => q.OrderBy(u => u.Id)),
        };
    }

    [HttpGet("search")]
    public async Task<IActionResult> SearchUsers(
        [FromQuery] string query,
        [FromQuery] int pageNumber = 1,
        [FromQuery] int pageSize = 10,
        [FromQuery] string? sortBy = null,
        [FromQuery] string? sortOrder = "asc"
    )
    {
        if (string.IsNullOrEmpty(query)) return BadRequest(new { message = "Tham số tìm kiếm không hợp lệ." });

        Func<IQueryable<TaiKhoan>, IOrderedQueryable<TaiKhoan>>? orderBy = null;
        if (!string.IsNullOrEmpty(sortBy))
        {
            orderBy = BuildOrderBy(sortBy, sortOrder);
        }

        var users = await _userRepository.GetAllAsync(
            Filter: u => u.TenDangNhap.Contains(query) || u.Email.Contains(query),
            PageNumber: pageNumber,
            PageSize: pageSize,
            OrderBy: orderBy
        );

        return Ok(users);
    }

    [HttpPost]
    public async Task<IActionResult> CreateUser([FromBody] TaiKhoanRequest request)
    {
        if (!ModelState.IsValid) return BadRequest(ModelState);

        if (request == null) return BadRequest(new { message = "Dữ liệu không hợp lệ." });

        var existingUserByUsername = await _userRepository.GetByUsernameAsync(request.TenDangNhap);
        if (existingUserByUsername != null)
            return Conflict(new { message = "Tên đăng nhập đã tồn tại." });

        var existingUserByEmail = await _userRepository.GetByEmailAsync(request.Email ?? string.Empty);
        if (existingUserByEmail != null)
            return Conflict(new { message = "Email đã tồn tại." });

        var user = new TaiKhoan
        {
            TenDangNhap = request.TenDangNhap,
            MatKhau = request.MatKhau,
            Email = request.Email,
            Sdt = request.Sdt,
            Avatar = request.Avatar,
        };

        var createdUser = await _userRepository.CreateAsync(user);
        await _userRepository.SaveChangesAsync();

        return Ok(createdUser);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateUser(string id, [FromBody] TaiKhoanUpdateRequest request)
    {
        if (string.IsNullOrEmpty(id) || request == null)
            return BadRequest(new { message = "Dữ liệu không hợp lệ." });

        if (User.FindFirst(ClaimTypes.NameIdentifier)?.Value != id && !User.IsInRole("admin"))
            return Forbid();

        var existingUser = await _userRepository.GetByIdAsync(id);
        if (existingUser == null) return NotFound();

        if (existingUser.Email != request.Email)
        {
            var userWithSameEmail = await _userRepository.GetByEmailAsync(request.Email ?? string.Empty);
            if (userWithSameEmail != null)
                return Conflict(new { message = "Email đã tồn tại." });
        }

        existingUser.VaiTro = request.VaiTro;
        existingUser.Email = request.Email ?? existingUser.Email;
        existingUser.Sdt = request.Sdt ?? existingUser.Sdt;
        existingUser.Avatar = request.Avatar ?? existingUser.Avatar;
        existingUser.TrangThai = request.TrangThai;

        await _userRepository.UpdateAsync(existingUser);
        await _userRepository.SaveChangesAsync();

        return Ok(existingUser);
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteUser(string id)
    {
        if (string.IsNullOrEmpty(id)) return BadRequest(new { message = "Tài khoản không hợp lệ." });

        var existingUser = await _userRepository.GetByIdAsync(id);
        if (existingUser == null) return NotFound();

        await _userRepository.DeleteAsync(id);
        await _userRepository.SaveChangesAsync();

        return Ok(new { message = "Xóa tài khoản thành công." });
    }
}