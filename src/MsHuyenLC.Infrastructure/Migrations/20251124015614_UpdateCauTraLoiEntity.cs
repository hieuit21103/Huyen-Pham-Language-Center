using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MsHuyenLC.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class UpdateCauTraLoiEntity : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "CauTraLoiText",
                table: "CauTraLoi",
                type: "text",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "varchar(10)",
                oldMaxLength: 10,
                oldNullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "CauTraLoiText",
                table: "CauTraLoi",
                type: "varchar(10)",
                maxLength: 10,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "text",
                oldNullable: true);
        }
    }
}
