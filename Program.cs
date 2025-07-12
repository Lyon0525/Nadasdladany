using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Nadasdladany.Data;
using Nadasdladany.Models;
using System;
using System.Threading.Tasks;

namespace Nadasdladany
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.
            builder.Services.AddControllersWithViews();

            builder.Services.AddDbContext<NadasdladanyDbContext>(opts => opts.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

            builder.Services.AddIdentity<ApplicationUser, IdentityRole>(options =>
            {
                // Configure Identity options here if needed
                options.SignIn.RequireConfirmedAccount = false; // Set to true if you want email confirmation (not for this simple admin setup)
                options.Password.RequireDigit = true;
                options.Password.RequiredLength = 8;
                options.Password.RequireNonAlphanumeric = false;
                options.Password.RequireUppercase = true;
                options.Password.RequireLowercase = true;
                // options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(5);
                // options.Lockout.MaxFailedAccessAttempts = 5;
                // options.User.RequireUniqueEmail = true;
            }).AddEntityFrameworkStores<NadasdladanyDbContext>()
                .AddDefaultTokenProviders();

            builder.Services.ConfigureApplicationCookie(options =>
            {
                options.Cookie.HttpOnly = false;
                options.ExpireTimeSpan = TimeSpan.FromDays(30); // How long the login session is valid
                options.LoginPath = "/Account/Login";       // Path to your login page
                options.LogoutPath = "/Account/Logout";     // Path to your logout action
                options.AccessDeniedPath = "/Account/AccessDenied"; // Path if user tries to access restricted area
                options.SlidingExpiration = true;
            });

            var app = builder.Build();

            using (var scope = app.Services.CreateScope())
            {
                var services = scope.ServiceProvider;
                try
                {
                    var context = services.GetRequiredService<NadasdladanyDbContext>();
                    context.Database.Migrate();
                    // We will seed the first admin user and role here or via a separate utility
                    await SeedIdentityData(services); // Custom method to seed admin
                }
                catch (Exception ex)
                {
                    var logger = services.GetRequiredService<ILogger<Program>>();
                    logger.LogError(ex, "An error occurred while migrating or seeding the database.");
                }
            }

            // Configure the HTTP request pipeline.
            if (!app.Environment.IsDevelopment())
            {
                app.UseExceptionHandler("/Home/Error");
                app.UseHsts();
            }
            else
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRouting();

            app.UseAuthentication();
            app.UseAuthorization();

            app.MapControllerRoute(
                name: "newsDetail",
                pattern: "hirek/{slug}", 
                defaults: new { controller = "News", action = "Details" });

            app.MapControllerRoute(
                name: "default",
                pattern: "{controller=Home}/{action=Index}/{id?}");

            app.Run();

        }

        static async Task SeedIdentityData(IServiceProvider serviceProvider)
        {
            var userManager = serviceProvider.GetRequiredService<UserManager<ApplicationUser>>();
            var roleManager = serviceProvider.GetRequiredService<RoleManager<IdentityRole>>();

            string adminRoleName = "Administrator";
            string adminEmail = "admin@nadasdladany.hu"; 
            string adminPassword = "Password123!";      

            // Ensure Administrator role exists
            if (!await roleManager.RoleExistsAsync(adminRoleName))
            {
                await roleManager.CreateAsync(new IdentityRole(adminRoleName));
                Console.WriteLine($"Role '{adminRoleName}' created.");
            }

            // Ensure Admin user exists
            var adminUser = await userManager.FindByEmailAsync(adminEmail);
            if (adminUser == null)
            {
                adminUser = new ApplicationUser { UserName = adminEmail, Email = adminEmail, EmailConfirmed = true };
                var result = await userManager.CreateAsync(adminUser, adminPassword);
                if (result.Succeeded)
                {
                    await userManager.AddToRoleAsync(adminUser, adminRoleName);
                    Console.WriteLine($"User '{adminEmail}' created and assigned to '{adminRoleName}' role.");
                }
                else
                {
                    foreach (var error in result.Errors)
                    {
                        Console.WriteLine($"Error creating admin user: {error.Description}");
                    }
                }
            }
            else
            {
                Console.WriteLine($"User '{adminEmail}' already exists.");
                // Ensure user is in role if they exist but somehow aren't in it
                if (!await userManager.IsInRoleAsync(adminUser, adminRoleName))
                {
                    await userManager.AddToRoleAsync(adminUser, adminRoleName);
                    Console.WriteLine($"User '{adminEmail}' added to '{adminRoleName}' role.");
                }
            }
        }
    }
}
