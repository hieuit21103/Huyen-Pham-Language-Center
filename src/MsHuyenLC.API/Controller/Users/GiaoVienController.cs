using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using MsHuyenLC.Application.DTOs.Users.GiaoVien;
using MsHuyenLC.Application.Interfaces;
using MsHuyenLC.Application.Interfaces.Auth;
using MsHuyenLC.Application.Interfaces.System;

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
        ISystemLoggerService logService,
        IUserRepository userRepository,
        IGenericService<PhanCong> assignmentService,
        IGenericService<LopHoc> classService,
        IPasswordHasher passwordHasher) : base(service, logService)
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
    public async Task<IActionResult> Create([FromBody] GiaoVienRequest request)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        var taiKhoan = await _userRepository.GetByIdAsync(request.TaiKhoanId.ToString());
        
        if (taiKhoan == null)
            return BadRequest(new { message = "Tài khoản không tồn tại" });

        if (taiKhoan.VaiTro != VaiTro.giaovien)
            return BadRequest(new { message = "Tài khoản không có vai trò giáo viên" });

        var giaoVien = new GiaoVien
        {
            HoTen = request.HoTen,
            ChuyenMon = request.ChuyenMon,
            TrinhDo = request.TrinhDo,
            KinhNghiem = request.KinhNghiem,
            TaiKhoanId = request.TaiKhoanId
        };

        var result = await _service.AddAsync(giaoVien);
        if (result == null)
            return BadRequest(new { message = "Tạo giáo viên thất bại" });

        var response = new GiaoVienResponse
        {
            Id = giaoVien.Id,
            HoTen = giaoVien.HoTen,
            ChuyenMon = giaoVien.ChuyenMon,
            TrinhDo = giaoVien.TrinhDo,
            KinhNghiem = giaoVien.KinhNghiem,
            TaiKhoanId = giaoVien.TaiKhoanId,
            Email = taiKhoan.Email,
            Sdt = taiKhoan.Sdt
        };

        return Ok(response);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Update(string id, [FromBody] GiaoVienUpdateRequest request)
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

        var taiKhoan = await _userRepository.GetByIdAsync(giaoVien.TaiKhoanId.ToString());
        if (taiKhoan != null)
        {
            taiKhoan.TrangThai = TrangThaiTaiKhoan.bikhoa;
            await _userRepository.UpdateAsync(taiKhoan);
        }

        return Ok(new { message = "Đã vô hiệu hóa giáo viên" });
    }
}
