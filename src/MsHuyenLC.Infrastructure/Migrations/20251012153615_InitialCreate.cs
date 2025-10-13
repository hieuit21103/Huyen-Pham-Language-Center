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
                name: "KhoaHoc",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    TenKhoaHoc = table.Column<string>(type: "text", nullable: false),
                    MoTa = table.Column<string>(type: "text", nullable: true),
                    HocPhi = table.Column<decimal>(type: "numeric", nullable: false),
                    ThoiLuong = table.Column<int>(type: "integer", nullable: false),
                    NgayKhaiGiang = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    TrangThai = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_KhoaHoc", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "NganHangDe",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    NoiDung = table.Column<string>(type: "text", nullable: false),
                    LoaiCauHoi = table.Column<int>(type: "integer", nullable: false),
                    UrlAmThanh = table.Column<string>(type: "text", nullable: true),
                    DapAnDung = table.Column<string>(type: "text", nullable: true),
                    MucDo = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_NganHangDe", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "SaoLuuDuLieu",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    NgaySaoLuu = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
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
                    DatLaiMatKhauToken = table.Column<string>(type: "text", nullable: true),
                    ThoiHanToken = table.Column<string>(type: "text", nullable: true),
                    Avatar = table.Column<string>(type: "text", nullable: true),
                    NgayTao = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
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
                    PhongHoc = table.Column<string>(type: "text", nullable: true),
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
                name: "DangKyKhach",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    HoTen = table.Column<string>(type: "text", nullable: false),
                    Email = table.Column<string>(type: "text", nullable: false),
                    SoDienThoai = table.Column<string>(type: "text", nullable: false),
                    NoiDung = table.Column<string>(type: "text", nullable: true),
                    NgayDangKy = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    TrangThai = table.Column<int>(type: "integer", nullable: false),
                    KetQua = table.Column<int>(type: "integer", nullable: false),
                    NgayXuLy = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    KhoaHocId = table.Column<Guid>(type: "uuid", nullable: false),
                    TaiKhoanXuLyId = table.Column<Guid>(type: "uuid", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DangKyKhach", x => x.Id);
                    table.ForeignKey(
                        name: "FK_DangKyKhach_KhoaHoc_KhoaHocId",
                        column: x => x.KhoaHocId,
                        principalTable: "KhoaHoc",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_DangKyKhach_TaiKhoan_TaiKhoanXuLyId",
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
                    NgaySinh = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    GioiTinh = table.Column<int>(type: "integer", nullable: true),
                    DiaChi = table.Column<string>(type: "text", nullable: true),
                    NgayDangKy = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
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
                    ThoiGian = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
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
                name: "KyThi",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    TenKyThi = table.Column<string>(type: "text", nullable: false),
                    NgayThi = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    ThoiLuong = table.Column<int>(type: "integer", nullable: false),
                    HinhThuc = table.Column<int>(type: "integer", nullable: false),
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
                    IdLopHoc = table.Column<Guid>(type: "uuid", nullable: false),
                    NgayHoc = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    GioBatDau = table.Column<TimeSpan>(type: "interval", nullable: false),
                    GioKetThuc = table.Column<TimeSpan>(type: "interval", nullable: false),
                    PhongHoc = table.Column<string>(type: "text", nullable: true),
                    LopHocId = table.Column<Guid>(type: "uuid", nullable: false)
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
                });

            migrationBuilder.CreateTable(
                name: "ThongBao",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    NguoiGui = table.Column<Guid>(type: "uuid", nullable: false),
                    DoiTuongNhan = table.Column<int>(type: "integer", nullable: false),
                    TieuDe = table.Column<string>(type: "text", nullable: false),
                    NoiDung = table.Column<string>(type: "text", nullable: false),
                    NgayGui = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    NguoiGuiTaiKhoanId = table.Column<Guid>(type: "uuid", nullable: false),
                    LopHocId = table.Column<Guid>(type: "uuid", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ThongBao", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ThongBao_LopHoc_LopHocId",
                        column: x => x.LopHocId,
                        principalTable: "LopHoc",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_ThongBao_TaiKhoan_NguoiGuiTaiKhoanId",
                        column: x => x.NguoiGuiTaiKhoanId,
                        principalTable: "TaiKhoan",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "PhanCong",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    NgayPhanCong = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
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
                name: "DangKy",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    NgayDangKy = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    TrangThai = table.Column<int>(type: "integer", nullable: false),
                    HocVienId = table.Column<Guid>(type: "uuid", nullable: false),
                    LopHocId = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DangKy", x => x.Id);
                    table.ForeignKey(
                        name: "FK_DangKy_HocVien_HocVienId",
                        column: x => x.HocVienId,
                        principalTable: "HocVien",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_DangKy_LopHoc_LopHocId",
                        column: x => x.LopHocId,
                        principalTable: "LopHoc",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "PhanHoi",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    NoiDung = table.Column<string>(type: "text", nullable: false),
                    NgayGui = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    HocVienId = table.Column<Guid>(type: "uuid", nullable: false),
                    GiaoVienId = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PhanHoi", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PhanHoi_GiaoVien_GiaoVienId",
                        column: x => x.GiaoVienId,
                        principalTable: "GiaoVien",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_PhanHoi_HocVien_HocVienId",
                        column: x => x.HocVienId,
                        principalTable: "HocVien",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ThanhToan",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    SoTien = table.Column<decimal>(type: "numeric", nullable: false),
                    PhuongThuc = table.Column<int>(type: "integer", nullable: false),
                    TrangThai = table.Column<int>(type: "integer", nullable: false),
                    NgayLap = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    NgayHetHan = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    NgayThanhToan = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    HocVienId = table.Column<Guid>(type: "uuid", nullable: false),
                    KhoaHocId = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ThanhToan", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ThanhToan_HocVien_HocVienId",
                        column: x => x.HocVienId,
                        principalTable: "HocVien",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ThanhToan_KhoaHoc_KhoaHocId",
                        column: x => x.KhoaHocId,
                        principalTable: "KhoaHoc",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "DeThi",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    TenDe = table.Column<string>(type: "text", nullable: false),
                    SoCauHoi = table.Column<int>(type: "integer", nullable: false),
                    KyThiId = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DeThi", x => x.Id);
                    table.ForeignKey(
                        name: "FK_DeThi_KyThi_KyThiId",
                        column: x => x.KyThiId,
                        principalTable: "KyThi",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "KetQuaHocTap",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    DiemSo = table.Column<decimal>(type: "numeric", nullable: false),
                    NgayThi = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    HocVienId = table.Column<Guid>(type: "uuid", nullable: false),
                    KyThiId = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_KetQuaHocTap", x => x.Id);
                    table.ForeignKey(
                        name: "FK_KetQuaHocTap_HocVien_HocVienId",
                        column: x => x.HocVienId,
                        principalTable: "HocVien",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_KetQuaHocTap_KyThi_KyThiId",
                        column: x => x.KyThiId,
                        principalTable: "KyThi",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "BaiThi",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    DiemTracNghiem = table.Column<float>(type: "real", nullable: true),
                    DiemTuLuan = table.Column<float>(type: "real", nullable: true),
                    TongDiem = table.Column<float>(type: "real", nullable: true),
                    NhanXet = table.Column<string>(type: "text", nullable: true),
                    NgayNop = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    DeThiId = table.Column<Guid>(type: "uuid", nullable: false),
                    HocVienId = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BaiThi", x => x.Id);
                    table.ForeignKey(
                        name: "FK_BaiThi_DeThi_DeThiId",
                        column: x => x.DeThiId,
                        principalTable: "DeThi",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_BaiThi_HocVien_HocVienId",
                        column: x => x.HocVienId,
                        principalTable: "HocVien",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "DeThiCauHoi",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    DeThiId = table.Column<Guid>(type: "uuid", nullable: false),
                    CauHoiId = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DeThiCauHoi", x => x.Id);
                    table.ForeignKey(
                        name: "FK_DeThiCauHoi_DeThi_DeThiId",
                        column: x => x.DeThiId,
                        principalTable: "DeThi",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_DeThiCauHoi_NganHangDe_CauHoiId",
                        column: x => x.CauHoiId,
                        principalTable: "NganHangDe",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "BaiThiChiTiet",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    BaiThiId = table.Column<Guid>(type: "uuid", nullable: false),
                    CauHoiId = table.Column<Guid>(type: "uuid", nullable: false),
                    CauTraLoi = table.Column<string>(type: "text", nullable: true),
                    Diem = table.Column<float>(type: "real", nullable: true),
                    NhanXet = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BaiThiChiTiet", x => x.Id);
                    table.ForeignKey(
                        name: "FK_BaiThiChiTiet_BaiThi_BaiThiId",
                        column: x => x.BaiThiId,
                        principalTable: "BaiThi",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_BaiThiChiTiet_DeThiCauHoi_CauHoiId",
                        column: x => x.CauHoiId,
                        principalTable: "DeThiCauHoi",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_BaiThi_DeThiId",
                table: "BaiThi",
                column: "DeThiId");

            migrationBuilder.CreateIndex(
                name: "IX_BaiThi_HocVienId",
                table: "BaiThi",
                column: "HocVienId");

            migrationBuilder.CreateIndex(
                name: "IX_BaiThiChiTiet_BaiThiId",
                table: "BaiThiChiTiet",
                column: "BaiThiId");

            migrationBuilder.CreateIndex(
                name: "IX_BaiThiChiTiet_CauHoiId",
                table: "BaiThiChiTiet",
                column: "CauHoiId");

            migrationBuilder.CreateIndex(
                name: "IX_DangKy_HocVienId",
                table: "DangKy",
                column: "HocVienId");

            migrationBuilder.CreateIndex(
                name: "IX_DangKy_LopHocId",
                table: "DangKy",
                column: "LopHocId");

            migrationBuilder.CreateIndex(
                name: "IX_DangKyKhach_KhoaHocId",
                table: "DangKyKhach",
                column: "KhoaHocId");

            migrationBuilder.CreateIndex(
                name: "IX_DangKyKhach_TaiKhoanXuLyId",
                table: "DangKyKhach",
                column: "TaiKhoanXuLyId");

            migrationBuilder.CreateIndex(
                name: "IX_DeThi_KyThiId",
                table: "DeThi",
                column: "KyThiId");

            migrationBuilder.CreateIndex(
                name: "IX_DeThiCauHoi_CauHoiId",
                table: "DeThiCauHoi",
                column: "CauHoiId");

            migrationBuilder.CreateIndex(
                name: "IX_DeThiCauHoi_DeThiId",
                table: "DeThiCauHoi",
                column: "DeThiId");

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
                name: "IX_KetQuaHocTap_HocVienId",
                table: "KetQuaHocTap",
                column: "HocVienId");

            migrationBuilder.CreateIndex(
                name: "IX_KetQuaHocTap_KyThiId",
                table: "KetQuaHocTap",
                column: "KyThiId");

            migrationBuilder.CreateIndex(
                name: "IX_KyThi_LopHocId",
                table: "KyThi",
                column: "LopHocId");

            migrationBuilder.CreateIndex(
                name: "IX_LichHoc_LopHocId",
                table: "LichHoc",
                column: "LopHocId");

            migrationBuilder.CreateIndex(
                name: "IX_LopHoc_KhoaHocId",
                table: "LopHoc",
                column: "KhoaHocId");

            migrationBuilder.CreateIndex(
                name: "IX_NhatKyHeThong_TaiKhoanId",
                table: "NhatKyHeThong",
                column: "TaiKhoanId");

            migrationBuilder.CreateIndex(
                name: "IX_PhanCong_GiaoVienId",
                table: "PhanCong",
                column: "GiaoVienId");

            migrationBuilder.CreateIndex(
                name: "IX_PhanCong_LopHocId",
                table: "PhanCong",
                column: "LopHocId");

            migrationBuilder.CreateIndex(
                name: "IX_PhanHoi_GiaoVienId",
                table: "PhanHoi",
                column: "GiaoVienId");

            migrationBuilder.CreateIndex(
                name: "IX_PhanHoi_HocVienId",
                table: "PhanHoi",
                column: "HocVienId");

            migrationBuilder.CreateIndex(
                name: "IX_TaiKhoan_TenDangNhap",
                table: "TaiKhoan",
                column: "TenDangNhap",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_ThanhToan_HocVienId",
                table: "ThanhToan",
                column: "HocVienId");

            migrationBuilder.CreateIndex(
                name: "IX_ThanhToan_KhoaHocId",
                table: "ThanhToan",
                column: "KhoaHocId");

            migrationBuilder.CreateIndex(
                name: "IX_ThongBao_LopHocId",
                table: "ThongBao",
                column: "LopHocId");

            migrationBuilder.CreateIndex(
                name: "IX_ThongBao_NguoiGuiTaiKhoanId",
                table: "ThongBao",
                column: "NguoiGuiTaiKhoanId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "BaiThiChiTiet");

            migrationBuilder.DropTable(
                name: "DangKy");

            migrationBuilder.DropTable(
                name: "DangKyKhach");

            migrationBuilder.DropTable(
                name: "GiaoVu");

            migrationBuilder.DropTable(
                name: "KetQuaHocTap");

            migrationBuilder.DropTable(
                name: "LichHoc");

            migrationBuilder.DropTable(
                name: "NhatKyHeThong");

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
                name: "BaiThi");

            migrationBuilder.DropTable(
                name: "DeThiCauHoi");

            migrationBuilder.DropTable(
                name: "GiaoVien");

            migrationBuilder.DropTable(
                name: "HocVien");

            migrationBuilder.DropTable(
                name: "DeThi");

            migrationBuilder.DropTable(
                name: "NganHangDe");

            migrationBuilder.DropTable(
                name: "TaiKhoan");

            migrationBuilder.DropTable(
                name: "KyThi");

            migrationBuilder.DropTable(
                name: "LopHoc");

            migrationBuilder.DropTable(
                name: "KhoaHoc");
        }
    }
}
