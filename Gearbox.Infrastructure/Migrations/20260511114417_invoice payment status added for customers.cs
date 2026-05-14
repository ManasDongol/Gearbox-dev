using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Gearbox.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class invoicepaymentstatusaddedforcustomers : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "PaymentStatus",
                table: "SalesServicesInvoices",
                type: "boolean",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "PaymentStatus",
                table: "SalesServicesInvoices");
        }
    }
}
