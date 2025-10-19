using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using MsHuyenLC.Application.DTOs.Learning.DangKyKhach;
using MsHuyenLC.Application.Interfaces;
using MsHuyenLC.Application.Interfaces.Auth;
using MsHuyenLC.Application.Interfaces.System;
using System.Security.Claims;

namespace MsHuyenLC.API.Controller.Learning;

[Route("api/[controller]")]
[ApiController]
public class DangKyKhachController : BaseController<DangKyKhach>
{
    private readonly IGenericService<KhoaHoc> _khoaHocService;
    private readonly IUserRepository _userRepository;
    private readonly IPasswordHasher _passwordHasher;
    private readonly IGenericService<HocVien> _hocVienService;

    public DangKyKhachController(
        IGenericService<DangKyKhach> service,
        ISystemLoggerService logService,
        IGenericService<KhoaHoc> khoaHocService,
        IUserRepository userRepository,
        IPasswordHasher passwordHasher,
        IGenericService<HocVien> hocVienService) : base(service, logService)
    {
        _khoaHocService = khoaHocService;
        _userRepository = userRepository;
        _passwordHasher = passwordHasher;
        _hocVienService = hocVienService;
    }

    protected override Func<IQueryable<DangKyKhach>, IOrderedQueryable<DangKyKhach>>? BuildOrderBy(string sortBy, string? sortOrder)
    {
        return sortBy?.ToLower() switch
        {
            "hoten" => sortOrder?.ToLower() == "desc"
                ? (q => q.OrderByDescending(d => d.HoTen))
                : (q => q.OrderBy(d => d.HoTen)),
            "ngaydangky" => sortOrder?.ToLower() == "desc"
                ? (q => q.OrderByDescending(d => d.NgayDangKy))
                : (q => q.OrderBy(d => d.NgayDangKy)),
            "trangthai" => sortOrder?.ToLower() == "desc"
                ? (q => q.OrderByDescending(d => d.TrangThai))
                : (q => q.OrderBy(d => d.TrangThai)),
            "ketqua" => sortOrder?.ToLower() == "desc"
                ? (q => q.OrderByDescending(d => d.KetQua))
                : (q => q.OrderBy(d => d.KetQua)),
            _ => sortOrder?.ToLower() == "desc"
                ? (q => q.OrderByDescending(d => d.NgayDangKy))
                : (q => q.OrderBy(d => d.NgayDangKy)),
        };
    }

    /// <summary>
    /// Guest self-registration (Public endpoint - no authentication required)
    /// </summary>
    [HttpPost("register")]
    [AllowAnonymous]
    public async Task<IActionResult> GuestRegister([FromBody] DangKyKhachRequest request)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        // Validate course exists
        var khoaHoc = await _khoaHocService.GetByIdAsync(request.KhoaHocId.ToString());
        if (khoaHoc == null)
            return BadRequest(new { message = "Khóa học không tồn tại." });

        // Check for duplicate registration (same email + phone for same course)
        var existingRegistrations = await _service.GetAllAsync(
            PageNumber: 1,
            PageSize: 1,
            Filter: d => d.Email == request.Email && 
                        d.SoDienThoai == request.SoDienThoai && 
                        d.KhoaHocId == request.KhoaHocId &&
                        (d.TrangThai == TrangThaiDangKy.choduyet || d.TrangThai == TrangThaiDangKy.daduyet)
        );

        if (existingRegistrations.Any())
            return BadRequest(new { message = "Bạn đã đăng ký khóa học này rồi. Vui lòng chờ xử lý." });

        var dangKy = new DangKyKhach
        {
            HoTen = request.HoTen,
            Email = request.Email,
            SoDienThoai = request.SoDienThoai,
            NoiDung = request.NoiDung,
            KhoaHocId = request.KhoaHocId,
            NgayDangKy = DateTime.UtcNow,
            TrangThai = TrangThaiDangKy.choduyet,
            KetQua = KetQuaDangKy.chuaxuly
        };

        var result = await _service.AddAsync(dangKy);
        if (result == null)
            return BadRequest(new { message = "Đăng ký thất bại." });

        await LogCreateAsync(result);

        var response = new DangKyKhachResponse
        {
            Id = result.Id,
            HoTen = result.HoTen,
            Email = result.Email,
            SoDienThoai = result.SoDienThoai,
            NoiDung = result.NoiDung,
            NgayDangKy = result.NgayDangKy,
            TrangThai = result.TrangThai,
            KetQua = result.KetQua,
            KhoaHocId = result.KhoaHocId,
            TenKhoaHoc = khoaHoc.TenKhoaHoc
        };

