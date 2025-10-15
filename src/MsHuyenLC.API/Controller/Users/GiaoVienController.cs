using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using MsHuyenLC.Application.DTOs.Teachers;
using MsHuyenLC.Application.Interfaces;
using MsHuyenLC.Application.Interfaces.Auth;

namespace MsHuyenLC.API.Controller.Users;

[ApiController]
[Route("api/[controller]")]
[Authorize(Roles = "admin,giaovu")]
public class GiaoVienController : BaseController<GiaoVien>
{
    private readonly IUserRepository _userRepository;
    private readonly IGenericService<PhanCong> _assignmentService;
    private readonly IGenericService<LopHoc> _classService;
    private readonly IPasswordHasher _passwordHasher;

    public GiaoVienController(
        IGenericService<GiaoVien> service,
        IUserRepository userRepository,
        IGenericService<PhanCong> assignmentService,
        IGenericService<LopHoc> classService,
        IPasswordHasher passwordHasher) : base(service)
    {
        _userRepository = userRepository;
        _assignmentService = assignmentService;
        _classService = classService;
        _passwordHasher = passwordHasher;
    }

    protected override Func<IQueryable<GiaoVien>, IOrderedQueryable<GiaoVien>>? BuildOrderBy(string sortBy, string? sortOrder)
    {
        return sortBy?.ToLower() switch
        {
            "hoten" => sortOrder?.ToLower() == "desc"
                ? (q => q.OrderByDescending(t => t.HoTen))
                : (q => q.OrderBy(t => t.HoTen)),
            "chuyenmon" => sortOrder?.ToLower() == "desc"
                ? (q => q.OrderByDescending(t => t.ChuyenMon))
                : (q => q.OrderBy(t => t.ChuyenMon)),
            _ => sortOrder?.ToLower() == "desc"
                ? (q => q.OrderByDescending(t => t.Id))
                : (q => q.OrderBy(t => t.Id)),
        };
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] TeacherCreateRequest request)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        var existingAccount = await _userRepository.GetByEmailAsync(request.Email);
        
        if (existingAccount != null)
            return BadRequest(new { message = "Email đã được sử dụng" });

        var taiKhoan = new TaiKhoan
        {
            Id = Guid.NewGuid(),
            TenDangNhap = request.Email,
            Email = request.Email,
            Sdt = request.SoDienThoai,
            MatKhau = _passwordHasher.HashPassword(request.SoDienThoai),
            VaiTro = VaiTro.giaovien,
        };

        if (await _userRepository.CreateAsync(taiKhoan) == null)
            return BadRequest(new { message = "Tạo tài khoản thất bại" });

        var giaoVien = new GiaoVien
        {
            Id = taiKhoan.Id,
            HoTen = request.HoTen,
            ChuyenMon = request.ChuyenMon,
            TrinhDo = request.TrinhDo,
            KinhNghiem = request.KinhNghiem,
            TaiKhoan = taiKhoan
        };

        if (await _service.AddAsync(giaoVien) == null)
            return BadRequest(new { message = "Tạo giáo viên thất bại" });

        var response = new TeacherResponse
        {
            Id = giaoVien.Id,
            HoTen = giaoVien.HoTen,
            ChuyenMon = giaoVien.ChuyenMon,
            TrinhDo = giaoVien.TrinhDo,
            KinhNghiem = giaoVien.KinhNghiem,
            Email = taiKhoan.Email ?? "",
            SoDienThoai = taiKhoan.Sdt ?? "",
            SoLopDangDay = 0
        };

        return Ok(response);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Update(string id, [FromBody] TeacherUpdateRequest request)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        var giaoVien = await _service.GetByIdAsync(id);
            
        if (giaoVien == null)
            return NotFound(new { message = "Không tìm thấy giáo viên" });

        giaoVien.HoTen = request.HoTen;
        giaoVien.ChuyenMon = request.ChuyenMon;
        giaoVien.TrinhDo = request.TrinhDo;
        giaoVien.KinhNghiem = request.KinhNghiem;

        await _service.UpdateAsync(giaoVien);

        return Ok(new { message = "Cập nhật thông tin giáo viên thành công" });
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(string id)
    {
        var giaoVien = await _service.GetByIdAsync(id);

        if (giaoVien == null)
            return NotFound(new { message = "Không tìm thấy giáo viên" });

        if (giaoVien.PhanCongs.Any())
            return BadRequest(new { message = "Không thể xóa giáo viên đang được phân công dạy lớp" });

        giaoVien.TaiKhoan.TrangThai = TrangThaiTaiKhoan.bikhoa;
        await _userRepository.UpdateAsync(giaoVien.TaiKhoan);

        return Ok(new { message = "Đã vô hiệu hóa giáo viên" });
    }
}
