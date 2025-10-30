using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MsHuyenLC.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class v10_UpdatePhienLamBai : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_PhienLamBai_TaiKhoan_HocVienId",
                table: "PhienLamBai");

            migrationBuilder.AddForeignKey(
                name: "FK_PhienLamBai_HocVien_HocVienId",
                table: "PhienLamBai",
                column: "HocVienId",
                principalTable: "HocVien",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_PhienLamBai_HocVien_HocVienId",
                table: "PhienLamBai");

            migrationBuilder.AddForeignKey(
                name: "FK_PhienLamBai_TaiKhoan_HocVienId",
                table: "PhienLamBai",
                column: "HocVienId",
                principalTable: "TaiKhoan",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
