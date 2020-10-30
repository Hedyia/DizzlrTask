using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DizzlrApp.Areas.Identity.Data;
using DizzlrApp.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace DizzlrApp.Data
{
    public class AppContext : IdentityDbContext<ApplicationUser>
    {
        public AppContext(DbContextOptions<AppContext> options)
            : base(options)
        {
        }
        public DbSet<File> FilesOnFileSystem { get; set; }
        public DbSet<Order> Orders { get; set; }
        public DbSet<Status> Statuses { get; set; }
        public DbSet<OrderFileItem> OrderFileItems { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);
            // Customize the ASP.NET Identity model and override the defaults if needed.
            // For example, you can rename the ASP.NET Identity table names and more.
            // Add your customizations after calling base.OnModelCreating(builder);
            builder.Entity<Status>().HasData(
                new Status { StatusId = 1, Name = "In Progress" },
                new Status { StatusId = 2, Name = "Completed"});
        }
    }
}
