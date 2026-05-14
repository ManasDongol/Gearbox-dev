using Gearbox.Application.Interfaces;
using Gearbox.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;

namespace Gearbox.Application.Services;

public class PdfService : IPdfService
{
    private readonly ApplicationDbContext _context;

    public PdfService(ApplicationDbContext context)
    {
        _context = context;
        QuestPDF.Settings.License = LicenseType.Community;
    }

    public async Task<byte[]> GenerateFinancialReportAsync()
    {
        var salesInvoices = await _context.SalesServicesInvoices
            .AsNoTracking()
            .Include(i => i.Customer)
                .ThenInclude(c => c.User)
            .Include(i => i.Staff)
                .ThenInclude(s => s.User)
            .OrderByDescending(i => i.CreatedAt)
            .ToListAsync();

        var purchaseInvoices = await _context.PurchaseInvoices
            .AsNoTracking()
            .Include(i => i.Vendor)
            .OrderByDescending(i => i.CreatedDate)
            .ToListAsync();

        var serviceRevenue = await _context.ServiceHistories
            .AsNoTracking()
            .SumAsync(s => s.TotalCost);

        var totalSales = salesInvoices.Sum(i => i.TotalAmount);
        var totalPurchases = purchaseInvoices.Sum(i => i.TotalAmount);
        var netRevenue = totalSales + serviceRevenue - totalPurchases;

        return Document.Create(container =>
        {
            container.Page(page =>
            {
                ApplyPageDefaults(page);
                ComposeHeader(page, "Financial Report", "Revenue, purchases, service income, and net performance");

                page.Content().Column(column =>
                {
                    column.Spacing(18);

                    column.Item().Row(row =>
                    {
                        row.RelativeItem().Element(StatCard).Column(card =>
                        {
                            card.Item().Text("Sales Revenue").FontSize(10).FontColor(Colors.Grey.Darken1);
                            card.Item().Text(FormatCurrency(totalSales)).FontSize(18).Bold();
                        });
                        row.RelativeItem().Element(StatCard).Column(card =>
                        {
                            card.Item().Text("Service Revenue").FontSize(10).FontColor(Colors.Grey.Darken1);
                            card.Item().Text(FormatCurrency(serviceRevenue)).FontSize(18).Bold();
                        });
                        row.RelativeItem().Element(StatCard).Column(card =>
                        {
                            card.Item().Text("Purchases").FontSize(10).FontColor(Colors.Grey.Darken1);
                            card.Item().Text(FormatCurrency(totalPurchases)).FontSize(18).Bold();
                        });
                        row.RelativeItem().Element(StatCard).Column(card =>
                        {
                            card.Item().Text("Net Revenue").FontSize(10).FontColor(Colors.Grey.Darken1);
                            card.Item().Text(FormatCurrency(netRevenue)).FontSize(18).Bold();
                        });
                    });

                    column.Item().Element(SectionTitle).Text("Recent Sales Invoices");
                    column.Item().Table(table =>
                    {
                        table.ColumnsDefinition(columns =>
                        {
                            columns.RelativeColumn(1.3f);
                            columns.RelativeColumn(2);
                            columns.RelativeColumn(2);
                            columns.RelativeColumn(1.4f);
                            columns.RelativeColumn(1.4f);
                        });

                        ComposeTableHeader(table, "Date", "Customer", "Staff", "Discount", "Total");

                        foreach (var invoice in salesInvoices.Take(12))
                        {
                            table.Cell().Element(TableCell).Text(FormatDate(invoice.CreatedAt));
                            table.Cell().Element(TableCell).Text(GetUserName(invoice.Customer?.User));
                            table.Cell().Element(TableCell).Text(GetUserName(invoice.Staff?.User));
                            table.Cell().Element(TableCell).AlignRight().Text(FormatCurrency(invoice.DiscountAmount));
                            table.Cell().Element(TableCell).AlignRight().Text(FormatCurrency(invoice.TotalAmount));
                        }
                    });

                    column.Item().Element(SectionTitle).Text("Recent Purchase Invoices");
                    column.Item().Table(table =>
                    {
                        table.ColumnsDefinition(columns =>
                        {
                            columns.RelativeColumn(1.3f);
                            columns.RelativeColumn(2);
                            columns.RelativeColumn(2);
                            columns.RelativeColumn(1.5f);
                        });

                        ComposeTableHeader(table, "Date", "Invoice No.", "Vendor", "Total");

                        foreach (var invoice in purchaseInvoices.Take(12))
                        {
                            table.Cell().Element(TableCell).Text(FormatDate(invoice.CreatedDate));
                            table.Cell().Element(TableCell).Text(invoice.InvoiceNumber ?? "N/A");
                            table.Cell().Element(TableCell).Text(invoice.Vendor?.Name ?? "Unknown Vendor");
                            table.Cell().Element(TableCell).AlignRight().Text(FormatCurrency(invoice.TotalAmount));
                        }
                    });
                });

                ComposeFooter(page);
            });
        }).GeneratePdf();
    }

