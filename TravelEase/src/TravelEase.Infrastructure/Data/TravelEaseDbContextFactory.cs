using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;
using System.IO;

namespace TravelEase.TravelEase.Infrastructure.Data
{
    public class TravelEaseDbContextFactory : IDesignTimeDbContextFactory<TravelEaseDbContext>
    {
        public TravelEaseDbContext CreateDbContext(string[] args)
        {
            var config = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json", optional: false)
                .Build();

            var optionsBuilder = new DbContextOptionsBuilder<TravelEaseDbContext>();
            optionsBuilder.UseSqlServer(config.GetConnectionString("DefaultConnection"));

            return new TravelEaseDbContext(optionsBuilder.Options);
        }
    }
}