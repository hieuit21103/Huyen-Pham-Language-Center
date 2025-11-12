using System;
using System.Threading;
using System.Threading.Tasks;
using MsHuyenLC.Application.Interfaces.Repositories;
using MsHuyenLC.Application.Interfaces.Repositories.Users;
using MsHuyenLC.Domain.Entities.Courses;
using MsHuyenLC.Domain.Entities.Finance;
using MsHuyenLC.Domain.Entities.Learning;
using MsHuyenLC.Domain.Entities.Learning.OnlineExam;
using MsHuyenLC.Domain.Entities.System;
using MsHuyenLC.Domain.Entities.Users;

namespace MsHuyenLC.Application.Interfaces;

/// <summary>
/// Unit of Work pattern để quản lý transactions và tập trung các repository
/// </summary>
public interface IUnitOfWork : IDisposable
{
    IGenericRepository<T> GetRepository<T>() where T : class;
    // ========== Learning - Online Exam ==========
    IGenericRepository<NganHangCauHoi> CauHois { get; }
    IGenericRepository<DapAnCauHoi> DapAnCauHois { get; }
    IGenericRepository<NhomCauHoi> NhomCauHois { get; }
    IGenericRepository<NhomCauHoiChiTiet> NhomCauHoiChiTiets { get; }
    IGenericRepository<CauHoiDeThi> CauHoiDeThis { get; }
    IGenericRepository<DeThi> DeThis { get; }
    IGenericRepository<KyThi> KyThis { get; }
    IGenericRepository<PhienLamBai> PhienLamBais { get; }
    IGenericRepository<CauTraLoi> CauTraLois { get; }
    
    // ========== Learning - Registration & Notification ==========
    IGenericRepository<DangKy> DangKys { get; }
    IGenericRepository<DangKyKhach> DangKyKhachs { get; }
    IGenericRepository<ThongBao> ThongBaos { get; }
    IGenericRepository<PhanHoi> PhanHois { get; }

    // ========== Courses ==========
    IGenericRepository<KhoaHoc> KhoaHocs { get; }
    IGenericRepository<LopHoc> LopHocs { get; }
    IGenericRepository<LichHoc> LichHocs { get; }
    IGenericRepository<PhanCong> PhanCongs { get; }
    IGenericRepository<PhongHoc> PhongHocs { get; }

    // ========== Finance ==========
    IGenericRepository<ThanhToan> ThanhToans { get; }

    // ========== Users ==========
    IUserRepository TaiKhoans { get; }
    IGenericRepository<HocVien> HocViens { get; }
    IGenericRepository<GiaoVien> GiaoViens { get; }
    IGenericRepository<GiaoVu> GiaoVus { get; }

    // ========== System ==========
    IGenericRepository<NhatKyHeThong> NhatKyHeThongs { get; }
    IGenericRepository<SaoLuuDuLieu> SaoLuuDuLieus { get; }
    IGenericRepository<CauHinhHeThong> CauHinhHeThongs { get; }

    // ========== Transaction Management ==========
    /// <summary>
    /// Lưu tất cả thay đổi vào database
    /// </summary>
    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Bắt đầu transaction mới
    /// </summary>
    Task BeginTransactionAsync();
    
    /// <summary>
    /// Commit transaction hiện tại
    /// </summary>
    Task CommitTransactionAsync();
    
    /// <summary>
    /// Rollback transaction nếu có lỗi
    /// </summary>
    Task RollbackTransactionAsync();
}