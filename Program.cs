using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using TrainTickets.Data;
using TrainTickets.Models;

namespace TrainTickets
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.
            var connectionString = builder.Configuration.GetConnectionString("DefaultConnection") ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");
            builder.Services.AddDbContext<ApplicationDbContext>(options =>
                options.UseSqlServer(connectionString));
            builder.Services.AddDatabaseDeveloperPageExceptionFilter();

            builder.Services.AddDefaultIdentity<ApplicationUser>(options =>
            {
                options.SignIn.RequireConfirmedAccount = false;
                options.SignIn.RequireConfirmedEmail = false;
                options.SignIn.RequireConfirmedPhoneNumber = false;

                options.Password.RequireDigit = true;
                options.Password.RequireLowercase = true;
                options.Password.RequireUppercase = true;
                options.Password.RequireNonAlphanumeric = false;
                options.Password.RequiredLength = 6;
                options.Password.RequiredUniqueChars = 1;
            })
                .AddRoles<IdentityRole>()
                .AddEntityFrameworkStores<ApplicationDbContext>();
            builder.Services.AddControllersWithViews()
                .AddMvcOptions(options =>
                {
                    options.ModelBindingMessageProvider.SetValueMustNotBeNullAccessor(
                        _ => "Полето е задължително.");

                    options.ModelBindingMessageProvider.SetAttemptedValueIsInvalidAccessor(
                        (value, fieldName) => $"Невалидна стойност за полето {fieldName}.");

                    options.ModelBindingMessageProvider.SetMissingBindRequiredValueAccessor(
                        fieldName => $"Полето {fieldName} е задължително.");

                    options.ModelBindingMessageProvider.SetValueIsInvalidAccessor(
                        value => "Невалидна стойност.");

                    options.ModelBindingMessageProvider.SetValueMustBeANumberAccessor(
                        fieldName => $"Полето {fieldName} трябва да е число.");

                    options.ModelBindingMessageProvider.SetNonPropertyAttemptedValueIsInvalidAccessor(
                        value => "Невалидна стойност.");
                });

            builder.Services.ConfigureApplicationCookie(options =>
            {
                options.LoginPath = "/Identity/Account/Login";
                options.AccessDeniedPath = "/Identity/Account/AccessDenied";
            });

            var app = builder.Build();

            using (IServiceScope service = app.Services.CreateScope())
            {
                IServiceProvider provider = service.ServiceProvider;

                var context = provider.GetRequiredService<ApplicationDbContext>();
                await ContextSeed.SeedInfoAsync(context);

                RoleManager<IdentityRole> roleManager =
                    provider.GetRequiredService<RoleManager<IdentityRole>>();
                await ContextSeed.SeedRolesAsync(roleManager);

                UserManager<ApplicationUser> userManager =
                    provider.GetRequiredService<UserManager<ApplicationUser>>();
                await ContextSeed.SeedAdminAsync(userManager);
            }

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseMigrationsEndPoint();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseRouting();

            app.UseAuthorization();

            app.MapStaticAssets();
            app.MapControllerRoute(
                name: "default",
                pattern: "{controller=Home}/{action=Index}/{id?}")
                .WithStaticAssets();
            app.MapRazorPages()
               .WithStaticAssets();

            app.Run();
        }
    }
}
