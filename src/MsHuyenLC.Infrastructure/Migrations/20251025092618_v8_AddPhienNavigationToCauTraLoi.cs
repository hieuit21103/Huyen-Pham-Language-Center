using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MsHuyenLC.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class v8_AddPhienNavigationToCauTraLoi : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                name: "IX_CauTraLoi_PhienId",
                table: "CauTraLoi",
                column: "PhienId");

            migrationBuilder.AddForeignKey(
                name: "FK_CauTraLoi_PhienLamBai_PhienId",
                table: "CauTraLoi",
                column: "PhienId",
                principalTable: "PhienLamBai",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_CauTraLoi_PhienLamBai_PhienId",
                table: "CauTraLoi");

            migrationBuilder.DropIndex(
                name: "IX_CauTraLoi_PhienId",
                table: "CauTraLoi");
        }
    }
}
