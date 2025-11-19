using FluentValidation;
using MsHuyenLC.Application.DTOs.Finance.ThanhToan;
using MsHuyenLC.Application.DTOs.Learning.DangKyKhoaHoc;
using MsHuyenLC.Application.Interfaces;
using MsHuyenLC.Application.Interfaces.Services.Learning;
using MsHuyenLC.Domain.Entities.Courses;
using MsHuyenLC.Domain.Entities.Learning;
using MsHuyenLC.Domain.Entities.Finance;
using MsHuyenLC.Domain.Enums;
using MsHuyenLC.Domain.Entities.Users;

namespace MsHuyenLC.Application.Services.Learning;

public class RegistrationService : IRegistrationService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IValidator<DangKyKhoaHocRequest> _createValidator;
    private readonly IValidator<DangKyKhoaHocCreateRequest> _createFullValidator;
    private readonly IValidator<DangKyKhoaHocUpdateRequest> _updateValidator;

    public RegistrationService(
        IUnitOfWork unitOfWork,
        IValidator<DangKyKhoaHocRequest> createValidator,
        IValidator<DangKyKhoaHocCreateRequest> createFullValidator,
        IValidator<DangKyKhoaHocUpdateRequest> updateValidator)
    {
        _unitOfWork = unitOfWork;
        _createValidator = createValidator;
        _createFullValidator = createFullValidator;
        _updateValidator = updateValidator;
    }

    public async Task<DangKyKhoaHoc?> GetByIdAsync(string id, string userId = "")
    {
        if (string.IsNullOrEmpty(userId))
        {
            return await _unitOfWork.DangKyKhoaHocs.GetByIdAsync(id);
        }
        else
        {
            var dangKy = await _unitOfWork.DangKyKhoaHocs.GetByIdAsync(id);
            if (dangKy == null || dangKy.HocVien == null)
                return null;
            var taiKhoan = await _unitOfWork.TaiKhoans.GetByIdAsync(dangKy.HocVien.TaiKhoanId.ToString());
            if (taiKhoan != null && taiKhoan.Id.ToString() == userId)
            {
                return dangKy;
            }
            return null;
        }
    }

    public async Task<IEnumerable<DangKyKhoaHoc>> GetAllAsync()
    {
        return await _unitOfWork.DangKyKhoaHocs.GetAllAsync();
    }

    public async Task<DangKyKhoaHoc> CreateAsync(DangKyKhoaHocRequest request)
    {
        await _createValidator.ValidateAndThrowAsync(request);

        var dangKy = new DangKyKhoaHoc
        {
            HocVienId = request.HocVienId,
            KhoaHocId = request.KhoaHocId,
            NgayDangKy = DateOnly.FromDateTime(DateTime.UtcNow),
            TrangThai = TrangThaiDangKy.choduyet
        };

        var result = await _unitOfWork.DangKyKhoaHocs.AddAsync(dangKy);
        await _unitOfWork.SaveChangesAsync();
        return result;
    }

    public async Task<DangKyKhoaHoc> CreateFullAsync(DangKyKhoaHocCreateRequest request)
    {
        await _createFullValidator.ValidateAndThrowAsync(request);

        var dangKy = new DangKyKhoaHoc
        {
            HocVienId = request.HocVienId,
            KhoaHocId = request.KhoaHocId,
            TrangThai = request.TrangThai,
            NgayDangKy = request.NgayDangKy ?? DateOnly.FromDateTime(DateTime.UtcNow),
            LopHocId = request.LopHocId,
            NgayXepLop = request.NgayXepLop
        };

        var result = await _unitOfWork.DangKyKhoaHocs.AddAsync(dangKy);
        await _unitOfWork.SaveChangesAsync();
        return result;
    }

    public async Task<DangKyKhoaHoc?> UpdateAsync(string id, DangKyKhoaHocUpdateRequest request)
    {
        await _updateValidator.ValidateAndThrowAsync(request);

        var dangKy = await _unitOfWork.DangKyKhoaHocs.GetByIdAsync(id);
        if (dangKy == null)
            throw new KeyNotFoundException($"Không tìm thấy đăng ký với ID: {id}");

        if (request.KhoaHocId.HasValue)
            dangKy.KhoaHocId = request.KhoaHocId.Value;

        if (request.HocVienId.HasValue)
            dangKy.HocVienId = request.HocVienId.Value;

        if (request.NgayDangKy.HasValue)
            dangKy.NgayDangKy = request.NgayDangKy.Value;

        if (request.LopHocId.HasValue)
            dangKy.LopHocId = request.LopHocId.Value;

        if (request.NgayXepLop.HasValue)
            dangKy.NgayXepLop = request.NgayXepLop.Value;

        var previousStatus = dangKy.TrangThai;
        if (request.TrangThai.HasValue)
            dangKy.TrangThai = request.TrangThai.Value;

        if (previousStatus == TrangThaiDangKy.choduyet && dangKy.TrangThai == TrangThaiDangKy.daduyet)
        {
            await _unitOfWork.ThanhToans.AddAsync(new ThanhToan
            {
                DangKyId = dangKy.Id,
                SoTien = dangKy.KhoaHoc.HocPhi,
            });
        }

        if (previousStatus == TrangThaiDangKy.dathanhtoan && dangKy.TrangThai == TrangThaiDangKy.daxeplop)
        {
            dangKy.NgayXepLop = DateOnly.FromDateTime(DateTime.UtcNow);
            var lopHoc = await _unitOfWork.LopHocs.GetByIdAsync(dangKy.LopHocId.ToString() ?? "");
            if (lopHoc != null)
            {
                lopHoc.CapNhatSiSo(1);
                await _unitOfWork.LopHocs.UpdateAsync(lopHoc);
            }
        }

        if (previousStatus == TrangThaiDangKy.daduyet && dangKy.TrangThai == TrangThaiDangKy.huy)
        {
            if (dangKy.LopHocId.HasValue)
            {
                var lopHoc = await _unitOfWork.LopHocs.GetByIdAsync(dangKy.LopHocId.ToString() ?? "");
                if (lopHoc != null)
                {
                    lopHoc.CapNhatSiSo(-1);
                    await _unitOfWork.LopHocs.UpdateAsync(lopHoc);
                }
            }
        }

        if (previousStatus == TrangThaiDangKy.daduyet && dangKy.TrangThai == TrangThaiDangKy.dathanhtoan)
        {
            var thanhToans = await _unitOfWork.ThanhToans.GetAllAsync(filter: t => t.DangKyId == dangKy.Id && t.TrangThai == TrangThaiThanhToan.chuathanhtoan);
            var thanhToan = thanhToans.FirstOrDefault();
            if (thanhToan != null)
            {
                thanhToan.TrangThai = TrangThaiThanhToan.dathanhtoan;
                thanhToan.NgayThanhToan = DateOnly.FromDateTime(DateTime.UtcNow);
                await _unitOfWork.ThanhToans.UpdateAsync(thanhToan);
            }
        }

        await _unitOfWork.SaveChangesAsync();
        return dangKy;
    }

    public async Task<bool> DeleteAsync(string id)
    {
        var dangKy = await _unitOfWork.DangKyKhoaHocs.GetByIdAsync(id);
        if (dangKy == null)
            return false;
        var lopHoc = await _unitOfWork.LopHocs.GetByIdAsync(dangKy.LopHocId.ToString() ?? "");
        if (lopHoc != null && dangKy.TrangThai == TrangThaiDangKy.daduyet)
        {
            lopHoc.CapNhatSiSo(-1);
            await _unitOfWork.LopHocs.UpdateAsync(lopHoc);
        }
        await _unitOfWork.DangKyKhoaHocs.DeleteAsync(dangKy);
        await _unitOfWork.SaveChangesAsync();
        return true;
    }

    public async Task<IEnumerable<DangKyKhoaHoc>> GetByStudentIdAsync(string studentId)
    {
        if (!Guid.TryParse(studentId, out var id))
            return Enumerable.Empty<DangKyKhoaHoc>();

        return await _unitOfWork.DangKyKhoaHocs.GetAllAsync(x => x.HocVienId == id);
    }

    public async Task<IEnumerable<DangKyKhoaHoc>> GetByCourseIdAsync(string courseId)
    {
        if (!Guid.TryParse(courseId, out var id))
            return Enumerable.Empty<DangKyKhoaHoc>();

        return await _unitOfWork.DangKyKhoaHocs.GetAllAsync(x => x.KhoaHocId == id);
    }

    public async Task<int> CountAsync()
    {
        return await _unitOfWork.DangKyKhoaHocs.CountAsync();
    }
}
