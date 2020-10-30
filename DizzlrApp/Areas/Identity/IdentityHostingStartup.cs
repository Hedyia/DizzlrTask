using DizzlrApp.Areas.Identity.Data;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using AppContext = DizzlrApp.Data.AppContext;

[assembly: HostingStartup(typeof(DizzlrApp.Areas.Identity.IdentityHostingStartup))]
namespace DizzlrApp.Areas.Identity
{
    public class IdentityHostingStartup : IHostingStartup
    {
        public void Configure(IWebHostBuilder builder)
        {
            builder.ConfigureServices((context, services) =>
            {
                services.AddDbContext<AppContext>(options =>
                    options.UseSqlServer(
                        context.Configuration.GetConnectionString("AppContextConnection")));

                services.AddDefaultIdentity<ApplicationUser>(options => options.SignIn.RequireConfirmedAccount = false)
                .AddRoles<IdentityRole>()
                    .AddEntityFrameworkStores<AppContext>();
            });
        }
    }
}