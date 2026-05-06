using System.Security.Claims;
using System.Text;
using System.Threading.RateLimiting;
using Gearbox.Application.BackgroundJobs;
using Gearbox.Application.BackgroundJobs.Email;
using Gearbox.Domain.Entities;
using Gearbox.Infrastructure.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Scalar.AspNetCore;
using Gearbox.Domain.Interfaces;
using Gearbox.Application.Interfaces;
using  Gearbox.Infrastructure.Repositories;
using  Gearbox.Application.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.RateLimiting;
using Microsoft.IdentityModel.Tokens;

var builder = WebApplication.CreateBuilder(args);


builder.Services.AddOpenApi();

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAngularApp",
        policy =>
        {
            policy.WithOrigins("http://localhost:4200") // Angular dev server URL
                .AllowAnyHeader()
                .AllowAnyMethod()
                .AllowCredentials();
        });
});
builder.Services.AddDbContext<ApplicationDbContext>(
    options=>options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection"))
    
    );


builder.Services.AddRateLimiter(options =>
{
    options.AddFixedWindowLimiter("fixed", opt =>
    {
        opt.PermitLimit = 4;
        opt.Window = TimeSpan.FromSeconds(10);
        opt.QueueProcessingOrder = QueueProcessingOrder.OldestFirst;
        opt.QueueLimit = 2;

    });
});

builder.Services.AddIdentity<AppUser,IdentityRole<Guid>>(options =>
    {
        options.User.RequireUniqueEmail = true;
    })
    .AddEntityFrameworkStores<ApplicationDbContext>()
    .AddDefaultTokenProviders();
builder.Services.AddControllers();



var jwtSettings = builder.Configuration.GetSection("Jwt");
var key = Encoding.UTF8.GetBytes(jwtSettings["Key"]);

builder.Services.AddAuthentication(options =>
    {
        options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
        options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
    })
    .AddJwtBearer(options =>
    {
        Console.WriteLine(key);
        Console.WriteLine("HAHDHSAHSHSD");
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,

            ValidIssuer =builder.Configuration["Jwt:Issuer"],
            ValidAudience = builder.Configuration["Jwt:Audience"],
            IssuerSigningKey = new SymmetricSecurityKey(key),
            
            
            NameClaimType = ClaimTypes.Name,
            RoleClaimType = ClaimTypes.Role
        };

        options.Events = new JwtBearerEvents
        {
            OnMessageReceived = context =>
            {
              /*  {
                    var auth = context.Request.Headers["Authorization"].FirstOrDefault();

                    if (!string.IsNullOrEmpty(auth) && auth.StartsWith("Bearer "))
                    {
                        context.Token = auth.Substring("Bearer ".Length).Trim();
                    }

                    return Task.CompletedTask;
                }*/
                
                  var token = context.Request.Cookies["jwt"];

                  // 2. fallback: Authorization header (important for Angular dev)
                  if (string.IsNullOrEmpty(token))
                  {
                      var authHeader = context.Request.Headers["Authorization"].FirstOrDefault();
                      if (!string.IsNullOrEmpty(authHeader) && authHeader.StartsWith("Bearer "))
                      {
                          token = authHeader.Substring("Bearer ".Length).Trim();
                      }
                  }

                  if (!string.IsNullOrEmpty(token))
                  {
                      context.Token = token;
                  }

                  return Task.CompletedTask;
              },
              OnAuthenticationFailed = context =>
              {
                  Console.WriteLine($"Authentication failed: {context.Exception}");
                  Console.WriteLine($"Authentication failed: {context.Exception.Message}");
                  Console.WriteLine($"Inner: {context.Exception.InnerException?.Message}");
                  return Task.CompletedTask;

              } 
            
        };
    });

