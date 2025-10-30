using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MsHuyenLC.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class v9_UpdateThongBao : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ThongBao_TaiKhoan_NguoiGuiTaiKhoanId",
                table: "ThongBao");

            migrationBuilder.DropIndex(
                name: "IX_ThongBao_NguoiGuiTaiKhoanId",
                table: "ThongBao");

            migrationBuilder.DropColumn(
                name: "NguoiGuiTaiKhoanId",
                table: "ThongBao");

            migrationBuilder.CreateIndex(
                name: "IX_ThongBao_NguoiGuiId",
                table: "ThongBao",
                column: "NguoiGuiId");

            migrationBuilder.AddForeignKey(
                name: "FK_ThongBao_TaiKhoan_NguoiGuiId",
                table: "ThongBao",
                column: "NguoiGuiId",
                principalTable: "TaiKhoan",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ThongBao_TaiKhoan_NguoiGuiId",
                table: "ThongBao");

            migrationBuilder.DropIndex(
                name: "IX_ThongBao_NguoiGuiId",
                table: "ThongBao");

            migrationBuilder.AddColumn<Guid>(
                name: "NguoiGuiTaiKhoanId",
                table: "ThongBao",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.CreateIndex(
                name: "IX_ThongBao_NguoiGuiTaiKhoanId",
                table: "ThongBao",
                column: "NguoiGuiTaiKhoanId");

            migrationBuilder.AddForeignKey(
                name: "FK_ThongBao_TaiKhoan_NguoiGuiTaiKhoanId",
                table: "ThongBao",
                column: "NguoiGuiTaiKhoanId",
                principalTable: "TaiKhoan",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
