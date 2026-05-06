using Gearbox.Domain.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace Gearbox.Infrastructure.Data
{
    public class ApplicationDbContext : IdentityDbContext<AppUser, IdentityRole<Guid>, Guid>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options) { }

        public DbSet<AppUser> Users { get; set; }
        public DbSet<Customer> Customers { get; set; }
        public DbSet<Staff> Staffs { get; set; }
        public DbSet<Vehicle> Vehicles { get; set; }
        public DbSet<Part> Parts { get; set; }
        public DbSet<Vendor> Vendors { get; set; }
     
        public DbSet<PurchaseInvoice> PurchaseInvoices { get; set; }
        public DbSet<PurchaseInvoiceItem> PurchaseInvoiceItems { get; set; }
        public DbSet<Appointment> Appointments { get; set; }
        public DbSet<Service> Services { get; set; }
        public DbSet<ServiceHistory> ServiceHistories { get; set; }
     
        public DbSet<ServiceReview> ServiceReviews { get; set; }
        public DbSet<PartRequest> PartRequests { get; set; }
        public DbSet<Notification> Notifications { get; set; }
        public DbSet<SalesServicesInvoice> SalesServicesInvoices { get; set; }
        public DbSet<SalesServicesInvoiceItem> SalesServicesInvoiceItems { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.Entity<AppUser>().ToTable("Users");
            modelBuilder.Entity<IdentityRole<Guid>>().ToTable("Roles");
            modelBuilder.Entity<IdentityUserRole<Guid>>().ToTable("UserRoles");
            modelBuilder.Entity<IdentityUserClaim<Guid>>().ToTable("UserClaims");
            modelBuilder.Entity<IdentityUserLogin<Guid>>().ToTable("UserLogins");
            modelBuilder.Entity<IdentityRoleClaim<Guid>>().ToTable("RoleClaims");
            modelBuilder.Entity<IdentityUserToken<Guid>>().ToTable("UserTokens");


            modelBuilder.Entity<Customer>()
                .HasKey(c => c.UserId);
            // User - Customer (1:1)
            modelBuilder.Entity<Customer>()
                .HasOne(c => c.User)
                .WithOne(u => u.Customer)
                .HasForeignKey<Customer>(c => c.UserId)
                .OnDelete(DeleteBehavior.Cascade);
            
            modelBuilder.Entity<Staff>()
                .HasKey(c => c.UserId);

            // User - Staff (1:1)
            modelBuilder.Entity<Staff>()
                .HasOne(s => s.User)
                .WithOne(u => u.Staff)
                .HasForeignKey<Staff>(s => s.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            // User - Notification (1:Many)
            modelBuilder.Entity<AppUser>()
                .HasMany(u => u.Notifications)
                .WithOne(n => n.User)
                .HasForeignKey(n => n.UserId)
                .OnDelete(DeleteBehavior.Cascade);
            
            // User - Notification (1:Many)
            modelBuilder.Entity<AppUser>()
                .HasIndex(u => u.Email)
                .IsUnique();
            modelBuilder.Entity<AppUser>()
                .HasIndex(u => u.PhoneNumber)
                .IsUnique();
            


            // Customer - Vehicle (1:Many)
            modelBuilder.Entity<Customer>()
                .HasMany(c => c.Vehicles)
                .WithOne(v => v.Customer)
                .HasForeignKey(v => v.CustomerId)
                .OnDelete(DeleteBehavior.Cascade);

         

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
            
             
            modelBuilder.Entity<Customer>()
                .HasMany(c => c.SalesServicesInvoices)
                .WithOne(i => i.Customer)
                .HasForeignKey(i => i.CustomerId)
                .OnDelete(DeleteBehavior.Restrict);
            
            modelBuilder.Entity<Staff>()
                .HasMany(s => s.SalesServicesInvoices)
                .WithOne(i => i.Staff)
                .HasForeignKey(i => i.StaffId)
                .OnDelete(DeleteBehavior.Restrict);
            modelBuilder.Entity<SalesServicesInvoice>()
                .HasMany(i => i.Items)
                .WithOne(ii => ii.SalesServicesInvoice)
                .HasForeignKey(ii => ii.SalesServicesInvoiceId)
                .OnDelete(DeleteBehavior.Cascade);
            
            
     
            //service - service reviews
                
            // Precision for decimals
            modelBuilder.Entity<Part>().Property(p => p.SellingPrice).HasColumnType("decimal(18,2)");
            modelBuilder.Entity<Customer>().Property(c => c.TotalSpent).HasColumnType("decimal(18,2)");
            modelBuilder.Entity<Customer>().Property(c => c.PendingCredits).HasColumnType("decimal(18,2)");
            modelBuilder.Entity<SalesServicesInvoice>().Property(si => si.TotalAmount).HasColumnType("decimal(18,2)");
            modelBuilder.Entity<SalesServicesInvoice>().Property(si => si.DiscountAmount).HasColumnType("decimal(18,2)");
            modelBuilder.Entity<SalesServicesInvoiceItem>().Property(sii => sii.UnitPrice).HasColumnType("decimal(18,2)");
            modelBuilder.Entity<PurchaseInvoice>().Property(pi => pi.TotalAmount).HasColumnType("decimal(18,2)");
            modelBuilder.Entity<PurchaseInvoiceItem>().Property(pii => pii.CostPrice).HasColumnType("decimal(18,2)");
            modelBuilder.Entity<ServiceDetails>().Property(sd => sd.BasePrice).HasColumnType("decimal(18,2)");
            modelBuilder.Entity<ServiceHistory>().Property(sh => sh.TotalCost).HasColumnType("decimal(18,2)");
            modelBuilder.Entity<ServiceBill>().Property(sb => sb.TotalAmount).HasColumnType("decimal(18,2)");
        }
    }
}
