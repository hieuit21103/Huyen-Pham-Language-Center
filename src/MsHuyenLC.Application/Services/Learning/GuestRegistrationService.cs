using FluentValidation;
using MsHuyenLC.Application.DTOs.Learning.DangKyKhach;
using MsHuyenLC.Application.Interfaces;
using MsHuyenLC.Application.Interfaces.Services.Learning;
using MsHuyenLC.Application.Interfaces.Services.Auth;
using MsHuyenLC.Application.Interfaces.Services.Email;
using MsHuyenLC.Application.Interfaces.Services.User;
using MsHuyenLC.Application.Interfaces.Services.Course;
using MsHuyenLC.Application.Interfaces.Services.Finance;
using MsHuyenLC.Domain.Entities.Learning;
using MsHuyenLC.Domain.Entities.Users;
using MsHuyenLC.Domain.Entities.Finance;
using MsHuyenLC.Domain.Enums;

namespace MsHuyenLC.Application.Services.Learning;

public class GuestRegistrationService : IGuestRegistrationService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IValidator<DangKyKhachRequest> _createValidator;
    private readonly IValidator<DangKyKhachCreateRequest> _createFullValidator;
    private readonly IValidator<DangKyKhachUpdateRequest> _updateValidator;
    private readonly IPasswordHasher _passwordHasher;
    private readonly IEmailService _emailService;
    private readonly IStudentService _studentService;
    private readonly IRegistrationService _registrationService;
    private readonly ICourseService _courseService;
    private readonly IPaymentService _paymentService;

    public GuestRegistrationService(
        IUnitOfWork unitOfWork,
        IValidator<DangKyKhachRequest> createValidator,
        IValidator<DangKyKhachCreateRequest> createFullValidator,
        IValidator<DangKyKhachUpdateRequest> updateValidator,
        IPasswordHasher passwordHasher,
        IEmailService emailService,
        IStudentService studentService,
        IRegistrationService registrationService,
        ICourseService courseService,
        IPaymentService paymentService)
    {
        _unitOfWork = unitOfWork;
        _createValidator = createValidator;
        _createFullValidator = createFullValidator;
        _updateValidator = updateValidator;
        _passwordHasher = passwordHasher;
        _emailService = emailService;
        _studentService = studentService;
        _registrationService = registrationService;
        _courseService = courseService;
        _paymentService = paymentService;
    }

    public async Task<DangKyKhach?> GetByIdAsync(string id)
    {
        return await _unitOfWork.DangKyKhachs.GetByIdAsync(id);
    }

    public async Task<IEnumerable<DangKyKhach>> GetAllAsync()
    {
        return await _unitOfWork.DangKyKhachs.GetAllAsync();
    }

    public async Task<DangKyKhach> CreateAsync(DangKyKhachRequest request)
    {
        await _createValidator.ValidateAndThrowAsync(request);

        var dangKyKhach = new DangKyKhach
        {
            HoTen = request.HoTen,
            GioiTinh = request.GioiTinh,
            Email = request.Email,
            SoDienThoai = request.SoDienThoai,
            NoiDung = request.NoiDung,
            KhoaHocId = request.KhoaHocId,
            NgayDangKy = DateOnly.FromDateTime(DateTime.UtcNow),
            TrangThai = TrangThaiDangKy.choduyet,
            KetQua = KetQuaDangKy.chuaxuly
        };

        var result = await _unitOfWork.DangKyKhachs.AddAsync(dangKyKhach);
        await _unitOfWork.SaveChangesAsync();
        return result;
    }

    public async Task<DangKyKhach?> CreateFullAsync(DangKyKhachCreateRequest request)
    {
        await _createFullValidator.ValidateAndThrowAsync(request);

        var dangKyKhach = new DangKyKhach
        {
            HoTen = request.HoTen,
            GioiTinh = request.GioiTinh,
            Email = request.Email,
            SoDienThoai = request.SoDienThoai,
            NoiDung = request.NoiDung,
            KhoaHocId = request.KhoaHocId,
            NgayDangKy = DateOnly.FromDateTime(DateTime.UtcNow),
            TrangThai = TrangThaiDangKy.choduyet,
            KetQua = KetQuaDangKy.chuaxuly
        };

        var result = await _unitOfWork.DangKyKhachs.AddAsync(dangKyKhach);
        await _unitOfWork.SaveChangesAsync();
        return result;
    }

    public async Task<DangKyKhach?> UpdateAsync(string id, DangKyKhachUpdateRequest request)
    {
        await _updateValidator.ValidateAndThrowAsync(request);

        var dangKyKhach = await _unitOfWork.DangKyKhachs.GetByIdAsync(id);
        if (dangKyKhach == null)
            throw new KeyNotFoundException($"Không tìm thấy đăng ký khách với ID: {id}");

        var previousTrangThai = dangKyKhach.TrangThai;
        if (request.HoTen != null)
            dangKyKhach.HoTen = request.HoTen;

        dangKyKhach.GioiTinh = request.GioiTinh;

        if (request.Email != null)
            dangKyKhach.Email = request.Email;

        if (request.SoDienThoai != null)
            dangKyKhach.SoDienThoai = request.SoDienThoai;

        dangKyKhach.NoiDung = request.NoiDung;

        if (request.KhoaHocId.HasValue)
            dangKyKhach.KhoaHocId = request.KhoaHocId.Value;

        if (request.TrangThai.HasValue)
            dangKyKhach.TrangThai = request.TrangThai.Value;

        if (request.KetQua.HasValue)
            dangKyKhach.KetQua = request.KetQua.Value;

        if (previousTrangThai != TrangThaiDangKy.daduyet &&
            dangKyKhach.TrangThai == TrangThaiDangKy.daduyet)
        {
            await ProcessApprovedRegistrationAsync(dangKyKhach);
        }

        await _unitOfWork.SaveChangesAsync();
        return dangKyKhach;
    }

    /// <summary>
    /// Xử lý logic khi đăng ký được duyệt: tạo tài khoản, học viên, đăng ký và thanh toán
    /// </summary>
    private async Task ProcessApprovedRegistrationAsync(DangKyKhach dangKyKhach)
    {
        var existingAccount = await _unitOfWork.TaiKhoans.GetByEmailAsync(dangKyKhach.Email);
        
        if (existingAccount != null)
        {
            dangKyKhach.KetQua = KetQuaDangKy.daxuly;
            return;
        }

        var khoaHoc = await _courseService.GetByIdAsync(dangKyKhach.KhoaHocId.ToString());
        if (khoaHoc == null)
        {
            throw new InvalidOperationException("Khóa học không tồn tại");
        }

        // 1. Tạo tài khoản
        var taiKhoan = new TaiKhoan
        {
            TenDangNhap = dangKyKhach.Email,
            MatKhau = _passwordHasher.HashPassword(dangKyKhach.SoDienThoai),
            Email = dangKyKhach.Email,
            Sdt = dangKyKhach.SoDienThoai,
            VaiTro = VaiTro.hocvien,
            TrangThai = TrangThaiTaiKhoan.hoatdong
        };

        var createdAccount = await _unitOfWork.TaiKhoans.AddAsync(taiKhoan);
        await _unitOfWork.SaveChangesAsync();

        if (createdAccount == null)
        {
            throw new InvalidOperationException("Tạo tài khoản thất bại");
        }

        // 2. Tạo học viên
        var hocVien = new HocVien
        {
            HoTen = dangKyKhach.HoTen,
            GioiTinh = dangKyKhach.GioiTinh,
            TaiKhoanId = createdAccount.Id,
            TrangThai = TrangThaiHocVien.danghoc,
            NgayDangKy = DateOnly.FromDateTime(DateTime.UtcNow)
        };

        var createdHocVien = await _unitOfWork.HocViens.AddAsync(hocVien);
        await _unitOfWork.SaveChangesAsync();

        if (createdHocVien == null)
        {
            throw new InvalidOperationException("Tạo học viên thất bại");
        }

        // 3. Tạo đăng ký học viên
        var dangKyHocVien = new DangKy
        {
            HocVienId = createdHocVien.Id,
            KhoaHocId = dangKyKhach.KhoaHocId,
            NgayDangKy = DateOnly.FromDateTime(DateTime.UtcNow),
            TrangThai = TrangThaiDangKy.daduyet,
        };

        var createdDangKy = await _unitOfWork.DangKys.AddAsync(dangKyHocVien);
        await _unitOfWork.SaveChangesAsync();

        if (createdDangKy == null)
        {
            throw new InvalidOperationException("Tạo đăng ký học viên thất bại");
        }

        // 4. Tạo thanh toán
        var thanhToan = new ThanhToan
        {
            DangKyId = createdDangKy.Id,
            SoTien = khoaHoc.HocPhi,
        };

        var createdThanhToan = await _unitOfWork.ThanhToans.AddAsync(thanhToan);
        await _unitOfWork.SaveChangesAsync();

        if (createdThanhToan == null)
        {
            throw new InvalidOperationException("Tạo thanh toán thất bại");
        }

        // 5. Gửi email thông báo
        try
        {
            if (!string.IsNullOrEmpty(createdAccount.Email))
            {
                await _emailService.SendAccountCreationEmailAsync(
                    to: createdAccount.Email,
                    fullName: createdHocVien.HoTen,
                    userName: createdAccount.Email,
                    temporaryPassword: dangKyKhach.SoDienThoai
                );

                await _emailService.SendWelcomeStudentEmailAsync(
                    to: createdAccount.Email,
                    fullName: createdHocVien.HoTen,
                    courseName: khoaHoc.TenKhoaHoc,
                    startDate: DateOnly.FromDateTime(DateTime.UtcNow)
                );
            }
        }
        catch (Exception)
        {

        }

        // 6. Cập nhật kết quả xử lý
        dangKyKhach.KetQua = KetQuaDangKy.daxuly;
    }

    public async Task<bool> DeleteAsync(string id)
    {
        var dangKyKhach = await _unitOfWork.DangKyKhachs.GetByIdAsync(id);
        if (dangKyKhach == null)
            return false;

        await _unitOfWork.DangKyKhachs.DeleteAsync(dangKyKhach);
        await _unitOfWork.SaveChangesAsync();
        return true;
    }

    public async Task<IEnumerable<DangKyKhach>> GetByCourseIdAsync(string courseId)
    {
        if (!Guid.TryParse(courseId, out var id))
            return Enumerable.Empty<DangKyKhach>();

        return await _unitOfWork.DangKyKhachs.GetAllAsync(dk => dk.KhoaHocId == id);
    }

    public async Task<int> CountAsync()
    {
        return await _unitOfWork.DangKyKhachs.CountAsync();
    }
}
