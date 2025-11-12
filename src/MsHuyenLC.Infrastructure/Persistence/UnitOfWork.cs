using Microsoft.EntityFrameworkCore.Storage;
using MsHuyenLC.Application.Interfaces.Repositories;
using MsHuyenLC.Application.Interfaces;
using MsHuyenLC.Application.Interfaces.Repositories.Users;
using MsHuyenLC.Domain.Entities.Learning.OnlineExam;
using MsHuyenLC.Infrastructure.Repositories;
using MsHuyenLC.Infrastructure.Repositories.Users;

namespace MsHuyenLC.Infrastructure.Persistence;

public class UnitOfWork : IUnitOfWork
{
    private readonly ApplicationDbContext _context;
    private IDbContextTransaction? _currentTransaction;
    private IGenericRepository<CauHoi>? _cauHois;
    private IGenericRepository<DapAnCauHoi>? _dapAnCauHois;
    private IGenericRepository<NhomCauHoi>? _nhomCauHois;
    private IGenericRepository<NhomCauHoiChiTiet>? _nhomCauHoiChiTiets;
    private IGenericRepository<CauHoiDeThi>? _cauHoiDeThis;
    private IGenericRepository<DeThi>? _deThis;
    private IGenericRepository<KyThi>? _kyThis;
    private IGenericRepository<PhienLamBai>? _phienLamBais;
    private IGenericRepository<CauTraLoi>? _cauTraLois;
    private IGenericRepository<DangKyKhoaHoc>? _dangKyKhoaHocs;
    private IGenericRepository<DangKyTuVan>? _dangKyTuVans;
    private IGenericRepository<ThongBao>? _thongBaos;
    private IGenericRepository<PhanHoi>? _phanHois;
    private IGenericRepository<KhoaHoc>? _khoaHocs;
    private IGenericRepository<LopHoc>? _lopHocs;
    private IGenericRepository<LichHoc>? _lichHocs;
    private IGenericRepository<PhongHoc>? _phongHocs;
    private IGenericRepository<PhanCong>? _phanCongs;
    private IGenericRepository<ThanhToan>? _thanhToans;
    private IUserRepository? _taiKhoans;
    private IGenericRepository<HocVien>? _hocViens;
    private IGenericRepository<GiaoVien>? _giaoViens;
    private IGenericRepository<GiaoVu>? _giaoVus;
    private IGenericRepository<NhatKyHeThong>? _nhatKyHeThongs;
    private IGenericRepository<SaoLuuDuLieu>? _saoLuuDuLieus;
    private IGenericRepository<CauHinhHeThong>? _cauHinhHeThongs;

    public UnitOfWork(ApplicationDbContext context)
    {
        _context = context ?? throw new ArgumentNullException(nameof(context));
    }

    public IGenericRepository<T> GetRepository<T>() where T : class
    {
        return new GenericRepository<T>(_context);
    }

    // ========== Learning - Online Exam ==========
    public IGenericRepository<CauHoi> CauHois => 
        _cauHois ??= GetRepository<CauHoi>();
    
    public IGenericRepository<DapAnCauHoi> DapAnCauHois => 
        _dapAnCauHois ??= GetRepository<DapAnCauHoi>();
    
    public IGenericRepository<NhomCauHoi> NhomCauHois => 
        _nhomCauHois ??= GetRepository<NhomCauHoi>();

    public IGenericRepository<NhomCauHoiChiTiet> NhomCauHoiChiTiets => 
        _nhomCauHoiChiTiets ??= GetRepository<NhomCauHoiChiTiet>();

    public IGenericRepository<CauHoiDeThi> CauHoiDeThis => 
        _cauHoiDeThis ??= GetRepository<CauHoiDeThi>();

    public IGenericRepository<DeThi> DeThis => 
        _deThis ??= GetRepository<DeThi>();

    public IGenericRepository<KyThi> KyThis => 
        _kyThis ??= GetRepository<KyThi>();

    public IGenericRepository<PhienLamBai> PhienLamBais => 
        _phienLamBais ??= GetRepository<PhienLamBai>();

