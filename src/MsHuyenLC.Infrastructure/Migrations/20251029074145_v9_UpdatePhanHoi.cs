using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MsHuyenLC.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class v9_UpdatePhanHoi : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_PhanHoi_GiaoVien_GiaoVienId",
                table: "PhanHoi");

            migrationBuilder.DropIndex(
                name: "IX_PhanHoi_GiaoVienId",
                table: "PhanHoi");

            migrationBuilder.DropColumn(
                name: "GiaoVienId",
                table: "PhanHoi");

            migrationBuilder.RenameColumn(
                name: "NgayGui",
                table: "PhanHoi",
                newName: "NgayTao");

            migrationBuilder.AddColumn<string>(
                name: "LoaiPhanHoi",
                table: "PhanHoi",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "TieuDe",
                table: "PhanHoi",
                type: "text",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "LoaiPhanHoi",
                table: "PhanHoi");

            migrationBuilder.DropColumn(
                name: "TieuDe",
                table: "PhanHoi");

            migrationBuilder.RenameColumn(
                name: "NgayTao",
                table: "PhanHoi",
                newName: "NgayGui");

            migrationBuilder.AddColumn<Guid>(
                name: "GiaoVienId",
                table: "PhanHoi",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.CreateIndex(
                name: "IX_PhanHoi_GiaoVienId",
                table: "PhanHoi",
                column: "GiaoVienId");

            migrationBuilder.AddForeignKey(
                name: "FK_PhanHoi_GiaoVien_GiaoVienId",
                table: "PhanHoi",
                column: "GiaoVienId",
                principalTable: "GiaoVien",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