        return Ok(new 
        { 
            message = "Đăng ký thành công. Chúng tôi sẽ liên hệ với bạn trong thời gian sớm nhất.",
            data = response 
        });
    }

    /// <summary>
    /// Admin/GiaoVu manually create registration after consultation
    /// </summary>
    [HttpPost("create")]
    [Authorize(Roles = "admin,giaovu")]
    public async Task<IActionResult> CreateByAdmin([FromBody] DangKyKhachCreateDTO request)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        // Validate course exists
        var khoaHoc = await _khoaHocService.GetByIdAsync(request.KhoaHocId.ToString());
        if (khoaHoc == null)
            return BadRequest(new { message = "Khóa học không tồn tại." });

        var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

        var dangKy = new DangKyKhach
        {
            HoTen = request.HoTen,
            Email = request.Email,
            SoDienThoai = request.SoDienThoai,
            NoiDung = request.NoiDung,
            KhoaHocId = request.KhoaHocId,
            NgayDangKy = DateTime.UtcNow,
            TrangThai = TrangThaiDangKy.choduyet,
            KetQua = KetQuaDangKy.chuaxuly,
            TaiKhoanXuLyId = userId != null ? Guid.Parse(userId) : null
        };

        var result = await _service.AddAsync(dangKy);
        if (result == null)
            return BadRequest(new { message = "Tạo đăng ký thất bại." });

        await LogCreateAsync(result);

        var response = new DangKyKhachResponse
        {
            Id = result.Id,
            HoTen = result.HoTen,
            Email = result.Email,
            SoDienThoai = result.SoDienThoai,
            NoiDung = result.NoiDung,
            NgayDangKy = result.NgayDangKy,
            TrangThai = result.TrangThai,
            KetQua = result.KetQua,
            KhoaHocId = result.KhoaHocId,
            TenKhoaHoc = khoaHoc.TenKhoaHoc,
            TaiKhoanXuLyId = result.TaiKhoanXuLyId
        };

        return Ok(response);
    }

    /// <summary>
    /// Update guest registration (Admin/GiaoVu can edit all fields)
    /// </summary>
    [HttpPut("{id}")]
    [Authorize(Roles = "admin,giaovu")]
    public async Task<IActionResult> Update(string id, [FromBody] DangKyKhachUpdateRequest request)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        var existing = await _service.GetByIdAsync(id);
        if (existing == null)
            return NotFound(new { message = "Không tìm thấy đăng ký." });

        var oldData = new DangKyKhach
        {
            Id = existing.Id,
            HoTen = existing.HoTen,
            Email = existing.Email,
            SoDienThoai = existing.SoDienThoai,
            NoiDung = existing.NoiDung,
            KhoaHocId = existing.KhoaHocId,
            TrangThai = existing.TrangThai,
            KetQua = existing.KetQua,
            NgayDangKy = existing.NgayDangKy,
            NgayXuLy = existing.NgayXuLy,
            TaiKhoanXuLyId = existing.TaiKhoanXuLyId
        };

        // Update basic info if provided
        if (!string.IsNullOrWhiteSpace(request.HoTen))
            existing.HoTen = request.HoTen;

        if (!string.IsNullOrWhiteSpace(request.Email))
            existing.Email = request.Email;

        if (!string.IsNullOrWhiteSpace(request.SoDienThoai))
            existing.SoDienThoai = request.SoDienThoai;

        if (request.NoiDung != null)
            existing.NoiDung = request.NoiDung;

        // Validate and update course if provided
        if (request.KhoaHocId.HasValue)
        {
            var khoaHoc = await _khoaHocService.GetByIdAsync(request.KhoaHocId.Value.ToString());
            if (khoaHoc == null)
                return BadRequest(new { message = "Khóa học không tồn tại." });

            existing.KhoaHocId = request.KhoaHocId.Value;
        }

        // Track previous status to detect approval
        var previousTrangThai = existing.TrangThai;

        // Update status and result
        if (request.TrangThai.HasValue)
            existing.TrangThai = request.TrangThai.Value;

        if (request.KetQua.HasValue)
        {
            var previousKetQua = existing.KetQua;
            existing.KetQua = request.KetQua.Value;

            // Update processing info when status changes to processed
            if (previousKetQua != request.KetQua.Value && 
                request.KetQua.Value != KetQuaDangKy.chuaxuly)
            {
                existing.NgayXuLy = DateTime.UtcNow;
                var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                if (userId != null)
                    existing.TaiKhoanXuLyId = Guid.Parse(userId);
            }
        }

        // Auto-create account when approved (status changes to daduyet)
        if (previousTrangThai != TrangThaiDangKy.daduyet && 
            existing.TrangThai == TrangThaiDangKy.daduyet)
        {
            // Check if account already exists
            var existingAccount = await _userRepository.GetByEmailAsync(existing.Email);
            
            if (existingAccount == null)
            {
                // Create TaiKhoan with email as username and phone as password
                var taiKhoan = new TaiKhoan
                {
                    TenDangNhap = existing.Email,
                    MatKhau = _passwordHasher.HashPassword(existing.SoDienThoai),
                    Email = existing.Email,
                    Sdt = existing.SoDienThoai,
                    VaiTro = VaiTro.hocvien,
                    TrangThai = TrangThaiTaiKhoan.hoatdong
                };

                var createdAccount = await _userRepository.CreateAsync(taiKhoan);
                
                if (createdAccount != null)
                {
                    // Create HocVien profile linked to the account
                    var hocVien = new HocVien
                    {
                        HoTen = existing.HoTen,
                        TaiKhoanId = createdAccount.Id,
                        TrangThai = TrangThaiHocVien.danghoc,
                        NgayDangKy = DateTime.UtcNow
                    };

                    await _hocVienService.AddAsync(hocVien);

                    // Update KetQua to indicate account created
                    existing.KetQua = KetQuaDangKy.daxuly;
                }
            }
            else
            {
                // Account already exists - just mark as processed
                existing.KetQua = KetQuaDangKy.daxuly;
            }
        }

        await _service.UpdateAsync(existing);
        await LogUpdateAsync(oldData, existing);

        var message = existing.TrangThai == TrangThaiDangKy.daduyet && previousTrangThai != TrangThaiDangKy.daduyet
            ? "Đã duyệt và tạo tài khoản thành công. Thông tin đăng nhập: Email = " + existing.Email + ", Mật khẩu = Số điện thoại"
            : "Cập nhật đăng ký thành công.";

        return Ok(new { message });
    }

    /// <summary>
    /// Delete guest registration
    /// </summary>
    [HttpDelete("{id}")]
    [Authorize(Roles = "admin,giaovu")]
    public async Task<IActionResult> Delete(string id)
    {
        var existing = await _service.GetByIdAsync(id);
        if (existing == null)
            return NotFound(new { message = "Không tìm thấy đăng ký." });

        await _service.DeleteAsync(existing);
        await LogDeleteAsync(existing);

        return Ok(new { message = "Xóa đăng ký thành công." });
    }

    /// <summary>
    /// Get statistics for dashboard
    /// </summary>
    [HttpGet("statistics")]
    [Authorize(Roles = "admin,giaovu")]
    public async Task<IActionResult> GetStatistics()
    {
        var allRegistrations = await _service.GetAllAsync(
            PageNumber: 1,
            PageSize: int.MaxValue
        );

        var stats = new
        {
            TongSoDangKy = allRegistrations.Count(),
            ChoDuyet = allRegistrations.Count(d => d.TrangThai == TrangThaiDangKy.choduyet),
            DaDuyet = allRegistrations.Count(d => d.TrangThai == TrangThaiDangKy.daduyet),
            DaXepLop = allRegistrations.Count(d => d.TrangThai == TrangThaiDangKy.daxeplop),
            Huy = allRegistrations.Count(d => d.TrangThai == TrangThaiDangKy.huy),
            ChuaXuLy = allRegistrations.Count(d => d.KetQua == KetQuaDangKy.chuaxuly),
            DaXuLy = allRegistrations.Count(d => d.KetQua == KetQuaDangKy.daxuly),
            DaTuChoi = allRegistrations.Count(d => d.KetQua == KetQuaDangKy.datuchoi),
            DaHuy = allRegistrations.Count(d => d.KetQua == KetQuaDangKy.dahuy),
            DangKyTrongThang = allRegistrations.Count(d => d.NgayDangKy.Month == DateTime.UtcNow.Month && 
                                                           d.NgayDangKy.Year == DateTime.UtcNow.Year)
        };

        return Ok(stats);
    }

    /// <summary>
    /// Search guest registrations
    /// </summary>
    [HttpGet("search")]
    [Authorize(Roles = "admin,giaovu")]
    public async Task<IActionResult> Search(
        [FromQuery] string query,
        [FromQuery] int pageNumber = 1,
        [FromQuery] int pageSize = 10)
    {
        if (string.IsNullOrWhiteSpace(query))
            return BadRequest(new { message = "Từ khóa tìm kiếm không hợp lệ." });

        var registrations = await _service.GetAllAsync(
            PageNumber: pageNumber,
            PageSize: pageSize,
            Filter: d => d.HoTen.Contains(query) || 
                        d.Email.Contains(query) || 
                        d.SoDienThoai.Contains(query),
            OrderBy: q => q.OrderByDescending(d => d.NgayDangKy),
            Includes: d => d.KhoaHoc
        );

        var responses = new List<DangKyKhachResponse>();
        foreach (var reg in registrations)
        {
            var response = new DangKyKhachResponse
            {
                Id = reg.Id,
                HoTen = reg.HoTen,
                Email = reg.Email,
                SoDienThoai = reg.SoDienThoai,
                NoiDung = reg.NoiDung,
                NgayDangKy = reg.NgayDangKy,
                TrangThai = reg.TrangThai,
                KetQua = reg.KetQua,
                NgayXuLy = reg.NgayXuLy,
                KhoaHocId = reg.KhoaHocId,
                TenKhoaHoc = reg.KhoaHoc?.TenKhoaHoc,
                TaiKhoanXuLyId = reg.TaiKhoanXuLyId
            };

            if (reg.TaiKhoanXuLyId.HasValue)
            {
                var user = await _userRepository.GetByIdAsync(reg.TaiKhoanXuLyId.Value.ToString());
                response.NguoiXuLy = user?.TenDangNhap;
            }

            responses.Add(response);
        }

        return Ok(responses);
    }
}