// Repositories
builder.Services.AddScoped(typeof(IGenericRepository<>), typeof(GenericRepository<>));
builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<ICustomerRepository, CustomerRepository>();
builder.Services.AddScoped<IStaffRepository, StaffRepository>();
builder.Services.AddScoped<IVehicleRepository, VehicleRepository>();
builder.Services.AddScoped<IPartRepository, PartRepository>();
builder.Services.AddScoped<IVendorRepository, VendorRepository>();
builder.Services.AddScoped<ISalesInvoiceRepository, SalesInvoiceRepository>();
builder.Services.AddScoped<IPurchaseInvoiceRepository, PurchaseInvoiceRepository>();
builder.Services.AddScoped<IAppointmentRepository, AppointmentRepository>();

builder.Services.AddScoped<IServiceReviewRepository, ServiceReviewRepository>();
builder.Services.AddScoped<IPartRequestRepository, PartRequestRepository>();
builder.Services.AddScoped<INotificationRepository, NotificationRepository>();
builder.Services.AddScoped<IPurchaseInvoiceItemRepository, PurchaseInvoiceItemRepository>();
builder.Services.AddScoped<ISalesInvoiceItemRepository, SalesInvoiceItemRepository>();
builder.Services.AddScoped<ISalesServicesInvoiceRepository, SalesServicesInvoiceRepository>();
builder.Services.AddScoped<ISalesServicesInvoiceItemRepository, SalesServicesInvoiceItemRepository>();
builder.Services.AddScoped<IServiceRepository, ServiceRepository>();
builder.Services.AddScoped<AuthRepository>();

builder.Services.AddScoped<AuthService>();

// Services
builder.Services.AddScoped<IUserService,UserService>();
builder.Services.AddScoped<ICustomerService,CustomerService>();
builder.Services.AddScoped<IStaffService,StaffService>();
builder.Services.AddScoped<IVehicleService,VehicleService>();
builder.Services.AddScoped<IPartService,PartService>();
builder.Services.AddScoped<IVendorService,VendorService>();

builder.Services.AddScoped<IPurchaseInvoiceService,PurchaseInvoiceService>();
builder.Services.AddScoped<IAppointmentService,AppointmentService>();

builder.Services.AddScoped<IServiceReviewService,ServiceReviewService>();
builder.Services.AddScoped<IPartRequestService,PartRequestService>();
builder.Services.AddScoped<INotificationService,NotificationService>();
builder.Services.AddScoped<IPurchaseInvoiceItemService,PurchaseInvoiceItemService>();

builder.Services.AddScoped<ISalesServicesInvoiceService, SalesServicesInvoiceService>();
builder.Services.AddScoped<ISalesServicesInvoiceItemService, SalesServicesInvoiceItemService>();
builder.Services.AddScoped<IServiceService, ServiceService>();
builder.Services.AddScoped<TokenService>();

builder.Services.AddSingleton<EmailQueue>();
builder.Services.AddSingleton<EmailTemplateService>();
builder.Services.AddSingleton<EmailService>();

builder.Services.AddHostedService<EmailBackgroundWorker>();


var app = builder.Build();


using (var scope = app.Services.CreateScope())
{
    var userManager = scope.ServiceProvider.GetRequiredService<UserManager<AppUser>>();
    var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole<Guid>>>();

    // Seed roles
    var roles = new[] { "Admin", "Staff", "Customer" };
    foreach (var roleName in roles)
    {
        if (!await roleManager.RoleExistsAsync(roleName))
        {
            await roleManager.CreateAsync(new IdentityRole<Guid>(roleName));
        }
    }

    // Seed default admin
    var adminEmail = "admin@gearbox.com";
    if (await userManager.FindByEmailAsync(adminEmail) == null)
    {
        var adminUser = new AppUser
        {
            UserName = "admin",
            Email = adminEmail,
            EmailConfirmed = true,
            IsActive = true,
            CreatedAt = DateTime.UtcNow
        };
        await userManager.CreateAsync(adminUser, "Admin@123"); // temporary password
        await userManager.AddToRoleAsync(adminUser, "Admin");
    }
}

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.MapScalarApiReference();
}

app.UseRateLimiter();
app.UseCors("AllowAngularApp");
app.UseAuthentication();
app.UseAuthorization();
//app.UseHttpsRedirection();

app.MapControllers();

app.Run();

