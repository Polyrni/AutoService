using AutoService.Data.Models;
using Microsoft.EntityFrameworkCore;

namespace AutoService.Data
{
    public class AppDbContext : DbContext
    {
        public DbSet<Customer> Customers => Set<Customer>();

        public AppDbContext(DbContextOptions<AppDbContext> options) 
            : base(options) { }
    }
}
