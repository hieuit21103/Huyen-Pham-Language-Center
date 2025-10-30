using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MsHuyenLC.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class v9_SimplifyThongBao : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "NgayNhan",
                table: "ThongBaoNguoiNhan");

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
                name: "NgayXoa",
                table: "ThongBao");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "NgayNhan",
                table: "ThongBaoNguoiNhan",
                type: "timestamp with time zone",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

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
                name: "NgayXoa",
                table: "ThongBao",
                type: "timestamp with time zone",
                nullable: true);
        }
    }
}
