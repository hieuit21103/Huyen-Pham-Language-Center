using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MsHuyenLC.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class v7_UpdateThanhToanAndThongBao : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ThanhToan_HocVien_HocVienId",
                table: "ThanhToan");

            migrationBuilder.DropForeignKey(
                name: "FK_ThanhToan_KhoaHoc_KhoaHocId",
                table: "ThanhToan");

            migrationBuilder.DropIndex(
                name: "IX_ThanhToan_HocVienId",
                table: "ThanhToan");

            migrationBuilder.DropColumn(
                name: "NgayGui",
                table: "ThongBao");

            migrationBuilder.DropColumn(
                name: "HocVienId",
                table: "ThanhToan");

            migrationBuilder.RenameColumn(
                name: "NguoiGui",
                table: "ThongBao",
                newName: "NguoiGuiId");

            migrationBuilder.RenameColumn(
                name: "KhoaHocId",
                table: "ThanhToan",
                newName: "DangKyId");

            migrationBuilder.RenameIndex(
                name: "IX_ThanhToan_KhoaHocId",
                table: "ThanhToan",
                newName: "IX_ThanhToan_DangKyId");

            migrationBuilder.AddColumn<string>(
                name: "FileDinhKem",
                table: "ThongBao",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "LoaiThongBao",
                table: "ThongBao",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "MaThongBao",
                table: "ThongBao",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<int>(
                name: "MucDoUuTien",
                table: "ThongBao",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<DateTime>(
                name: "NgaySua",
                table: "ThongBao",
                type: "timestamp with time zone",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "NgayTao",
                table: "ThongBao",
                type: "timestamp with time zone",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<DateTime>(
                name: "NgayXoa",
                table: "ThongBao",
                type: "timestamp with time zone",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "CongThanhToan",
                table: "ThanhToan",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "GhiChu",
                table: "ThanhToan",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "MaGiaoDichNganHang",
                table: "ThanhToan",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "MaThanhToan",
                table: "ThanhToan",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<DateTime>(
                name: "NgayCapNhat",
                table: "ThanhToan",
                type: "timestamp with time zone",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "NgayXacNhan",
                table: "ThanhToan",
                type: "timestamp with time zone",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "NguoiCapNhatId",
                table: "ThanhToan",
                type: "uuid",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "NguoiXacNhanId",
                table: "ThanhToan",
                type: "uuid",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ThongTinNganHang",
                table: "ThanhToan",
                type: "text",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "ThongBaoNguoiNhan",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    ThongBaoId = table.Column<Guid>(type: "uuid", nullable: false),
                    NguoiNhanId = table.Column<Guid>(type: "uuid", nullable: false),
                    DaDoc = table.Column<bool>(type: "boolean", nullable: false),
                    ThoiGianDoc = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    NgayNhan = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ThongBaoNguoiNhan", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ThongBaoNguoiNhan_TaiKhoan_NguoiNhanId",
                        column: x => x.NguoiNhanId,
                        principalTable: "TaiKhoan",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ThongBaoNguoiNhan_ThongBao_ThongBaoId",
                        column: x => x.ThongBaoId,
                        principalTable: "ThongBao",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ThanhToan_NguoiCapNhatId",
                table: "ThanhToan",
                column: "NguoiCapNhatId");

            migrationBuilder.CreateIndex(
                name: "IX_ThanhToan_NguoiXacNhanId",
                table: "ThanhToan",
                column: "NguoiXacNhanId");

            migrationBuilder.CreateIndex(
                name: "IX_ThongBaoNguoiNhan_NguoiNhanId",
                table: "ThongBaoNguoiNhan",
                column: "NguoiNhanId");

            migrationBuilder.CreateIndex(
                name: "IX_ThongBaoNguoiNhan_ThongBaoId",
                table: "ThongBaoNguoiNhan",
                column: "ThongBaoId");

            migrationBuilder.AddForeignKey(
                name: "FK_ThanhToan_DangKy_DangKyId",
                table: "ThanhToan",
                column: "DangKyId",
                principalTable: "DangKy",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_ThanhToan_TaiKhoan_NguoiCapNhatId",
                table: "ThanhToan",
                column: "NguoiCapNhatId",
                principalTable: "TaiKhoan",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_ThanhToan_TaiKhoan_NguoiXacNhanId",
                table: "ThanhToan",
                column: "NguoiXacNhanId",
                principalTable: "TaiKhoan",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ThanhToan_DangKy_DangKyId",
                table: "ThanhToan");

            migrationBuilder.DropForeignKey(
                name: "FK_ThanhToan_TaiKhoan_NguoiCapNhatId",
                table: "ThanhToan");

            migrationBuilder.DropForeignKey(
                name: "FK_ThanhToan_TaiKhoan_NguoiXacNhanId",
                table: "ThanhToan");

            migrationBuilder.DropTable(
                name: "ThongBaoNguoiNhan");

            migrationBuilder.DropIndex(
                name: "IX_ThanhToan_NguoiCapNhatId",
                table: "ThanhToan");

            migrationBuilder.DropIndex(
                name: "IX_ThanhToan_NguoiXacNhanId",
                table: "ThanhToan");

            migrationBuilder.DropColumn(
                name: "FileDinhKem",
                table: "ThongBao");

            migrationBuilder.DropColumn(
                name: "LoaiThongBao",
                table: "ThongBao");

            migrationBuilder.DropColumn(
                name: "MaThongBao",
                table: "ThongBao");

            migrationBuilder.DropColumn(
                name: "MucDoUuTien",
                table: "ThongBao");

            migrationBuilder.DropColumn(
                name: "NgaySua",
                table: "ThongBao");

            migrationBuilder.DropColumn(
                name: "NgayTao",
                table: "ThongBao");

            migrationBuilder.DropColumn(
                name: "NgayXoa",
                table: "ThongBao");

            migrationBuilder.DropColumn(
                name: "CongThanhToan",
                table: "ThanhToan");

            migrationBuilder.DropColumn(
                name: "GhiChu",
                table: "ThanhToan");

            migrationBuilder.DropColumn(
                name: "MaGiaoDichNganHang",
                table: "ThanhToan");

            migrationBuilder.DropColumn(
                name: "MaThanhToan",
                table: "ThanhToan");

            migrationBuilder.DropColumn(
                name: "NgayCapNhat",
                table: "ThanhToan");

            migrationBuilder.DropColumn(
                name: "NgayXacNhan",
                table: "ThanhToan");

            migrationBuilder.DropColumn(
                name: "NguoiCapNhatId",
                table: "ThanhToan");

            migrationBuilder.DropColumn(
                name: "NguoiXacNhanId",
                table: "ThanhToan");

            migrationBuilder.DropColumn(
                name: "ThongTinNganHang",
                table: "ThanhToan");

            migrationBuilder.RenameColumn(
                name: "NguoiGuiId",
                table: "ThongBao",
                newName: "NguoiGui");

            migrationBuilder.RenameColumn(
                name: "DangKyId",
                table: "ThanhToan",
                newName: "KhoaHocId");

            migrationBuilder.RenameIndex(
                name: "IX_ThanhToan_DangKyId",
                table: "ThanhToan",
                newName: "IX_ThanhToan_KhoaHocId");

            migrationBuilder.AddColumn<DateOnly>(
                name: "NgayGui",
                table: "ThongBao",
                type: "date",
                nullable: false,
                defaultValue: new DateOnly(1, 1, 1));

            migrationBuilder.AddColumn<Guid>(
                name: "HocVienId",
                table: "ThanhToan",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.CreateIndex(
                name: "IX_ThanhToan_HocVienId",
                table: "ThanhToan",
                column: "HocVienId");

            migrationBuilder.AddForeignKey(
                name: "FK_ThanhToan_HocVien_HocVienId",
                table: "ThanhToan",
                column: "HocVienId",
                principalTable: "HocVien",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_ThanhToan_KhoaHoc_KhoaHocId",
                table: "ThanhToan",
                column: "KhoaHocId",
                principalTable: "KhoaHoc",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
