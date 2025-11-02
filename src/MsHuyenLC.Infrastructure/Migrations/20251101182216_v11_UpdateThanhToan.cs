using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MsHuyenLC.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class v11_UpdateThanhToan : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ThanhToan_TaiKhoan_NguoiCapNhatId",
                table: "ThanhToan");

            migrationBuilder.DropForeignKey(
                name: "FK_ThanhToan_TaiKhoan_NguoiXacNhanId",
                table: "ThanhToan");

            migrationBuilder.DropIndex(
                name: "IX_ThanhToan_NguoiCapNhatId",
                table: "ThanhToan");

            migrationBuilder.DropIndex(
                name: "IX_ThanhToan_NguoiXacNhanId",
                table: "ThanhToan");

            migrationBuilder.DropColumn(
                name: "GhiChu",
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

            migrationBuilder.AlterColumn<string>(
                name: "MaThanhToan",
                table: "ThanhToan",
                type: "text",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "text");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "MaThanhToan",
                table: "ThanhToan",
                type: "text",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "text",
                oldNullable: true);

            migrationBuilder.AddColumn<string>(
                name: "GhiChu",
                table: "ThanhToan",
                type: "text",
                nullable: true);

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

            migrationBuilder.CreateIndex(
                name: "IX_ThanhToan_NguoiCapNhatId",
                table: "ThanhToan",
                column: "NguoiCapNhatId");

            migrationBuilder.CreateIndex(
                name: "IX_ThanhToan_NguoiXacNhanId",
                table: "ThanhToan",
                column: "NguoiXacNhanId");

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
    }
}
