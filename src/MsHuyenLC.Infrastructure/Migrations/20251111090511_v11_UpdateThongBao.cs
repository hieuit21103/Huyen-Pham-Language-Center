using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MsHuyenLC.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class v11_UpdateThongBao : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ThongBao_TaiKhoan_NguoiNhanId",
                table: "ThongBao");

            migrationBuilder.AlterColumn<Guid>(
                name: "NguoiNhanId",
                table: "ThongBao",
                type: "uuid",
                nullable: true,
                oldClrType: typeof(Guid),
                oldType: "uuid");

            migrationBuilder.AddForeignKey(
                name: "FK_ThongBao_TaiKhoan_NguoiNhanId",
                table: "ThongBao",
                column: "NguoiNhanId",
                principalTable: "TaiKhoan",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ThongBao_TaiKhoan_NguoiNhanId",
                table: "ThongBao");

            migrationBuilder.AlterColumn<Guid>(
                name: "NguoiNhanId",
                table: "ThongBao",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"),
                oldClrType: typeof(Guid),
                oldType: "uuid",
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_ThongBao_TaiKhoan_NguoiNhanId",
                table: "ThongBao",
                column: "NguoiNhanId",
                principalTable: "TaiKhoan",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