    public async Task<byte[]> GenerateCustomerReportAsync()
    {
        var customers = await _context.Customers
            .AsNoTracking()
            .Include(c => c.User)
            .Include(c => c.Vehicles)
            .Include(c => c.SalesServicesInvoices)
            .Include(c => c.ServiceHistories)
            .OrderBy(c => c.User.FirstName)
            .ThenBy(c => c.User.LastName)
            .ToListAsync();

        var totalSpent = customers.Sum(c => c.TotalSpent);
        var pendingCredits = customers.Sum(c => c.PendingCredits);
        var totalVehicles = customers.Sum(c => c.Vehicles.Count);
        var activeCustomers = customers.Count(c => c.User.IsActive);

        return Document.Create(container =>
        {
            container.Page(page =>
            {
                ApplyPageDefaults(page);
                ComposeHeader(page, "Customer Report", "Customer activity, vehicles, spending, and outstanding credits");

                page.Content().Column(column =>
                {
                    column.Spacing(18);

                    column.Item().Row(row =>
                    {
                        row.RelativeItem().Element(StatCard).Column(card =>
                        {
                            card.Item().Text("Customers").FontSize(10).FontColor(Colors.Grey.Darken1);
                            card.Item().Text(customers.Count.ToString()).FontSize(18).Bold();
                        });
                        row.RelativeItem().Element(StatCard).Column(card =>
                        {
                            card.Item().Text("Active").FontSize(10).FontColor(Colors.Grey.Darken1);
                            card.Item().Text(activeCustomers.ToString()).FontSize(18).Bold();
                        });
                        row.RelativeItem().Element(StatCard).Column(card =>
                        {
                            card.Item().Text("Vehicles").FontSize(10).FontColor(Colors.Grey.Darken1);
                            card.Item().Text(totalVehicles.ToString()).FontSize(18).Bold();
                        });
                        row.RelativeItem().Element(StatCard).Column(card =>
                        {
                            card.Item().Text("Pending Credits").FontSize(10).FontColor(Colors.Grey.Darken1);
                            card.Item().Text(FormatCurrency(pendingCredits)).FontSize(18).Bold();
                        });
                    });

                    column.Item().Element(SectionTitle).Text("Customer Summary");
                    column.Item().Table(table =>
                    {
                        table.ColumnsDefinition(columns =>
                        {
                            columns.RelativeColumn(2);
                            columns.RelativeColumn(2);
                            columns.RelativeColumn(1.2f);
                            columns.RelativeColumn(1.3f);
                            columns.RelativeColumn(1.4f);
                            columns.RelativeColumn(1.4f);
                        });

                        ComposeTableHeader(table, "Customer", "Contact", "Vehicles", "Services", "Total Spent", "Credits");

                        foreach (var customer in customers.Take(24))
                        {
                            table.Cell().Element(TableCell).Text(GetUserName(customer.User));
                            table.Cell().Element(TableCell).Text(customer.User.PhoneNumber ?? customer.User.Email ?? "N/A");
                            table.Cell().Element(TableCell).AlignRight().Text(customer.Vehicles.Count.ToString());
                            table.Cell().Element(TableCell).AlignRight().Text(customer.ServiceHistories.Count.ToString());
                            table.Cell().Element(TableCell).AlignRight().Text(FormatCurrency(customer.TotalSpent));
                            table.Cell().Element(TableCell).AlignRight().Text(FormatCurrency(customer.PendingCredits));
                        }
                    });

                    column.Item().Text($"Total customer spending tracked: {FormatCurrency(totalSpent)}")
                        .FontSize(11)
                        .FontColor(Colors.Grey.Darken2);
                });

                ComposeFooter(page);
            });
        }).GeneratePdf();
    }

