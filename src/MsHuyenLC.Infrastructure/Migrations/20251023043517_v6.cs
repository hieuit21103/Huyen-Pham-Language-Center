using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MsHuyenLC.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class v6 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "TieuDe",
                table: "NhomCauHoi",
                type: "text",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "text",
                oldNullable: true);

            migrationBuilder.AddColumn<string>(
                name: "UrlHinhAnh",
                table: "NhomCauHoi",
                type: "text",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "UrlHinhAnh",
                table: "NhomCauHoi");

            migrationBuilder.AlterColumn<string>(
                name: "TieuDe",
                table: "NhomCauHoi",
                type: "text",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "text");
        }
    }
}
