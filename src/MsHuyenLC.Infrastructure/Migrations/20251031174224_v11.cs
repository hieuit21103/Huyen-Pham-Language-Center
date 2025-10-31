using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MsHuyenLC.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class v11 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "KetQuaHocTap");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "KetQuaHocTap",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    HocVienId = table.Column<Guid>(type: "uuid", nullable: false),
                    KyThiId = table.Column<Guid>(type: "uuid", nullable: false),
                    DiemSo = table.Column<decimal>(type: "numeric", nullable: false),
                    NgayThi = table.Column<DateOnly>(type: "date", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_KetQuaHocTap", x => x.Id);
                    table.ForeignKey(
                        name: "FK_KetQuaHocTap_HocVien_HocVienId",
                        column: x => x.HocVienId,
                        principalTable: "HocVien",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_KetQuaHocTap_KyThi_KyThiId",
                        column: x => x.KyThiId,
                        principalTable: "KyThi",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_KetQuaHocTap_HocVienId",
                table: "KetQuaHocTap",
                column: "HocVienId");

            migrationBuilder.CreateIndex(
                name: "IX_KetQuaHocTap_KyThiId",
                table: "KetQuaHocTap",
                column: "KyThiId");
        }
    }
}