    private static void ApplyPageDefaults(PageDescriptor page)
    {
        page.Size(PageSizes.A4);
        page.Margin(36);
        page.DefaultTextStyle(text => text.FontSize(9).FontFamily(Fonts.Arial));
    }

    private static void ComposeHeader(PageDescriptor page, string title, string subtitle)
    {
        page.Header().Column(column =>
        {
            column.Item().Text("Gearbox").FontSize(14).Bold().FontColor(Colors.Teal.Darken4);
            column.Item().Text(title).FontSize(22).Bold();
            column.Item().Text(subtitle).FontSize(10).FontColor(Colors.Grey.Darken1);
            column.Item().Text($"Generated on {DateTime.UtcNow:yyyy-MM-dd HH:mm} UTC")
                .FontSize(8)
                .FontColor(Colors.Grey.Darken1);
            column.Item().PaddingTop(8).LineHorizontal(1).LineColor(Colors.Grey.Lighten2);
        });
    }

    private static void ComposeFooter(PageDescriptor page)
    {
        page.Footer().AlignCenter().Text(text =>
        {
            text.Span("Page ");
            text.CurrentPageNumber();
            text.Span(" of ");
            text.TotalPages();
        });
    }

    private static void ComposeTableHeader(TableDescriptor table, params string[] headers)
    {
        foreach (var header in headers)
        {
            table.Cell().Element(TableHeaderCell).Text(header);
        }
    }

    private static IContainer StatCard(IContainer container)
    {
        return container
            .Border(1)
            .BorderColor(Colors.Grey.Lighten2)
            .Background(Colors.Grey.Lighten5)
            .Padding(10);
    }

    private static IContainer SectionTitle(IContainer container)
    {
        return container.PaddingTop(4).PaddingBottom(4).DefaultTextStyle(text => text.FontSize(13).Bold());
    }

    private static IContainer TableHeaderCell(IContainer container)
    {
        return container
            .Background(Colors.Teal.Darken4)
            .PaddingVertical(6)
            .PaddingHorizontal(5)
            .DefaultTextStyle(text => text.FontColor(Colors.White).Bold());
    }

    private static IContainer TableCell(IContainer container)
    {
        return container
            .BorderBottom(1)
            .BorderColor(Colors.Grey.Lighten2)
            .PaddingVertical(5)
            .PaddingHorizontal(5);
    }

    private static string FormatCurrency(decimal value)
    {
        return $"Rs. {value:N2}";
    }

    private static string FormatDate(DateTime value)
    {
        return value.ToString("yyyy-MM-dd");
    }

    private static string GetUserName(Gearbox.Domain.Entities.AppUser? user)
    {
        if (user == null)
        {
            return "Unknown";
        }

        var fullName = $"{user.FirstName} {user.LastName}".Trim();
        return string.IsNullOrWhiteSpace(fullName) ? user.UserName ?? user.Email ?? "Unknown" : fullName;
    }
}
