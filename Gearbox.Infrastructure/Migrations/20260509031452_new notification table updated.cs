using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Gearbox.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class newnotificationtableupdated : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsGlobal",
                table: "Notifications",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<string>(
                name: "TargetRole",
                table: "Notifications",
                type: "text",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsGlobal",
                table: "Notifications");

            migrationBuilder.DropColumn(
                name: "TargetRole",
                table: "Notifications");
        }
    }
}
