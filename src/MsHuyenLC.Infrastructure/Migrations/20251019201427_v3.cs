using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MsHuyenLC.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class v3 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_BaiThiChiTiet_DeThiCauHoi_CauHoiId",
                table: "BaiThiChiTiet");

            migrationBuilder.DropForeignKey(
                name: "FK_DeThi_KyThi_KyThiId",
                table: "DeThi");

            migrationBuilder.DropTable(
                name: "DeThiCauHoi");

            migrationBuilder.DropTable(
                name: "NganHangDe");

            migrationBuilder.AlterColumn<Guid>(
                name: "KyThiId",
                table: "DeThi",
                type: "uuid",
                nullable: true,
                oldClrType: typeof(Guid),
                oldType: "uuid");

            migrationBuilder.AddColumn<int>(
                name: "LoaiDeThi",
                table: "DeThi",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "ThoiGianLamBai",
                table: "DeThi",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateTable(
                name: "DocHieu",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    NoiDung = table.Column<string>(type: "text", nullable: false),
                    CapDo = table.Column<int>(type: "integer", nullable: false),
                    DoKho = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DocHieu", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "CauHoi",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    NoiDung = table.Column<string>(type: "text", nullable: false),
                    LoaiCauHoi = table.Column<int>(type: "integer", nullable: false),
                    KyNang = table.Column<int>(type: "integer", nullable: false),
                    UrlHinh = table.Column<string>(type: "text", nullable: true),
                    UrlAmThanh = table.Column<string>(type: "text", nullable: true),
                    DapAnDung = table.Column<string>(type: "text", nullable: true),
                    GiaiThich = table.Column<string>(type: "text", nullable: true),
                    CapDo = table.Column<int>(type: "integer", nullable: false),
                    DoKho = table.Column<int>(type: "integer", nullable: false),
                    DocHieuId = table.Column<Guid>(type: "uuid", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CauHoi", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CauHoi_DocHieu_DocHieuId",
                        column: x => x.DocHieuId,
                        principalTable: "DocHieu",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "CauHoiDeThi",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    DeThiId = table.Column<Guid>(type: "uuid", nullable: false),
                    CauHoiId = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CauHoiDeThi", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CauHoiDeThi_CauHoi_CauHoiId",
                        column: x => x.CauHoiId,
                        principalTable: "CauHoi",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_CauHoiDeThi_DeThi_DeThiId",
                        column: x => x.DeThiId,
                        principalTable: "DeThi",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_CauHoi_DocHieuId",
                table: "CauHoi",
                column: "DocHieuId");

            migrationBuilder.CreateIndex(
                name: "IX_CauHoiDeThi_CauHoiId",
                table: "CauHoiDeThi",
                column: "CauHoiId");

            migrationBuilder.CreateIndex(
                name: "IX_CauHoiDeThi_DeThiId",
                table: "CauHoiDeThi",
                column: "DeThiId");

            migrationBuilder.AddForeignKey(
                name: "FK_BaiThiChiTiet_CauHoi_CauHoiId",
                table: "BaiThiChiTiet",
                column: "CauHoiId",
                principalTable: "CauHoi",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_DeThi_KyThi_KyThiId",
                table: "DeThi",
                column: "KyThiId",
                principalTable: "KyThi",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_BaiThiChiTiet_CauHoi_CauHoiId",
                table: "BaiThiChiTiet");

            migrationBuilder.DropForeignKey(
                name: "FK_DeThi_KyThi_KyThiId",
                table: "DeThi");

            migrationBuilder.DropTable(
                name: "CauHoiDeThi");

            migrationBuilder.DropTable(
                name: "CauHoi");

            migrationBuilder.DropTable(
                name: "DocHieu");

            migrationBuilder.DropColumn(
                name: "LoaiDeThi",
                table: "DeThi");

            migrationBuilder.DropColumn(
                name: "ThoiGianLamBai",
                table: "DeThi");

            migrationBuilder.AlterColumn<Guid>(
                name: "KyThiId",
                table: "DeThi",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"),
                oldClrType: typeof(Guid),
                oldType: "uuid",
                oldNullable: true);

            migrationBuilder.CreateTable(
                name: "NganHangDe",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    DapAnDung = table.Column<string>(type: "text", nullable: true),
                    LoaiCauHoi = table.Column<int>(type: "integer", nullable: false),
                    MucDo = table.Column<int>(type: "integer", nullable: false),
                    NoiDung = table.Column<string>(type: "text", nullable: false),
                    UrlAmThanh = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_NganHangDe", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "DeThiCauHoi",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    CauHoiId = table.Column<Guid>(type: "uuid", nullable: false),
                    DeThiId = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DeThiCauHoi", x => x.Id);
                    table.ForeignKey(
                        name: "FK_DeThiCauHoi_DeThi_DeThiId",
                        column: x => x.DeThiId,
                        principalTable: "DeThi",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_DeThiCauHoi_NganHangDe_CauHoiId",
                        column: x => x.CauHoiId,
                        principalTable: "NganHangDe",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_DeThiCauHoi_CauHoiId",
                table: "DeThiCauHoi",
                column: "CauHoiId");

            migrationBuilder.CreateIndex(
                name: "IX_DeThiCauHoi_DeThiId",
                table: "DeThiCauHoi",
                column: "DeThiId");

            migrationBuilder.AddForeignKey(
                name: "FK_BaiThiChiTiet_DeThiCauHoi_CauHoiId",
                table: "BaiThiChiTiet",
                column: "CauHoiId",
                principalTable: "DeThiCauHoi",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_DeThi_KyThi_KyThiId",
                table: "DeThi",
                column: "KyThiId",
                principalTable: "KyThi",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
