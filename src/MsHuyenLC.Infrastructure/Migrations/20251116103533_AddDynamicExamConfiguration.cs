using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MsHuyenLC.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddDynamicExamConfiguration : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "LoaiDeThi",
                table: "DeThi");

            migrationBuilder.DropColumn(
                name: "TongCauHoi",
                table: "DeThi");

            migrationBuilder.AddColumn<Guid>(
                name: "KyThiId",
                table: "PhienLamBai",
                type: "uuid",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "KyNang",
                table: "NhomCauHoi",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateTable(
                name: "CauHinhKyThi",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    CapDo = table.Column<int>(type: "integer", nullable: false),
                    DoKho = table.Column<int>(type: "integer", nullable: false),
                    KyNang = table.Column<int>(type: "integer", nullable: false),
                    CheDoCauHoi = table.Column<int>(type: "integer", nullable: false),
                    SoCauHoi = table.Column<int>(type: "integer", nullable: false),
                    KyThiId = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CauHinhKyThi", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CauHinhKyThi_KyThi_KyThiId",
                        column: x => x.KyThiId,
                        principalTable: "KyThi",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_PhienLamBai_KyThiId",
                table: "PhienLamBai",
                column: "KyThiId");

            migrationBuilder.CreateIndex(
                name: "IX_CauHinhKyThi_KyThiId",
                table: "CauHinhKyThi",
                column: "KyThiId");

            migrationBuilder.AddForeignKey(
                name: "FK_PhienLamBai_KyThi_KyThiId",
                table: "PhienLamBai",
                column: "KyThiId",
                principalTable: "KyThi",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_PhienLamBai_KyThi_KyThiId",
                table: "PhienLamBai");

            migrationBuilder.DropTable(
                name: "CauHinhKyThi");

            migrationBuilder.DropIndex(
                name: "IX_PhienLamBai_KyThiId",
                table: "PhienLamBai");

            migrationBuilder.DropColumn(
                name: "KyThiId",
                table: "PhienLamBai");

            migrationBuilder.DropColumn(
                name: "KyNang",
                table: "NhomCauHoi");

            migrationBuilder.AddColumn<int>(
                name: "LoaiDeThi",
                table: "DeThi",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "TongCauHoi",
                table: "DeThi",
                type: "integer",
                nullable: false,
                defaultValue: 0);
        }
    }
}
