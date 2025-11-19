using MsHuyenLC.Domain.Entities.Learning.OnlineExam;

namespace MsHuyenLC.Infrastructure.Persistence;

public class ApplicationDbContext : DbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
    : base(options)
    {
    }
    // Learning
    public DbSet<CauHoi> CauHois { get; set; } = null!;
    public DbSet<DapAnCauHoi> DapAnCauHois { get; set; } = null!;
    public DbSet<NhomCauHoi> NhomCauHois { get; set; } = null!;
    public DbSet<NhomCauHoiChiTiet> NhomCauHoiChiTiets { get; set; } = null!;
    public DbSet<CauHoiDeThi> CauHoiDeThis { get; set; } = null!;
    public DbSet<DeThi> DeThis { get; set; } = null!;
    public DbSet<KyThi> KyThis { get; set; } = null!;
    public DbSet<PhienLamBai> BaiThis { get; set; } = null!;
    public DbSet<CauTraLoi> BaiThiChiTiets { get; set; } = null!;
    public DbSet<DangKyKhoaHoc> DangKys { get; set; } = null!;
    public DbSet<DangKyTuVan> DangKyKhachs { get; set; } = null!;
    public DbSet<ThongBao> ThongBaos { get; set; } = null!;
    public DbSet<PhanHoi> PhanHois { get; set; } = null!;

    // Courses
    public DbSet<KhoaHoc> KhoaHocs { get; set; } = null!;
    public DbSet<LopHoc> LopHocs { get; set; } = null!;
    public DbSet<ThoiGianBieu> ThoiGianBieus { get; set; } = null!;
    public DbSet<LichHoc> LichHocs { get; set; } = null!;
    public DbSet<PhanCong> PhanCongs { get; set; } = null!;

    // Finance
    public DbSet<ThanhToan> ThanhToans { get; set; } = null!;

    // Users
    public DbSet<TaiKhoan> TaiKhoans { get; set; } = null!;
    public DbSet<HocVien> HocViens { get; set; } = null!;
    public DbSet<GiaoVien> GiaoViens { get; set; } = null!;
    public DbSet<GiaoVu> GiaoVus { get; set; } = null!;

    // System
    public DbSet<NhatKyHeThong> NhatKyHeThongs { get; set; } = null!;
    public DbSet<SaoLuuDuLieu> SaoLuuDuLieus { get; set; } = null!;
    public DbSet<CauHinhHeThong> CauHinhHeThongs { get; set; } = null!;

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<CauHoi>().ToTable("CauHoi");
        modelBuilder.Entity<CauHoi>()
            .Navigation(n => n.CacDapAn)
            .AutoInclude();
        modelBuilder.Entity<CauHoi>()
            .Navigation(n => n.CacNhom)
            .AutoInclude();
        modelBuilder.Entity<DapAnCauHoi>().ToTable("DapAnCauHoi");
        modelBuilder.Entity<CauHoiDeThi>().ToTable("CauHoiDeThi");
        modelBuilder.Entity<NhomCauHoi>().ToTable("NhomCauHoi");
        modelBuilder.Entity<NhomCauHoi>()
            .Navigation(nc => nc.CacChiTiet)
            .AutoInclude();
        modelBuilder.Entity<NhomCauHoiChiTiet>().ToTable("NhomCauHoiChiTiet");
        modelBuilder.Entity<DeThi>().ToTable("DeThi");
        modelBuilder.Entity<DeThi>()
            .Navigation(dt => dt.KyThi)
            .AutoInclude();
        modelBuilder.Entity<DeThi>()
            .Navigation(dt => dt.CacCauHoi)
            .AutoInclude();
        modelBuilder.Entity<KyThi>().ToTable("KyThi");
        modelBuilder.Entity<PhienLamBai>().ToTable("PhienLamBai");
        modelBuilder.Entity<PhienLamBai>()
            .Navigation(p => p.DeThi)
            .AutoInclude();
        modelBuilder.Entity<PhienLamBai>()
            .Navigation(p => p.HocVien)
            .AutoInclude();
        modelBuilder.Entity<PhienLamBai>()
            .Navigation(p => p.KyThi)
            .AutoInclude();
        modelBuilder.Entity<PhienLamBai>()
            .Navigation(p => p.CacCauTraLoi)
            .AutoInclude();
        modelBuilder.Entity<CauTraLoi>().ToTable("CauTraLoi");
        modelBuilder.Entity<CauTraLoi>()
            .Navigation(ct => ct.CauHoi)
            .AutoInclude();
        modelBuilder.Entity<DangKyKhoaHoc>().ToTable("DangKyKhoaHoc");
        modelBuilder.Entity<DangKyKhoaHoc>()
            .Navigation(dk => dk.HocVien)
            .AutoInclude();
        modelBuilder.Entity<DangKyKhoaHoc>()
            .Navigation(dk => dk.LopHoc)
            .AutoInclude();
        modelBuilder.Entity<DangKyKhoaHoc>()
            .Navigation(dk => dk.KhoaHoc)
            .AutoInclude();
        modelBuilder.Entity<DangKyTuVan>().ToTable("DangKyTuVan");
        modelBuilder.Entity<ThongBao>().ToTable("ThongBao");
        modelBuilder.Entity<PhanHoi>().ToTable("PhanHoi");
        modelBuilder.Entity<PhanHoi>()
            .Navigation(ph => ph.HocVien)
            .AutoInclude();
        modelBuilder.Entity<KhoaHoc>().ToTable("KhoaHoc");
        modelBuilder.Entity<LopHoc>().ToTable("LopHoc");
        modelBuilder.Entity<LopHoc>()
            .Navigation(lh => lh.CacDangKy)
            .AutoInclude();
        modelBuilder.Entity<LopHoc>()
            .Navigation(lh => lh.KhoaHoc)
            .AutoInclude();
        modelBuilder.Entity<ThoiGianBieu>().ToTable("ThoiGianBieu");
        modelBuilder.Entity<LichHoc>().ToTable("LichHoc");
        modelBuilder.Entity<LichHoc>()
            .Navigation(lh => lh.LopHoc)
            .AutoInclude();
        modelBuilder.Entity<LichHoc>()
            .Navigation(lh => lh.PhongHoc)
            .AutoInclude();
        modelBuilder.Entity<LichHoc>()
            .Navigation(lh => lh.ThoiGianBieu)
            .AutoInclude();
        modelBuilder.Entity<PhanCong>().ToTable("PhanCong");
        modelBuilder.Entity<PhanCong>()
            .Navigation(pc => pc.LopHoc)
            .AutoInclude();
        modelBuilder.Entity<PhanCong>()
            .Navigation(pc => pc.GiaoVien)
            .AutoInclude();
        modelBuilder.Entity<ThanhToan>().ToTable("ThanhToan");

        modelBuilder.Entity<TaiKhoan>().ToTable("TaiKhoan");
        modelBuilder.Entity<HocVien>().ToTable("HocVien");
        modelBuilder.Entity<HocVien>()
            .Navigation(hv => hv.TaiKhoan)
            .AutoInclude();
        modelBuilder.Entity<GiaoVien>().ToTable("GiaoVien");
        modelBuilder.Entity<GiaoVu>().ToTable("GiaoVu");

        modelBuilder.Entity<NhatKyHeThong>().ToTable("NhatKyHeThong");
        modelBuilder.Entity<NhatKyHeThong>()
            .Navigation(nk => nk.TaiKhoan)
            .AutoInclude();
        modelBuilder.Entity<SaoLuuDuLieu>().ToTable("SaoLuuDuLieu");
        modelBuilder.Entity<CauHinhHeThong>().ToTable("CauHinhHeThong");

        modelBuilder.Entity<TaiKhoan>()
            .HasIndex(tk => tk.TenDangNhap)
            .IsUnique();
    }
}
