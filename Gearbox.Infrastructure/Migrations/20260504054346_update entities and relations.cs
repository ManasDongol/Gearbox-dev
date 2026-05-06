using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Gearbox.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class updateentitiesandrelations : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ServiceBills_Appointments_AppointmentId",
                table: "ServiceBills");

            migrationBuilder.DropForeignKey(
                name: "FK_ServiceBills_ServiceHistories_ServiceHistoryId",
                table: "ServiceBills");

            migrationBuilder.DropTable(
                name: "SalesInvoiceItems");

            migrationBuilder.DropTable(
                name: "ServiceHistoryServiceDetails");

            migrationBuilder.DropTable(
                name: "SalesInvoices");

            migrationBuilder.DropPrimaryKey(
                name: "PK_ServiceBills",
                table: "ServiceBills");

            migrationBuilder.RenameTable(
                name: "ServiceBills",
                newName: "ServiceBill");

            migrationBuilder.RenameIndex(
                name: "IX_ServiceBills_ServiceHistoryId",
                table: "ServiceBill",
                newName: "IX_ServiceBill_ServiceHistoryId");

            migrationBuilder.RenameIndex(
                name: "IX_ServiceBills_AppointmentId",
                table: "ServiceBill",
                newName: "IX_ServiceBill_AppointmentId");

            migrationBuilder.AddColumn<Guid>(
                name: "ServiceId",
                table: "ServiceReviews",
                type: "uuid",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "ServiceDetailsId",
                table: "ServiceHistories",
                type: "uuid",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "ServiceId",
                table: "ServiceHistories",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddPrimaryKey(
                name: "PK_ServiceBill",
                table: "ServiceBill",
                column: "Id");

            migrationBuilder.CreateTable(
                name: "SalesServicesInvoices",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    CustomerId = table.Column<Guid>(type: "uuid", nullable: false),
                    StaffId = table.Column<Guid>(type: "uuid", nullable: false),
                    AppointmentId = table.Column<Guid>(type: "uuid", nullable: true),
                    TotalAmount = table.Column<decimal>(type: "numeric(18,2)", nullable: false),
                    DiscountAmount = table.Column<decimal>(type: "numeric(18,2)", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SalesServicesInvoices", x => x.Id);
                    table.ForeignKey(
                        name: "FK_SalesServicesInvoices_Appointments_AppointmentId",
                        column: x => x.AppointmentId,
                        principalTable: "Appointments",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_SalesServicesInvoices_Customers_CustomerId",
                        column: x => x.CustomerId,
                        principalTable: "Customers",
                        principalColumn: "UserId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_SalesServicesInvoices_Staffs_StaffId",
                        column: x => x.StaffId,
                        principalTable: "Staffs",
                        principalColumn: "UserId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Services",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Name = table.Column<string>(type: "text", nullable: false),
                    Description = table.Column<string>(type: "text", nullable: false),
                    Price = table.Column<decimal>(type: "numeric", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Services", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "SalesServicesInvoiceItems",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    SalesServicesInvoiceId = table.Column<Guid>(type: "uuid", nullable: false),
                    PartId = table.Column<Guid>(type: "uuid", nullable: true),
                    ServiceId = table.Column<Guid>(type: "uuid", nullable: true),
                    Type = table.Column<string>(type: "text", nullable: false),
                    Quantity = table.Column<int>(type: "integer", nullable: false),
                    UnitPrice = table.Column<decimal>(type: "numeric(18,2)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SalesServicesInvoiceItems", x => x.Id);
                    table.ForeignKey(
                        name: "FK_SalesServicesInvoiceItems_Parts_PartId",
                        column: x => x.PartId,
                        principalTable: "Parts",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_SalesServicesInvoiceItems_SalesServicesInvoices_SalesServic~",
                        column: x => x.SalesServicesInvoiceId,
                        principalTable: "SalesServicesInvoices",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_SalesServicesInvoiceItems_Services_ServiceId",
                        column: x => x.ServiceId,
                        principalTable: "Services",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateIndex(
                name: "IX_ServiceReviews_ServiceId",
                table: "ServiceReviews",
                column: "ServiceId");

            migrationBuilder.CreateIndex(
                name: "IX_ServiceHistories_ServiceDetailsId",
                table: "ServiceHistories",
                column: "ServiceDetailsId");

            migrationBuilder.CreateIndex(
                name: "IX_ServiceHistories_ServiceId",
                table: "ServiceHistories",
                column: "ServiceId");

            migrationBuilder.CreateIndex(
                name: "IX_SalesServicesInvoiceItems_PartId",
                table: "SalesServicesInvoiceItems",
                column: "PartId");

            migrationBuilder.CreateIndex(
                name: "IX_SalesServicesInvoiceItems_SalesServicesInvoiceId",
                table: "SalesServicesInvoiceItems",
                column: "SalesServicesInvoiceId");

            migrationBuilder.CreateIndex(
                name: "IX_SalesServicesInvoiceItems_ServiceId",
                table: "SalesServicesInvoiceItems",
                column: "ServiceId");

            migrationBuilder.CreateIndex(
                name: "IX_SalesServicesInvoices_AppointmentId",
                table: "SalesServicesInvoices",
                column: "AppointmentId");

            migrationBuilder.CreateIndex(
                name: "IX_SalesServicesInvoices_CustomerId",
                table: "SalesServicesInvoices",
                column: "CustomerId");

            migrationBuilder.CreateIndex(
                name: "IX_SalesServicesInvoices_StaffId",
                table: "SalesServicesInvoices",
                column: "StaffId");

            migrationBuilder.AddForeignKey(
                name: "FK_ServiceBill_Appointments_AppointmentId",
                table: "ServiceBill",
                column: "AppointmentId",
                principalTable: "Appointments",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_ServiceBill_ServiceHistories_ServiceHistoryId",
                table: "ServiceBill",
                column: "ServiceHistoryId",
                principalTable: "ServiceHistories",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_ServiceHistories_ServiceDetails_ServiceDetailsId",
                table: "ServiceHistories",
                column: "ServiceDetailsId",
                principalTable: "ServiceDetails",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_ServiceHistories_Services_ServiceId",
                table: "ServiceHistories",
                column: "ServiceId",
                principalTable: "Services",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_ServiceReviews_Services_ServiceId",
                table: "ServiceReviews",
                column: "ServiceId",
                principalTable: "Services",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ServiceBill_Appointments_AppointmentId",
                table: "ServiceBill");

            migrationBuilder.DropForeignKey(
                name: "FK_ServiceBill_ServiceHistories_ServiceHistoryId",
                table: "ServiceBill");

            migrationBuilder.DropForeignKey(
                name: "FK_ServiceHistories_ServiceDetails_ServiceDetailsId",
                table: "ServiceHistories");

            migrationBuilder.DropForeignKey(
                name: "FK_ServiceHistories_Services_ServiceId",
                table: "ServiceHistories");

            migrationBuilder.DropForeignKey(
                name: "FK_ServiceReviews_Services_ServiceId",
                table: "ServiceReviews");

            migrationBuilder.DropTable(
                name: "SalesServicesInvoiceItems");

            migrationBuilder.DropTable(
                name: "SalesServicesInvoices");

            migrationBuilder.DropTable(
                name: "Services");

            migrationBuilder.DropIndex(
                name: "IX_ServiceReviews_ServiceId",
                table: "ServiceReviews");

            migrationBuilder.DropIndex(
                name: "IX_ServiceHistories_ServiceDetailsId",
                table: "ServiceHistories");

            migrationBuilder.DropIndex(
                name: "IX_ServiceHistories_ServiceId",
                table: "ServiceHistories");

            migrationBuilder.DropPrimaryKey(
                name: "PK_ServiceBill",
                table: "ServiceBill");

            migrationBuilder.DropColumn(
                name: "ServiceId",
                table: "ServiceReviews");

            migrationBuilder.DropColumn(
                name: "ServiceDetailsId",
                table: "ServiceHistories");

            migrationBuilder.DropColumn(
                name: "ServiceId",
                table: "ServiceHistories");

            migrationBuilder.RenameTable(
                name: "ServiceBill",
                newName: "ServiceBills");

            migrationBuilder.RenameIndex(
                name: "IX_ServiceBill_ServiceHistoryId",
                table: "ServiceBills",
                newName: "IX_ServiceBills_ServiceHistoryId");

            migrationBuilder.RenameIndex(
                name: "IX_ServiceBill_AppointmentId",
                table: "ServiceBills",
                newName: "IX_ServiceBills_AppointmentId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_ServiceBills",
                table: "ServiceBills",
                column: "Id");

            migrationBuilder.CreateTable(
                name: "SalesInvoices",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    CustomerId = table.Column<Guid>(type: "uuid", nullable: false),
                    StaffId = table.Column<Guid>(type: "uuid", nullable: false),
                    CreatedDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    DiscountAmount = table.Column<decimal>(type: "numeric(18,2)", nullable: false),
                    DueDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    InvoiceNumber = table.Column<string>(type: "text", nullable: false),
                    IsLoyaltyDiscountApplied = table.Column<bool>(type: "boolean", nullable: false),
                    IsPaid = table.Column<bool>(type: "boolean", nullable: false),
                    TotalAmount = table.Column<decimal>(type: "numeric(18,2)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SalesInvoices", x => x.Id);
                    table.ForeignKey(
                        name: "FK_SalesInvoices_Customers_CustomerId",
                        column: x => x.CustomerId,
                        principalTable: "Customers",
                        principalColumn: "UserId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_SalesInvoices_Staffs_StaffId",
                        column: x => x.StaffId,
                        principalTable: "Staffs",
                        principalColumn: "UserId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "ServiceHistoryServiceDetails",
                columns: table => new
                {
                    ServiceHistoriesId = table.Column<Guid>(type: "uuid", nullable: false),
                    ServicesId = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ServiceHistoryServiceDetails", x => new { x.ServiceHistoriesId, x.ServicesId });
                    table.ForeignKey(
                        name: "FK_ServiceHistoryServiceDetails_ServiceDetails_ServicesId",
                        column: x => x.ServicesId,
                        principalTable: "ServiceDetails",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ServiceHistoryServiceDetails_ServiceHistories_ServiceHistor~",
                        column: x => x.ServiceHistoriesId,
                        principalTable: "ServiceHistories",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "SalesInvoiceItems",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    PartId = table.Column<Guid>(type: "uuid", nullable: false),
                    SalesInvoiceId = table.Column<Guid>(type: "uuid", nullable: false),
                    Quantity = table.Column<int>(type: "integer", nullable: false),
                    UnitPrice = table.Column<decimal>(type: "numeric(18,2)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SalesInvoiceItems", x => x.Id);
                    table.ForeignKey(
                        name: "FK_SalesInvoiceItems_Parts_PartId",
                        column: x => x.PartId,
                        principalTable: "Parts",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_SalesInvoiceItems_SalesInvoices_SalesInvoiceId",
                        column: x => x.SalesInvoiceId,
                        principalTable: "SalesInvoices",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_SalesInvoiceItems_PartId",
                table: "SalesInvoiceItems",
                column: "PartId");

            migrationBuilder.CreateIndex(
                name: "IX_SalesInvoiceItems_SalesInvoiceId",
                table: "SalesInvoiceItems",
                column: "SalesInvoiceId");

            migrationBuilder.CreateIndex(
                name: "IX_SalesInvoices_CustomerId",
                table: "SalesInvoices",
                column: "CustomerId");

            migrationBuilder.CreateIndex(
                name: "IX_SalesInvoices_StaffId",
                table: "SalesInvoices",
                column: "StaffId");

            migrationBuilder.CreateIndex(
                name: "IX_ServiceHistoryServiceDetails_ServicesId",
                table: "ServiceHistoryServiceDetails",
                column: "ServicesId");

            migrationBuilder.AddForeignKey(
                name: "FK_ServiceBills_Appointments_AppointmentId",
                table: "ServiceBills",
                column: "AppointmentId",
                principalTable: "Appointments",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);

            migrationBuilder.AddForeignKey(
                name: "FK_ServiceBills_ServiceHistories_ServiceHistoryId",
                table: "ServiceBills",
                column: "ServiceHistoryId",
                principalTable: "ServiceHistories",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);
        }
    }
}
