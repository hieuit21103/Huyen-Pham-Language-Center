using FluentValidation;
using MsHuyenLC.Application.DTOs.Learning.DangKyTuVan;
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
    private readonly IValidator<DangKyTuVanRequest> _createValidator;
    private readonly IValidator<DangKyTuVanCreateRequest> _createFullValidator;
    private readonly IValidator<DangKyTuVanUpdateRequest> _updateValidator;
    private readonly IPasswordHasher _passwordHasher;
    private readonly IEmailService _emailService;
    private readonly IStudentService _studentService;
    private readonly IRegistrationService _registrationService;
    private readonly ICourseService _courseService;
    private readonly IPaymentService _paymentService;

    public GuestRegistrationService(
        IUnitOfWork unitOfWork,
        IValidator<DangKyTuVanRequest> createValidator,
        IValidator<DangKyTuVanCreateRequest> createFullValidator,
        IValidator<DangKyTuVanUpdateRequest> updateValidator,
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

    public async Task<DangKyTuVan?> GetByIdAsync(string id)
    {
        return await _unitOfWork.DangKyTuVans.GetByIdAsync(id);
    }

    public async Task<IEnumerable<DangKyTuVan>> GetAllAsync()
    {
        return await _unitOfWork.DangKyTuVans.GetAllAsync();
    }

    public async Task<DangKyTuVan> CreateAsync(DangKyTuVanRequest request)
    {
        await _createValidator.ValidateAndThrowAsync(request);

        var dangKyTuVan = new DangKyTuVan
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

        var result = await _unitOfWork.DangKyTuVans.AddAsync(dangKyTuVan);
        await _unitOfWork.SaveChangesAsync();
        return result;
    }

    public async Task<DangKyTuVan?> CreateFullAsync(DangKyTuVanCreateRequest request)
    {
        await _createFullValidator.ValidateAndThrowAsync(request);

        var dangKyTuVan = new DangKyTuVan
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

        var result = await _unitOfWork.DangKyTuVans.AddAsync(dangKyTuVan);
        await _unitOfWork.SaveChangesAsync();
        return result;
    }

    public async Task<DangKyTuVan?> UpdateAsync(string id, DangKyTuVanUpdateRequest request)
    {
        await _updateValidator.ValidateAndThrowAsync(request);

        var dangKyTuVan = await _unitOfWork.DangKyTuVans.GetByIdAsync(id);
        if (dangKyTuVan == null)
            throw new KeyNotFoundException($"Không tìm thấy đăng ký tư vấn với ID: {id}");

        var previousTrangThai = dangKyTuVan.TrangThai;
        if (request.HoTen != null)
            dangKyTuVan.HoTen = request.HoTen;

        dangKyTuVan.GioiTinh = request.GioiTinh;

        if (request.Email != null)
            dangKyTuVan.Email = request.Email;

        if (request.SoDienThoai != null)
            dangKyTuVan.SoDienThoai = request.SoDienThoai;

        dangKyTuVan.NoiDung = request.NoiDung;

        if (request.KhoaHocId.HasValue)
            dangKyTuVan.KhoaHocId = request.KhoaHocId.Value;

        if (request.TrangThai.HasValue)
            dangKyTuVan.TrangThai = request.TrangThai.Value;

        if (request.KetQua.HasValue)
            dangKyTuVan.KetQua = request.KetQua.Value;

        if (previousTrangThai != TrangThaiDangKy.daduyet &&
            dangKyTuVan.TrangThai == TrangThaiDangKy.daduyet)
        {
            await ProcessApprovedRegistrationAsync(dangKyTuVan);
        }

        await _unitOfWork.SaveChangesAsync();
        return dangKyTuVan;
    }

    /// <summary>
    /// Xử lý logic khi đăng ký được duyệt: tạo tài khoản, học viên, đăng ký và thanh toán
    /// </summary>
    private async Task ProcessApprovedRegistrationAsync(DangKyTuVan dangKyKhach)
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
        var dangKyHocVien = new DangKyKhoaHoc
        {
            HocVienId = createdHocVien.Id,
            KhoaHocId = dangKyKhach.KhoaHocId,
            NgayDangKy = DateOnly.FromDateTime(DateTime.UtcNow),
            TrangThai = TrangThaiDangKy.daduyet,
        };

        var createdDangKy = await _unitOfWork.DangKyKhoaHocs.AddAsync(dangKyHocVien);
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
        var dangKyKhach = await _unitOfWork.DangKyTuVans.GetByIdAsync(id);
        if (dangKyKhach == null)
            return false;

        await _unitOfWork.DangKyTuVans.DeleteAsync(dangKyKhach);
        await _unitOfWork.SaveChangesAsync();
        return true;
    }

    public async Task<IEnumerable<DangKyTuVan>> GetByCourseIdAsync(string courseId)
    {
        if (!Guid.TryParse(courseId, out var id))
            return Enumerable.Empty<DangKyTuVan>();

        return await _unitOfWork.DangKyTuVans.GetAllAsync(dk => dk.KhoaHocId == id);
    }

    public async Task<int> CountAsync()
    {
        return await _unitOfWork.DangKyTuVans.CountAsync();
    }
}
