using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MsHuyenLC.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class v10_RemoveThongBaoTracking : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ThongBaoNguoiNhan");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "ThongBaoNguoiNhan",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    NguoiNhanId = table.Column<Guid>(type: "uuid", nullable: false),
                    ThongBaoId = table.Column<Guid>(type: "uuid", nullable: false),
                    DaDoc = table.Column<bool>(type: "boolean", nullable: false),
                    ThoiGianDoc = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ThongBaoNguoiNhan", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ThongBaoNguoiNhan_TaiKhoan_NguoiNhanId",
                        column: x => x.NguoiNhanId,
                        principalTable: "TaiKhoan",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ThongBaoNguoiNhan_ThongBao_ThongBaoId",
                        column: x => x.ThongBaoId,
                        principalTable: "ThongBao",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ThongBaoNguoiNhan_NguoiNhanId",
                table: "ThongBaoNguoiNhan",
                column: "NguoiNhanId");

            migrationBuilder.CreateIndex(
                name: "IX_ThongBaoNguoiNhan_ThongBaoId",
                table: "ThongBaoNguoiNhan",
                column: "ThongBaoId");
        }
    }
}
