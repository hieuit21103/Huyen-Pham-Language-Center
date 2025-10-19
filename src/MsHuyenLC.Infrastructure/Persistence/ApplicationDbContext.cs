namespace MsHuyenLC.Infrastructure.Persistence;

public class ApplicationDbContext : DbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
    : base(options)
    {
    }
    // Learning
    public DbSet<CauHoi> CauHois { get; set; } = null!;
    public DbSet<DocHieu> DocHieus { get; set; } = null!;
    public DbSet<CauHoiDeThi> CauHoiDeThis { get; set; } = null!;
    public DbSet<DeThi> DeThis { get; set; } = null!;
    public DbSet<KyThi> KyThis { get; set; } = null!;
    public DbSet<BaiThi> BaiThis { get; set; } = null!;
    public DbSet<BaiThiChiTiet> BaiThiChiTiets { get; set; } = null!;
    public DbSet<DangKy> DangKys { get; set; } = null!;
    public DbSet<DangKyKhach> DangKyKhachs { get; set; } = null!;
    public DbSet<ThongBao> ThongBaos { get; set; } = null!;
    public DbSet<PhanHoi> PhanHois { get; set; } = null!;
    public DbSet<KetQuaHocTap> KetQuaHocTaps { get; set; } = null!;

    // Courses
    public DbSet<KhoaHoc> KhoaHocs { get; set; } = null!;
    public DbSet<LopHoc> LopHocs { get; set; } = null!;
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

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<CauHoi>().ToTable("CauHoi");
        modelBuilder.Entity<CauHoiDeThi>().ToTable("CauHoiDeThi");
        modelBuilder.Entity<DocHieu>().ToTable("DocHieu");
        modelBuilder.Entity<DeThi>().ToTable("DeThi");
        modelBuilder.Entity<KyThi>().ToTable("KyThi");
        modelBuilder.Entity<BaiThi>().ToTable("BaiThi");
        modelBuilder.Entity<BaiThiChiTiet>().ToTable("BaiThiChiTiet");
        modelBuilder.Entity<DangKy>().ToTable("DangKy");
        modelBuilder.Entity<DangKyKhach>().ToTable("DangKyKhach");
        modelBuilder.Entity<ThongBao>().ToTable("ThongBao");
        modelBuilder.Entity<PhanHoi>().ToTable("PhanHoi");
        modelBuilder.Entity<KetQuaHocTap>().ToTable("KetQuaHocTap");

        modelBuilder.Entity<KhoaHoc>().ToTable("KhoaHoc");
        modelBuilder.Entity<LopHoc>().ToTable("LopHoc");
        modelBuilder.Entity<LichHoc>().ToTable("LichHoc");
        modelBuilder.Entity<PhanCong>().ToTable("PhanCong");

        modelBuilder.Entity<ThanhToan>().ToTable("ThanhToan");

        modelBuilder.Entity<TaiKhoan>().ToTable("TaiKhoan");
        modelBuilder.Entity<HocVien>().ToTable("HocVien");
        modelBuilder.Entity<GiaoVien>().ToTable("GiaoVien");
        modelBuilder.Entity<GiaoVu>().ToTable("GiaoVu");

        modelBuilder.Entity<NhatKyHeThong>().ToTable("NhatKyHeThong");
        modelBuilder.Entity<SaoLuuDuLieu>().ToTable("SaoLuuDuLieu");

        modelBuilder.Entity<TaiKhoan>()
            .HasIndex(tk => tk.TenDangNhap)
            .IsUnique();
    }
}
