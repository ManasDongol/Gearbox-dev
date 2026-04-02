using Gearbox.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Gearbox.Infrastructure.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options) { }

        public DbSet<User> Users { get; set; }
        public DbSet<Customer> Customers { get; set; }
        public DbSet<Staff> Staffs { get; set; }
        public DbSet<Vehicle> Vehicles { get; set; }
        public DbSet<Part> Parts { get; set; }
        public DbSet<Vendor> Vendors { get; set; }
        public DbSet<SalesInvoice> SalesInvoices { get; set; }
        public DbSet<SalesInvoiceItem> SalesInvoiceItems { get; set; }
        public DbSet<PurchaseInvoice> PurchaseInvoices { get; set; }
        public DbSet<PurchaseInvoiceItem> PurchaseInvoiceItems { get; set; }
        public DbSet<Appointment> Appointments { get; set; }
        public DbSet<ServiceDetails> ServiceDetails { get; set; } // Plural Dbset name might just be ServiceDetails
        public DbSet<ServiceHistory> ServiceHistories { get; set; }
        public DbSet<ServiceBill> ServiceBills { get; set; }
        public DbSet<ServiceReview> ServiceReviews { get; set; }
        public DbSet<PartRequest> PartRequests { get; set; }
        public DbSet<Notification> Notifications { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // User - Customer (1:1)
            modelBuilder.Entity<Customer>()
                .HasOne(c => c.User)
                .WithOne(u => u.Customer)
                .HasForeignKey<Customer>(c => c.UserId)
                .OnDelete(DeleteBehavior.Restrict);

            // User - Staff (1:1)
            modelBuilder.Entity<Staff>()
                .HasOne(s => s.User)
                .WithOne(u => u.Staff)
                .HasForeignKey<Staff>(s => s.UserId)
                .OnDelete(DeleteBehavior.Restrict);

            // User - Notification (1:Many)
            modelBuilder.Entity<User>()
                .HasMany(u => u.Notifications)
                .WithOne(n => n.User)
                .HasForeignKey(n => n.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            // Customer - Vehicle (1:Many)
            modelBuilder.Entity<Customer>()
                .HasMany(c => c.Vehicles)
                .WithOne(v => v.Customer)
                .HasForeignKey(v => v.CustomerId)
                .OnDelete(DeleteBehavior.Cascade);

            // Customer - SalesInvoice (1:Many)
            modelBuilder.Entity<Customer>()
                .HasMany(c => c.SalesInvoices)
                .WithOne(si => si.Customer)
                .HasForeignKey(si => si.CustomerId)
                .OnDelete(DeleteBehavior.Restrict); // Prevent deleting customer if they have invoices

            // Staff - SalesInvoice (1:Many)
            modelBuilder.Entity<Staff>()
                .HasMany(s => s.SalesInvoices)
                .WithOne(si => si.Staff)
                .HasForeignKey(si => si.StaffId)
                .OnDelete(DeleteBehavior.Restrict);

            // SalesInvoice - SalesInvoiceItem (1:Many)
            modelBuilder.Entity<SalesInvoice>()
                .HasMany(si => si.Items)
                .WithOne(sii => sii.SalesInvoice)
                .HasForeignKey(sii => sii.SalesInvoiceId)
                .OnDelete(DeleteBehavior.Cascade);

            // Part - SalesInvoiceItem (1:Many) => handled via Item -> Part relationship
            modelBuilder.Entity<SalesInvoiceItem>()
                .HasOne(sii => sii.Part)
                .WithMany()
                .HasForeignKey(sii => sii.PartId)
                .OnDelete(DeleteBehavior.Restrict);

            // Vendor - Part (1:Many)
            modelBuilder.Entity<Vendor>()
                .HasMany(v => v.Parts)
                .WithOne(p => p.Vendor)
                .HasForeignKey(p => p.VendorId)
                .OnDelete(DeleteBehavior.Restrict);

            // Vendor - PurchaseInvoice (1:Many)
            modelBuilder.Entity<Vendor>()
                .HasMany(v => v.PurchaseInvoices)
                .WithOne(pi => pi.Vendor)
                .HasForeignKey(pi => pi.VendorId)
                .OnDelete(DeleteBehavior.Restrict);

            // PurchaseInvoice - PurchaseInvoiceItem (1:Many)
            modelBuilder.Entity<PurchaseInvoice>()
                .HasMany(pi => pi.Items)
                .WithOne(pii => pii.PurchaseInvoice)
                .HasForeignKey(pii => pii.PurchaseInvoiceId)
                .OnDelete(DeleteBehavior.Cascade);

            // Part - PurchaseInvoiceItem (1:Many) => handled via Item -> Part
            modelBuilder.Entity<PurchaseInvoiceItem>()
                .HasOne(pii => pii.Part)
                .WithMany()
                .HasForeignKey(pii => pii.PartId)
                .OnDelete(DeleteBehavior.Restrict);

            // Customer - Appointment (1:Many)
            modelBuilder.Entity<Customer>()
                .HasMany(c => c.Appointments)
                .WithOne(a => a.Customer)
                .HasForeignKey(a => a.CustomerId)
                .OnDelete(DeleteBehavior.Restrict);

            // Vehicle - Appointment (1:Many)
            modelBuilder.Entity<Vehicle>()
                .HasMany(v => v.Appointments)
                .WithOne(a => a.Vehicle)
                .HasForeignKey(a => a.VehicleId)
                .OnDelete(DeleteBehavior.Restrict);

            // Appointment - ServiceDetails (Many:Many)
            modelBuilder.Entity<Appointment>()
                .HasMany(a => a.Services)
                .WithMany(s => s.Appointments)
                .UsingEntity(j => j.ToTable("AppointmentServiceDetails"));

            // Customer - ServiceHistory (1:Many)
            modelBuilder.Entity<Customer>()
                .HasMany(c => c.ServiceHistories)
                .WithOne(sh => sh.Customer)
                .HasForeignKey(sh => sh.CustomerId)
                .OnDelete(DeleteBehavior.Restrict);

            // Vehicle - ServiceHistory (1:Many)
            modelBuilder.Entity<Vehicle>()
                .HasMany(v => v.ServiceHistories)
                .WithOne(sh => sh.Vehicle)
                .HasForeignKey(sh => sh.VehicleId)
                .OnDelete(DeleteBehavior.Restrict);

            // ServiceHistory - ServiceDetails (Many:Many)
            modelBuilder.Entity<ServiceHistory>()
                .HasMany(sh => sh.Services)
                .WithMany(s => s.ServiceHistories)
                .UsingEntity(j => j.ToTable("ServiceHistoryServiceDetails"));

            // Appointment/ServiceHistory - ServiceBill (Optional FKs)
            modelBuilder.Entity<ServiceBill>()
                .HasOne(sb => sb.Appointment)
                .WithMany()
                .HasForeignKey(sb => sb.AppointmentId)
                .OnDelete(DeleteBehavior.SetNull);

            modelBuilder.Entity<ServiceBill>()
                .HasOne(sb => sb.ServiceHistory)
                .WithMany()
                .HasForeignKey(sb => sb.ServiceHistoryId)
                .OnDelete(DeleteBehavior.SetNull);

            // Customer - ServiceReview (1:Many)
            modelBuilder.Entity<Customer>()
                .HasMany(c => c.ServiceReviews)
                .WithOne(sr => sr.Customer)
                .HasForeignKey(sr => sr.CustomerId)
                .OnDelete(DeleteBehavior.Cascade);

            // Appointment - ServiceReview (1:Many / Optional)
            modelBuilder.Entity<ServiceReview>()
                .HasOne(sr => sr.Appointment)
                .WithMany()
                .HasForeignKey(sr => sr.AppointmentId)
                .OnDelete(DeleteBehavior.SetNull);

            // Customer - PartRequest (1:Many)
            modelBuilder.Entity<Customer>()
                .HasMany(c => c.PartRequests)
                .WithOne(pr => pr.Customer)
                .HasForeignKey(pr => pr.CustomerId)
                .OnDelete(DeleteBehavior.Cascade);
                
            // Precision for decimals
            modelBuilder.Entity<Part>().Property(p => p.SellingPrice).HasColumnType("decimal(18,2)");
            modelBuilder.Entity<Customer>().Property(c => c.TotalSpent).HasColumnType("decimal(18,2)");
            modelBuilder.Entity<Customer>().Property(c => c.PendingCredits).HasColumnType("decimal(18,2)");
            modelBuilder.Entity<SalesInvoice>().Property(si => si.TotalAmount).HasColumnType("decimal(18,2)");
            modelBuilder.Entity<SalesInvoice>().Property(si => si.DiscountAmount).HasColumnType("decimal(18,2)");
            modelBuilder.Entity<SalesInvoiceItem>().Property(sii => sii.UnitPrice).HasColumnType("decimal(18,2)");
            modelBuilder.Entity<PurchaseInvoice>().Property(pi => pi.TotalAmount).HasColumnType("decimal(18,2)");
            modelBuilder.Entity<PurchaseInvoiceItem>().Property(pii => pii.CostPrice).HasColumnType("decimal(18,2)");
            modelBuilder.Entity<ServiceDetails>().Property(sd => sd.BasePrice).HasColumnType("decimal(18,2)");
            modelBuilder.Entity<ServiceHistory>().Property(sh => sh.TotalCost).HasColumnType("decimal(18,2)");
            modelBuilder.Entity<ServiceBill>().Property(sb => sb.TotalAmount).HasColumnType("decimal(18,2)");
        }
    }
}
