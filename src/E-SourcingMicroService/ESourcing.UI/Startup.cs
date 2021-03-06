using ESourcing.Core.Entities;
using ESourcing.Core.Repositories;
using ESourcing.Core.Repositories.Base;
using ESourcing.Infrastructure.Data;
using ESourcing.Infrastructure.Repository;
using ESourcing.Infrastructure.Repository.Base;
using ESourcing.UI.Clients;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;

namespace ESourcing.UI
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
            services.AddDbContext<WebAppContext>(options => options.UseSqlServer(Configuration.GetConnectionString("IdentityConnection")));
            services.AddIdentity<AppUser, IdentityRole>(opt =>
            {
                opt.Password.RequiredLength = 4;
                opt.Password.RequireNonAlphanumeric = false;
                opt.Password.RequireLowercase = false;
                opt.Password.RequireUppercase = false;
                opt.Password.RequireDigit = false;
            }).AddDefaultTokenProviders().AddEntityFrameworkStores<WebAppContext>();

            services.AddControllersWithViews().AddRazorRuntimeCompilation();
            services.AddMvc();
            services.AddRazorPages();
            services.AddSession(opt =>
            {
                opt.IdleTimeout = TimeSpan.FromMinutes(20);
            });

            services.AddScoped(typeof(IRepository<>), typeof(Repository<>));
            services.AddScoped(typeof(IUserRepository), typeof(UserRepository));

            //services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
            //        .AddCookie(options =>
            //        {
            //            options.Cookie.Name = "My Cookie";
            //            options.LoginPath = "Home/Login";
            //            options.LogoutPath = "Home/Logout";
            //            options.ExpireTimeSpan = TimeSpan.FromDays(3);
            //            options.SlidingExpiration = false;
            //        });

            services.ConfigureApplicationCookie(options =>
            {
                options.LoginPath = $"/Home/Login";
                options.LogoutPath = $"/Home/Logout";
            });

            services.AddHttpClient();

            services.AddHttpClient<ProductClient>();
            services.AddHttpClient<AuctionClient>();
            services.AddHttpClient<BidClient>();
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            app.UseSession();
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Error");
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRouting();

            app.UseAuthentication();
            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute(name: "default",
                      pattern: "{controller=Home}/{action=Index}/{id?}");
                endpoints.MapRazorPages();
            });
        }
    }
}