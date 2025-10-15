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
            migrationBuilder.DropColumn(
                name: "DatLaiMatKhauToken",
                table: "TaiKhoan");

            migrationBuilder.DropColumn(
                name: "ThoiHanToken",
                table: "TaiKhoan");

            migrationBuilder.DropColumn(
                name: "IdLopHoc",
                table: "LichHoc");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "DatLaiMatKhauToken",
                table: "TaiKhoan",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ThoiHanToken",
                table: "TaiKhoan",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "IdLopHoc",
                table: "LichHoc",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));
        }
    }
}
