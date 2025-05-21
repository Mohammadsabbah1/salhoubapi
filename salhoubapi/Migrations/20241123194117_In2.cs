using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace salhoubapi.Migrations
{
    /// <inheritdoc />
    public partial class In2 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "percantage",
                table: "Users",
                newName: "Percantage");

            migrationBuilder.AlterColumn<float>(
                name: "Percantage",
                table: "Users",
                type: "real",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Percantage",
                table: "Users",
                newName: "percantage");

            migrationBuilder.AlterColumn<string>(
                name: "percantage",
                table: "Users",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(float),
                oldType: "real");
        }
    }
}
