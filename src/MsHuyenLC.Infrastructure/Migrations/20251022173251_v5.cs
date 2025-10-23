using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MsHuyenLC.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class v5 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_CauHoiDeThi_CauHoi_CauHoiId",
                table: "CauHoiDeThi");

            migrationBuilder.DropTable(
                name: "BaiThiChiTiet");

            migrationBuilder.DropTable(
                name: "BaiThi");

            migrationBuilder.DropTable(
                name: "CauHoi");

            migrationBuilder.DropTable(
                name: "DocHieu");

            migrationBuilder.RenameColumn(
                name: "ThoiGianLamBai",
                table: "DeThi",
                newName: "TongCauHoi");

            migrationBuilder.RenameColumn(
                name: "SoCauHoi",
                table: "DeThi",
                newName: "ThoiLuongPhut");

            migrationBuilder.AddColumn<string>(
                name: "MaDe",
                table: "DeThi",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<DateTime>(
                name: "NgayTao",
                table: "DeThi",
                type: "timestamp with time zone",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<Guid>(
                name: "NguoiTaoId",
                table: "DeThi",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddColumn<int>(
                name: "GioiTinh",
                table: "DangKyKhach",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<decimal>(
                name: "Diem",
                table: "CauHoiDeThi",
                type: "numeric",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<Guid>(
                name: "NhomCauHoiId",
                table: "CauHoiDeThi",
                type: "uuid",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "ThuTuCauHoi",
                table: "CauHoiDeThi",
                type: "integer",
                nullable: false,
                defaultValue: 0);

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
                    LoiThoai = table.Column<string>(type: "text", nullable: true),
                    DoanVan = table.Column<string>(type: "text", nullable: true)
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
                    NoiDung = table.Column<string>(type: "text", nullable: true),
                    TieuDe = table.Column<string>(type: "text", nullable: true),
                    SoLuongCauHoi = table.Column<int>(type: "integer", nullable: false),
                    DoKho = table.Column<int>(type: "integer", nullable: false),
                    CapDo = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_NhomCauHoi", x => x.Id);
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
                        name: "FK_PhienLamBai_TaiKhoan_HocVienId",
                        column: x => x.HocVienId,
                        principalTable: "TaiKhoan",
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

            migrationBuilder.CreateIndex(
                name: "IX_DeThi_NguoiTaoId",
                table: "DeThi",
                column: "NguoiTaoId");

            migrationBuilder.CreateIndex(
                name: "IX_CauHoiDeThi_NhomCauHoiId",
                table: "CauHoiDeThi",
                column: "NhomCauHoiId");

            migrationBuilder.CreateIndex(
                name: "IX_CauTraLoi_CauHoiId",
                table: "CauTraLoi",
                column: "CauHoiId");

            migrationBuilder.CreateIndex(
                name: "IX_DapAnCauHoi_CauHoiId",
                table: "DapAnCauHoi",
                column: "CauHoiId");

            migrationBuilder.CreateIndex(
                name: "IX_NhomCauHoiChiTiet_CauHoiId",
                table: "NhomCauHoiChiTiet",
                column: "CauHoiId");

            migrationBuilder.CreateIndex(
                name: "IX_NhomCauHoiChiTiet_NhomId",
                table: "NhomCauHoiChiTiet",
                column: "NhomId");

            migrationBuilder.CreateIndex(
                name: "IX_PhienLamBai_DeThiId",
                table: "PhienLamBai",
                column: "DeThiId");

            migrationBuilder.CreateIndex(
                name: "IX_PhienLamBai_HocVienId",
                table: "PhienLamBai",
                column: "HocVienId");

            migrationBuilder.AddForeignKey(
                name: "FK_CauHoiDeThi_NganHangCauHoi_CauHoiId",
                table: "CauHoiDeThi",
                column: "CauHoiId",
                principalTable: "NganHangCauHoi",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_CauHoiDeThi_NhomCauHoi_NhomCauHoiId",
                table: "CauHoiDeThi",
                column: "NhomCauHoiId",
                principalTable: "NhomCauHoi",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_DeThi_TaiKhoan_NguoiTaoId",
                table: "DeThi",
                column: "NguoiTaoId",
                principalTable: "TaiKhoan",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_CauHoiDeThi_NganHangCauHoi_CauHoiId",
                table: "CauHoiDeThi");

            migrationBuilder.DropForeignKey(
                name: "FK_CauHoiDeThi_NhomCauHoi_NhomCauHoiId",
                table: "CauHoiDeThi");

            migrationBuilder.DropForeignKey(
                name: "FK_DeThi_TaiKhoan_NguoiTaoId",
                table: "DeThi");

            migrationBuilder.DropTable(
                name: "CauTraLoi");

            migrationBuilder.DropTable(
                name: "DapAnCauHoi");

            migrationBuilder.DropTable(
                name: "NhomCauHoiChiTiet");

            migrationBuilder.DropTable(
                name: "PhienLamBai");

            migrationBuilder.DropTable(
                name: "NganHangCauHoi");

            migrationBuilder.DropTable(
                name: "NhomCauHoi");

            migrationBuilder.DropIndex(
                name: "IX_DeThi_NguoiTaoId",
                table: "DeThi");

            migrationBuilder.DropIndex(
                name: "IX_CauHoiDeThi_NhomCauHoiId",
                table: "CauHoiDeThi");

            migrationBuilder.DropColumn(
                name: "MaDe",
                table: "DeThi");

            migrationBuilder.DropColumn(
                name: "NgayTao",
                table: "DeThi");

            migrationBuilder.DropColumn(
                name: "NguoiTaoId",
                table: "DeThi");

            migrationBuilder.DropColumn(
                name: "GioiTinh",
                table: "DangKyKhach");

            migrationBuilder.DropColumn(
                name: "Diem",
                table: "CauHoiDeThi");

            migrationBuilder.DropColumn(
                name: "NhomCauHoiId",
                table: "CauHoiDeThi");

            migrationBuilder.DropColumn(
                name: "ThuTuCauHoi",
                table: "CauHoiDeThi");

            migrationBuilder.RenameColumn(
                name: "TongCauHoi",
                table: "DeThi",
                newName: "ThoiGianLamBai");

            migrationBuilder.RenameColumn(
                name: "ThoiLuongPhut",
                table: "DeThi",
                newName: "SoCauHoi");

            migrationBuilder.CreateTable(
                name: "BaiThi",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    DeThiId = table.Column<Guid>(type: "uuid", nullable: false),
                    HocVienId = table.Column<Guid>(type: "uuid", nullable: false),
                    DiemTracNghiem = table.Column<float>(type: "real", nullable: true),
                    DiemTuLuan = table.Column<float>(type: "real", nullable: true),
                    NgayNop = table.Column<DateOnly>(type: "date", nullable: false),
                    NhanXet = table.Column<string>(type: "text", nullable: true),
                    TongDiem = table.Column<float>(type: "real", nullable: true)
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
                name: "DocHieu",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    CapDo = table.Column<int>(type: "integer", nullable: false),
                    DoKho = table.Column<int>(type: "integer", nullable: false),
                    NoiDung = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DocHieu", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "CauHoi",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    DocHieuId = table.Column<Guid>(type: "uuid", nullable: true),
                    CapDo = table.Column<int>(type: "integer", nullable: false),
                    DapAnDung = table.Column<string>(type: "text", nullable: true),
                    DoKho = table.Column<int>(type: "integer", nullable: false),
                    GiaiThich = table.Column<string>(type: "text", nullable: true),
                    KyNang = table.Column<int>(type: "integer", nullable: false),
                    LoaiCauHoi = table.Column<int>(type: "integer", nullable: false),
                    NoiDung = table.Column<string>(type: "text", nullable: false),
                    UrlAmThanh = table.Column<string>(type: "text", nullable: true),
                    UrlHinh = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CauHoi", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CauHoi_DocHieu_DocHieuId",
                        column: x => x.DocHieuId,
                        principalTable: "DocHieu",
                        principalColumn: "Id");
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
                        name: "FK_BaiThiChiTiet_CauHoi_CauHoiId",
                        column: x => x.CauHoiId,
                        principalTable: "CauHoi",
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
                name: "IX_CauHoi_DocHieuId",
                table: "CauHoi",
                column: "DocHieuId");

            migrationBuilder.AddForeignKey(
                name: "FK_CauHoiDeThi_CauHoi_CauHoiId",
                table: "CauHoiDeThi",
                column: "CauHoiId",
                principalTable: "CauHoi",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