    public IGenericRepository<CauTraLoi> CauTraLois => 
        _cauTraLois ??= GetRepository<CauTraLoi>();

    // ========== Learning - Registration & Notification ==========
    public IGenericRepository<DangKyKhoaHoc> DangKyKhoaHocs => 
        _dangKyKhoaHocs ??= GetRepository<DangKyKhoaHoc>();

    public IGenericRepository<DangKyTuVan> DangKyTuVans => 
        _dangKyTuVans ??= GetRepository<DangKyTuVan>();

    public IGenericRepository<ThongBao> ThongBaos => 
        _thongBaos ??= GetRepository<ThongBao>();

    public IGenericRepository<PhanHoi> PhanHois => 
        _phanHois ??= GetRepository<PhanHoi>();

    // ========== Courses ==========
    public IGenericRepository<KhoaHoc> KhoaHocs => 
        _khoaHocs ??= GetRepository<KhoaHoc>();

    public IGenericRepository<LopHoc> LopHocs => 
        _lopHocs ??= GetRepository<LopHoc>();

    public IGenericRepository<LichHoc> LichHocs => 
        _lichHocs ??= GetRepository<LichHoc>();

    public IGenericRepository<PhanCong> PhanCongs =>
        _phanCongs ??= GetRepository<PhanCong>();
    
    public IGenericRepository<PhongHoc> PhongHocs =>
        _phongHocs ??= GetRepository<PhongHoc>();   

    // ========== Finance ==========
    public IGenericRepository<ThanhToan> ThanhToans => 
        _thanhToans ??= GetRepository<ThanhToan>();

    // ========== Users ==========
    public IUserRepository TaiKhoans =>
        _taiKhoans ??= new UserRepository(_context);
    
    public IGenericRepository<HocVien> HocViens => 
        _hocViens ??= GetRepository<HocVien>();

    public IGenericRepository<GiaoVien> GiaoViens => 
        _giaoViens ??= GetRepository<GiaoVien>();

    public IGenericRepository<GiaoVu> GiaoVus => 
        _giaoVus ??= GetRepository<GiaoVu>();

    // ========== System ==========
    public IGenericRepository<NhatKyHeThong> NhatKyHeThongs => 
        _nhatKyHeThongs ??= GetRepository<NhatKyHeThong>();

    public IGenericRepository<SaoLuuDuLieu> SaoLuuDuLieus => 
        _saoLuuDuLieus ??= GetRepository<SaoLuuDuLieu>();

    public IGenericRepository<CauHinhHeThong> CauHinhHeThongs => 
        _cauHinhHeThongs ??= GetRepository<CauHinhHeThong>();

    // ========== Transaction Management ==========
    public async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        return await _context.SaveChangesAsync(cancellationToken);
    }

    public async Task BeginTransactionAsync()
    {
        if (_currentTransaction != null)
        {
            throw new InvalidOperationException("Transaction đã được bắt đầu.");
        }

        _currentTransaction = await _context.Database.BeginTransactionAsync();
    }

    public async Task CommitTransactionAsync()
    {
        try
        {
            if (_currentTransaction == null)
            {
                throw new InvalidOperationException("Không có transaction nào để commit.");
            }

            await _context.SaveChangesAsync();
            await _currentTransaction.CommitAsync();
        }
        catch
        {
            await RollbackTransactionAsync();
            throw;
        }
        finally
        {
            if (_currentTransaction != null)
            {
                await _currentTransaction.DisposeAsync();
                _currentTransaction = null;
            }
        }
    }

    public async Task RollbackTransactionAsync()
    {
        try
        {
            if (_currentTransaction != null)
            {
                await _currentTransaction.RollbackAsync();
            }
        }
        finally
        {
            if (_currentTransaction != null)
            {
                await _currentTransaction.DisposeAsync();
                _currentTransaction = null;
            }
        }
    }

    public void Dispose()
    {
        _currentTransaction?.Dispose();
        _context.Dispose();
    }
}
