using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MsHuyenLC.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class v9 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ThongBao_LopHoc_LopHocId",
                table: "ThongBao");

            migrationBuilder.DropIndex(
                name: "IX_ThongBao_LopHocId",
                table: "ThongBao");

            migrationBuilder.DropColumn(
                name: "DoiTuongNhan",
                table: "ThongBao");

            migrationBuilder.DropColumn(
                name: "LopHocId",
                table: "ThongBao");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "DoiTuongNhan",
                table: "ThongBao",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<Guid>(
                name: "LopHocId",
                table: "ThongBao",
                type: "uuid",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_ThongBao_LopHocId",
                table: "ThongBao",
                column: "LopHocId");

            migrationBuilder.AddForeignKey(
                name: "FK_ThongBao_LopHoc_LopHocId",
                table: "ThongBao",
                column: "LopHocId",
                principalTable: "LopHoc",
                principalColumn: "Id");
        }
    }
}
