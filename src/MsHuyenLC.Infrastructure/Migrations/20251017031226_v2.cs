using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MsHuyenLC.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class v2 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_LopHoc_PhongHoc_PhongHocId",
                table: "LopHoc");

            migrationBuilder.DropIndex(
                name: "IX_LopHoc_PhongHocId",
                table: "LopHoc");

            migrationBuilder.DropColumn(
                name: "PhongHocId",
                table: "LopHoc");

            migrationBuilder.AddColumn<Guid>(
                name: "PhongHocId",
                table: "LichHoc",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.CreateIndex(
                name: "IX_LichHoc_PhongHocId",
                table: "LichHoc",
                column: "PhongHocId");

            migrationBuilder.AddForeignKey(
                name: "FK_LichHoc_PhongHoc_PhongHocId",
                table: "LichHoc",
                column: "PhongHocId",
                principalTable: "PhongHoc",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_LichHoc_PhongHoc_PhongHocId",
                table: "LichHoc");

            migrationBuilder.DropIndex(
                name: "IX_LichHoc_PhongHocId",
                table: "LichHoc");

            migrationBuilder.DropColumn(
                name: "PhongHocId",
                table: "LichHoc");

            migrationBuilder.AddColumn<Guid>(
                name: "PhongHocId",
                table: "LopHoc",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.CreateIndex(
                name: "IX_LopHoc_PhongHocId",
                table: "LopHoc",
                column: "PhongHocId");

            migrationBuilder.AddForeignKey(
                name: "FK_LopHoc_PhongHoc_PhongHocId",
                table: "LopHoc",
                column: "PhongHocId",
                principalTable: "PhongHoc",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
