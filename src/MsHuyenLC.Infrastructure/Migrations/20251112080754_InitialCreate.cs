using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MsHuyenLC.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "CauHinhHeThong",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Ten = table.Column<string>(type: "text", nullable: false),
                    GiaTri = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CauHinhHeThong", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "KhoaHoc",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    TenKhoaHoc = table.Column<string>(type: "text", nullable: false),
                    MoTa = table.Column<string>(type: "text", nullable: true),
                    HocPhi = table.Column<decimal>(type: "numeric", nullable: false),
                    ThoiLuong = table.Column<int>(type: "integer", nullable: false),
                    NgayKhaiGiang = table.Column<DateOnly>(type: "date", nullable: false),
                    TrangThai = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_KhoaHoc", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "NganHangCauHoi",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    NoiDungCauHoi = table.Column<string>(type: "text", nullable: false),
                    LoaiCauHoi = table.Column<int>(type: "integer", nullable: false),
                    KyNang = table.Column<int>(type: "integer", nullable: false),
                    DoKho = table.Column<int>(type: "integer", nullable: false),
                    CapDo = table.Column<int>(type: "integer", nullable: false),
                    UrlAmThanh = table.Column<string>(type: "text", nullable: true),
                    UrlHinhAnh = table.Column<string>(type: "text", nullable: true),
                    LoiThoai = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_NganHangCauHoi", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "NhomCauHoi",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    UrlAmThanh = table.Column<string>(type: "text", nullable: true),
                    UrlHinhAnh = table.Column<string>(type: "text", nullable: true),
                    NoiDung = table.Column<string>(type: "text", nullable: true),
                    TieuDe = table.Column<string>(type: "text", nullable: false),
                    SoLuongCauHoi = table.Column<int>(type: "integer", nullable: false),
                    DoKho = table.Column<int>(type: "integer", nullable: false),
                    CapDo = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_NhomCauHoi", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "PhongHoc",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    TenPhong = table.Column<string>(type: "text", nullable: false),
                    SoGhe = table.Column<int>(type: "integer", nullable: false),
                    NgayTao = table.Column<DateOnly>(type: "date", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PhongHoc", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "SaoLuuDuLieu",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    NgaySaoLuu = table.Column<DateOnly>(type: "date", nullable: false),
                    DuongDan = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SaoLuuDuLieu", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "TaiKhoan",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    TenDangNhap = table.Column<string>(type: "text", nullable: false),
                    MatKhau = table.Column<string>(type: "text", nullable: false),
                    VaiTro = table.Column<int>(type: "integer", nullable: false),
                    Email = table.Column<string>(type: "text", nullable: true),
                    Sdt = table.Column<string>(type: "text", nullable: true),
                    TrangThai = table.Column<int>(type: "integer", nullable: false),
                    Avatar = table.Column<string>(type: "text", nullable: true),
                    NgayTao = table.Column<DateOnly>(type: "date", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TaiKhoan", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "LopHoc",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    TenLop = table.Column<string>(type: "text", nullable: false),
                    SiSoHienTai = table.Column<int>(type: "integer", nullable: false),
                    SiSoToiDa = table.Column<int>(type: "integer", nullable: false),
                    TrangThai = table.Column<int>(type: "integer", nullable: false),
                    KhoaHocId = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LopHoc", x => x.Id);
                    table.ForeignKey(
                        name: "FK_LopHoc_KhoaHoc_KhoaHocId",
                        column: x => x.KhoaHocId,
                        principalTable: "KhoaHoc",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "DapAnCauHoi",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    CauHoiId = table.Column<Guid>(type: "uuid", nullable: false),
                    Nhan = table.Column<string>(type: "text", nullable: false),
                    NoiDung = table.Column<string>(type: "text", nullable: false),
                    Dung = table.Column<bool>(type: "boolean", nullable: false),
                    GiaiThich = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DapAnCauHoi", x => x.Id);
                    table.ForeignKey(
                        name: "FK_DapAnCauHoi_NganHangCauHoi_CauHoiId",
                        column: x => x.CauHoiId,
                        principalTable: "NganHangCauHoi",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "NhomCauHoiChiTiet",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    NhomId = table.Column<Guid>(type: "uuid", nullable: false),
                    CauHoiId = table.Column<Guid>(type: "uuid", nullable: false),
                    ThuTu = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_NhomCauHoiChiTiet", x => x.Id);
                    table.ForeignKey(
                        name: "FK_NhomCauHoiChiTiet_NganHangCauHoi_CauHoiId",
                        column: x => x.CauHoiId,
                        principalTable: "NganHangCauHoi",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_NhomCauHoiChiTiet_NhomCauHoi_NhomId",
                        column: x => x.NhomId,
                        principalTable: "NhomCauHoi",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "DangKyTuVan",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    HoTen = table.Column<string>(type: "text", nullable: false),
                    GioiTinh = table.Column<int>(type: "integer", nullable: false),
                    Email = table.Column<string>(type: "text", nullable: false),
                    SoDienThoai = table.Column<string>(type: "text", nullable: false),
                    NoiDung = table.Column<string>(type: "text", nullable: true),
                    NgayDangKy = table.Column<DateOnly>(type: "date", nullable: false),
                    TrangThai = table.Column<int>(type: "integer", nullable: false),
                    KetQua = table.Column<int>(type: "integer", nullable: false),
                    NgayXuLy = table.Column<DateOnly>(type: "date", nullable: true),
                    KhoaHocId = table.Column<Guid>(type: "uuid", nullable: false),
                    TaiKhoanXuLyId = table.Column<Guid>(type: "uuid", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DangKyTuVan", x => x.Id);
                    table.ForeignKey(
                        name: "FK_DangKyTuVan_KhoaHoc_KhoaHocId",
                        column: x => x.KhoaHocId,
                        principalTable: "KhoaHoc",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_DangKyTuVan_TaiKhoan_TaiKhoanXuLyId",
                        column: x => x.TaiKhoanXuLyId,
                        principalTable: "TaiKhoan",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "GiaoVien",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    HoTen = table.Column<string>(type: "text", nullable: false),
                    ChuyenMon = table.Column<string>(type: "text", nullable: true),
                    TrinhDo = table.Column<string>(type: "text", nullable: true),
                    KinhNghiem = table.Column<string>(type: "text", nullable: true),
                    TaiKhoanId = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_GiaoVien", x => x.Id);
                    table.ForeignKey(
                        name: "FK_GiaoVien_TaiKhoan_TaiKhoanId",
                        column: x => x.TaiKhoanId,
                        principalTable: "TaiKhoan",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "GiaoVu",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    HoTen = table.Column<string>(type: "text", nullable: false),
                    BoPhan = table.Column<string>(type: "text", nullable: true),
                    TaiKhoanId = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_GiaoVu", x => x.Id);
                    table.ForeignKey(
                        name: "FK_GiaoVu_TaiKhoan_TaiKhoanId",
                        column: x => x.TaiKhoanId,
                        principalTable: "TaiKhoan",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "HocVien",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    HoTen = table.Column<string>(type: "text", nullable: false),
                    NgaySinh = table.Column<DateOnly>(type: "date", nullable: true),
                    GioiTinh = table.Column<int>(type: "integer", nullable: true),
                    DiaChi = table.Column<string>(type: "text", nullable: true),
                    NgayDangKy = table.Column<DateOnly>(type: "date", nullable: false),
                    TrangThai = table.Column<int>(type: "integer", nullable: false),
                    TaiKhoanId = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_HocVien", x => x.Id);
                    table.ForeignKey(
                        name: "FK_HocVien_TaiKhoan_TaiKhoanId",
                        column: x => x.TaiKhoanId,
                        principalTable: "TaiKhoan",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "NhatKyHeThong",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    ThoiGian = table.Column<DateOnly>(type: "date", nullable: false),
                    HanhDong = table.Column<string>(type: "text", nullable: false),
                    ChiTiet = table.Column<string>(type: "text", nullable: true),
                    DuLieuCu = table.Column<string>(type: "text", nullable: false),
                    DuLieuMoi = table.Column<string>(type: "text", nullable: false),
                    IP = table.Column<string>(type: "text", nullable: false),
                    TaiKhoanId = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_NhatKyHeThong", x => x.Id);
                    table.ForeignKey(
                        name: "FK_NhatKyHeThong_TaiKhoan_TaiKhoanId",
                        column: x => x.TaiKhoanId,
                        principalTable: "TaiKhoan",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ThongBao",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    TieuDe = table.Column<string>(type: "text", nullable: false),
                    NoiDung = table.Column<string>(type: "text", nullable: false),
                    NguoiGuiId = table.Column<Guid>(type: "uuid", nullable: false),
                    NguoiNhanId = table.Column<Guid>(type: "uuid", nullable: true),
                    NgayTao = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ThongBao", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ThongBao_TaiKhoan_NguoiGuiId",
                        column: x => x.NguoiGuiId,
                        principalTable: "TaiKhoan",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ThongBao_TaiKhoan_NguoiNhanId",
                        column: x => x.NguoiNhanId,
                        principalTable: "TaiKhoan",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "KyThi",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    TenKyThi = table.Column<string>(type: "text", nullable: false),
                    NgayThi = table.Column<DateOnly>(type: "date", nullable: false),
                    GioBatDau = table.Column<TimeOnly>(type: "time without time zone", nullable: false),
                    GioKetThuc = table.Column<TimeOnly>(type: "time without time zone", nullable: false),
                    ThoiLuong = table.Column<int>(type: "integer", nullable: false),
                    TrangThai = table.Column<int>(type: "integer", nullable: false),
                    LopHocId = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_KyThi", x => x.Id);
                    table.ForeignKey(
                        name: "FK_KyThi_LopHoc_LopHocId",
                        column: x => x.LopHocId,
                        principalTable: "LopHoc",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "LichHoc",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Thu = table.Column<int>(type: "integer", nullable: false),
                    GioBatDau = table.Column<TimeOnly>(type: "time without time zone", nullable: false),
                    GioKetThuc = table.Column<TimeOnly>(type: "time without time zone", nullable: false),
                    TuNgay = table.Column<DateOnly>(type: "date", nullable: false),
                    DenNgay = table.Column<DateOnly>(type: "date", nullable: false),
                    CoHieuLuc = table.Column<bool>(type: "boolean", nullable: false),
                    LopHocId = table.Column<Guid>(type: "uuid", nullable: false),
                    PhongHocId = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LichHoc", x => x.Id);
                    table.ForeignKey(
                        name: "FK_LichHoc_LopHoc_LopHocId",
                        column: x => x.LopHocId,
                        principalTable: "LopHoc",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_LichHoc_PhongHoc_PhongHocId",
                        column: x => x.PhongHocId,
                        principalTable: "PhongHoc",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "PhanCong",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    NgayPhanCong = table.Column<DateOnly>(type: "date", nullable: false),
                    LopHocId = table.Column<Guid>(type: "uuid", nullable: false),
                    GiaoVienId = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PhanCong", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PhanCong_GiaoVien_GiaoVienId",
                        column: x => x.GiaoVienId,
                        principalTable: "GiaoVien",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_PhanCong_LopHoc_LopHocId",
                        column: x => x.LopHocId,
                        principalTable: "LopHoc",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "DangKyKhoaHoc",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    NgayDangKy = table.Column<DateOnly>(type: "date", nullable: false),
                    TrangThai = table.Column<int>(type: "integer", nullable: false),
                    HocVienId = table.Column<Guid>(type: "uuid", nullable: false),
                    KhoaHocId = table.Column<Guid>(type: "uuid", nullable: false),
                    LopHocId = table.Column<Guid>(type: "uuid", nullable: true),
                    NgayXepLop = table.Column<DateOnly>(type: "date", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DangKyKhoaHoc", x => x.Id);
                    table.ForeignKey(
                        name: "FK_DangKyKhoaHoc_HocVien_HocVienId",
                        column: x => x.HocVienId,
                        principalTable: "HocVien",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_DangKyKhoaHoc_KhoaHoc_KhoaHocId",
                        column: x => x.KhoaHocId,
                        principalTable: "KhoaHoc",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_DangKyKhoaHoc_LopHoc_LopHocId",
                        column: x => x.LopHocId,
                        principalTable: "LopHoc",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "PhanHoi",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    LoaiPhanHoi = table.Column<string>(type: "text", nullable: false),
                    TieuDe = table.Column<string>(type: "text", nullable: false),
                    NoiDung = table.Column<string>(type: "text", nullable: false),
                    HocVienId = table.Column<Guid>(type: "uuid", nullable: false),
                    NgayTao = table.Column<DateOnly>(type: "date", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PhanHoi", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PhanHoi_HocVien_HocVienId",
                        column: x => x.HocVienId,
                        principalTable: "HocVien",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "DeThi",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    MaDe = table.Column<string>(type: "text", nullable: false),
                    TenDe = table.Column<string>(type: "text", nullable: false),
                    LoaiDeThi = table.Column<int>(type: "integer", nullable: false),
                    NgayTao = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    NguoiTaoId = table.Column<Guid>(type: "uuid", nullable: false),
                    TongCauHoi = table.Column<int>(type: "integer", nullable: false),
                    ThoiLuongPhut = table.Column<int>(type: "integer", nullable: false),
                    KyThiId = table.Column<Guid>(type: "uuid", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DeThi", x => x.Id);
                    table.ForeignKey(
                        name: "FK_DeThi_KyThi_KyThiId",
                        column: x => x.KyThiId,
                        principalTable: "KyThi",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_DeThi_TaiKhoan_NguoiTaoId",
                        column: x => x.NguoiTaoId,
                        principalTable: "TaiKhoan",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ThanhToan",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    MaThanhToan = table.Column<string>(type: "text", nullable: true),
                    SoTien = table.Column<decimal>(type: "numeric", nullable: false),
                    PhuongThuc = table.Column<int>(type: "integer", nullable: false),
                    TrangThai = table.Column<int>(type: "integer", nullable: false),
                    NgayLap = table.Column<DateOnly>(type: "date", nullable: false),
                    NgayHetHan = table.Column<DateOnly>(type: "date", nullable: false),
                    NgayThanhToan = table.Column<DateOnly>(type: "date", nullable: true),
                    ThongTinNganHang = table.Column<string>(type: "text", nullable: true),
                    MaGiaoDichNganHang = table.Column<string>(type: "text", nullable: true),
                    CongThanhToan = table.Column<string>(type: "text", nullable: true),
                    DangKyId = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ThanhToan", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ThanhToan_DangKyKhoaHoc_DangKyId",
                        column: x => x.DangKyId,
                        principalTable: "DangKyKhoaHoc",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "CauHoiDeThi",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    DeThiId = table.Column<Guid>(type: "uuid", nullable: false),
                    CauHoiId = table.Column<Guid>(type: "uuid", nullable: false),
                    ThuTuCauHoi = table.Column<int>(type: "integer", nullable: false),
                    Diem = table.Column<decimal>(type: "numeric", nullable: false),
                    NhomCauHoiId = table.Column<Guid>(type: "uuid", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CauHoiDeThi", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CauHoiDeThi_DeThi_DeThiId",
                        column: x => x.DeThiId,
                        principalTable: "DeThi",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_CauHoiDeThi_NganHangCauHoi_CauHoiId",
                        column: x => x.CauHoiId,
                        principalTable: "NganHangCauHoi",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_CauHoiDeThi_NhomCauHoi_NhomCauHoiId",
                        column: x => x.NhomCauHoiId,
                        principalTable: "NhomCauHoi",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "PhienLamBai",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    TongCauHoi = table.Column<int>(type: "integer", nullable: false),
                    SoCauDung = table.Column<int>(type: "integer", nullable: true),
                    Diem = table.Column<decimal>(type: "numeric", nullable: true),
                    ThoiGianLam = table.Column<TimeSpan>(type: "interval", nullable: false),
                    NgayLam = table.Column<DateOnly>(type: "date", nullable: false),
                    HocVienId = table.Column<Guid>(type: "uuid", nullable: false),
                    DeThiId = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PhienLamBai", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PhienLamBai_DeThi_DeThiId",
                        column: x => x.DeThiId,
                        principalTable: "DeThi",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_PhienLamBai_HocVien_HocVienId",
                        column: x => x.HocVienId,
                        principalTable: "HocVien",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "CauTraLoi",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    PhienId = table.Column<Guid>(type: "uuid", nullable: false),
                    CauHoiId = table.Column<Guid>(type: "uuid", nullable: false),
                    CauTraLoiText = table.Column<string>(type: "text", nullable: true),
                    Dung = table.Column<bool>(type: "boolean", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CauTraLoi", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CauTraLoi_NganHangCauHoi_CauHoiId",
                        column: x => x.CauHoiId,
                        principalTable: "NganHangCauHoi",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_CauTraLoi_PhienLamBai_PhienId",
                        column: x => x.PhienId,
                        principalTable: "PhienLamBai",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_CauHoiDeThi_CauHoiId",
                table: "CauHoiDeThi",
                column: "CauHoiId");

            migrationBuilder.CreateIndex(
                name: "IX_CauHoiDeThi_DeThiId",
                table: "CauHoiDeThi",
                column: "DeThiId");

            migrationBuilder.CreateIndex(
                name: "IX_CauHoiDeThi_NhomCauHoiId",
                table: "CauHoiDeThi",
                column: "NhomCauHoiId");

            migrationBuilder.CreateIndex(
                name: "IX_CauTraLoi_CauHoiId",
                table: "CauTraLoi",
                column: "CauHoiId");

            migrationBuilder.CreateIndex(
                name: "IX_CauTraLoi_PhienId",
                table: "CauTraLoi",
                column: "PhienId");

            migrationBuilder.CreateIndex(
                name: "IX_DangKyKhoaHoc_HocVienId",
                table: "DangKyKhoaHoc",
                column: "HocVienId");

            migrationBuilder.CreateIndex(
                name: "IX_DangKyKhoaHoc_KhoaHocId",
                table: "DangKyKhoaHoc",
                column: "KhoaHocId");

            migrationBuilder.CreateIndex(
                name: "IX_DangKyKhoaHoc_LopHocId",
                table: "DangKyKhoaHoc",
                column: "LopHocId");

            migrationBuilder.CreateIndex(
                name: "IX_DangKyTuVan_KhoaHocId",
                table: "DangKyTuVan",
                column: "KhoaHocId");

            migrationBuilder.CreateIndex(
                name: "IX_DangKyTuVan_TaiKhoanXuLyId",
                table: "DangKyTuVan",
                column: "TaiKhoanXuLyId");

            migrationBuilder.CreateIndex(
                name: "IX_DapAnCauHoi_CauHoiId",
                table: "DapAnCauHoi",
                column: "CauHoiId");

            migrationBuilder.CreateIndex(
                name: "IX_DeThi_KyThiId",
                table: "DeThi",
                column: "KyThiId");

            migrationBuilder.CreateIndex(
                name: "IX_DeThi_NguoiTaoId",
                table: "DeThi",
                column: "NguoiTaoId");

            migrationBuilder.CreateIndex(
                name: "IX_GiaoVien_TaiKhoanId",
                table: "GiaoVien",
                column: "TaiKhoanId");

            migrationBuilder.CreateIndex(
                name: "IX_GiaoVu_TaiKhoanId",
                table: "GiaoVu",
                column: "TaiKhoanId");

            migrationBuilder.CreateIndex(
                name: "IX_HocVien_TaiKhoanId",
                table: "HocVien",
                column: "TaiKhoanId");

            migrationBuilder.CreateIndex(
                name: "IX_KyThi_LopHocId",
                table: "KyThi",
                column: "LopHocId");

            migrationBuilder.CreateIndex(
                name: "IX_LichHoc_LopHocId",
                table: "LichHoc",
                column: "LopHocId");

            migrationBuilder.CreateIndex(
                name: "IX_LichHoc_PhongHocId",
                table: "LichHoc",
                column: "PhongHocId");

            migrationBuilder.CreateIndex(
                name: "IX_LopHoc_KhoaHocId",
                table: "LopHoc",
                column: "KhoaHocId");

            migrationBuilder.CreateIndex(
                name: "IX_NhatKyHeThong_TaiKhoanId",
                table: "NhatKyHeThong",
                column: "TaiKhoanId");

            migrationBuilder.CreateIndex(
                name: "IX_NhomCauHoiChiTiet_CauHoiId",
                table: "NhomCauHoiChiTiet",
                column: "CauHoiId");

            migrationBuilder.CreateIndex(
                name: "IX_NhomCauHoiChiTiet_NhomId",
                table: "NhomCauHoiChiTiet",
                column: "NhomId");

            migrationBuilder.CreateIndex(
                name: "IX_PhanCong_GiaoVienId",
                table: "PhanCong",
                column: "GiaoVienId");

            migrationBuilder.CreateIndex(
                name: "IX_PhanCong_LopHocId",
                table: "PhanCong",
                column: "LopHocId");

            migrationBuilder.CreateIndex(
                name: "IX_PhanHoi_HocVienId",
                table: "PhanHoi",
                column: "HocVienId");

            migrationBuilder.CreateIndex(
                name: "IX_PhienLamBai_DeThiId",
                table: "PhienLamBai",
                column: "DeThiId");

            migrationBuilder.CreateIndex(
                name: "IX_PhienLamBai_HocVienId",
                table: "PhienLamBai",
                column: "HocVienId");

            migrationBuilder.CreateIndex(
                name: "IX_TaiKhoan_TenDangNhap",
                table: "TaiKhoan",
                column: "TenDangNhap",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_ThanhToan_DangKyId",
                table: "ThanhToan",
                column: "DangKyId");

            migrationBuilder.CreateIndex(
                name: "IX_ThongBao_NguoiGuiId",
                table: "ThongBao",
                column: "NguoiGuiId");

            migrationBuilder.CreateIndex(
                name: "IX_ThongBao_NguoiNhanId",
                table: "ThongBao",
                column: "NguoiNhanId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "CauHinhHeThong");

            migrationBuilder.DropTable(
                name: "CauHoiDeThi");

            migrationBuilder.DropTable(
                name: "CauTraLoi");

            migrationBuilder.DropTable(
                name: "DangKyTuVan");

            migrationBuilder.DropTable(
                name: "DapAnCauHoi");

            migrationBuilder.DropTable(
                name: "GiaoVu");

            migrationBuilder.DropTable(
                name: "LichHoc");

            migrationBuilder.DropTable(
                name: "NhatKyHeThong");

            migrationBuilder.DropTable(
                name: "NhomCauHoiChiTiet");

            migrationBuilder.DropTable(
                name: "PhanCong");

            migrationBuilder.DropTable(
                name: "PhanHoi");

            migrationBuilder.DropTable(
                name: "SaoLuuDuLieu");

            migrationBuilder.DropTable(
                name: "ThanhToan");

            migrationBuilder.DropTable(
                name: "ThongBao");

            migrationBuilder.DropTable(
                name: "PhienLamBai");

            migrationBuilder.DropTable(
                name: "PhongHoc");

            migrationBuilder.DropTable(
                name: "NganHangCauHoi");

            migrationBuilder.DropTable(
                name: "NhomCauHoi");

            migrationBuilder.DropTable(
                name: "GiaoVien");

            migrationBuilder.DropTable(
                name: "DangKyKhoaHoc");

            migrationBuilder.DropTable(
                name: "DeThi");

            migrationBuilder.DropTable(
                name: "HocVien");

            migrationBuilder.DropTable(
                name: "KyThi");

            migrationBuilder.DropTable(
                name: "TaiKhoan");

            migrationBuilder.DropTable(
                name: "LopHoc");

            migrationBuilder.DropTable(
                name: "KhoaHoc");
        }
    }
}
