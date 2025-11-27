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
                name: "GioBatDau",
                table: "LichHoc");

            migrationBuilder.DropColumn(
                name: "GioKetThuc",
                table: "LichHoc");

            migrationBuilder.DropColumn(
                name: "Thu",
                table: "LichHoc");

            migrationBuilder.CreateTable(
                name: "ThoiGianBieu",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    LichHocId = table.Column<Guid>(type: "uuid", nullable: false),
                    Thu = table.Column<int>(type: "integer", nullable: false),
                    GioBatDau = table.Column<TimeOnly>(type: "time without time zone", nullable: false),
                    GioKetThuc = table.Column<TimeOnly>(type: "time without time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ThoiGianBieu", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ThoiGianBieu_LichHoc_LichHocId",
                        column: x => x.LichHocId,
                        principalTable: "LichHoc",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ThoiGianBieu_LichHocId",
                table: "ThoiGianBieu",
                column: "LichHocId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ThoiGianBieu");

            migrationBuilder.AddColumn<TimeOnly>(
                name: "GioBatDau",
                table: "LichHoc",
                type: "time without time zone",
                nullable: false,
                defaultValue: new TimeOnly(0, 0, 0));

            migrationBuilder.AddColumn<TimeOnly>(
                name: "GioKetThuc",
                table: "LichHoc",
                type: "time without time zone",
                nullable: false,
                defaultValue: new TimeOnly(0, 0, 0));

            migrationBuilder.AddColumn<int>(
                name: "Thu",
                table: "LichHoc",
                type: "integer",
                nullable: false,
                defaultValue: 0);
        }
    }
}
