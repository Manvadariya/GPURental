using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using GPURental.Data;
using Microsoft.EntityFrameworkCore;
using GPURental.Models; // <-- ADD this to get access to the User class
using Microsoft.AspNetCore.Identity; // <-- ADD this for Identity

namespace GPURental
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }


        public IConfiguration Configuration { get; }

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddDbContext<AppDbContext>(options =>
                options.UseSqlServer(
                    Configuration.GetConnectionString("DefaultConnection")));

            // Use the full AddIdentity to include Role services
            services.AddIdentity<User, IdentityRole>()
                .AddEntityFrameworkStores<AppDbContext>()
                .AddDefaultTokenProviders();

            services.AddControllersWithViews();

            // --- ADD THIS LINE ---
            // This re-adds the services for the Identity UI Razor Pages that
            // AddDefaultIdentity used to provide automatically.
            services.AddRazorPages();
            // ---------------------
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
                app.UseHsts();
            }
            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRouting();

            // --- ADD THIS LINE FOR AUTHENTICATION ---
            // This middleware is responsible for establishing the user's identity.
            app.UseAuthentication();
            // ------------------------------------------

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller=Home}/{action=Index}/{id?}");
                // Add this line so the Identity UI pages (like Login, Register) work
                endpoints.MapRazorPages();
            });
        }
    }
}