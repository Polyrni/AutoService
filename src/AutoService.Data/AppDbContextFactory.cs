using Microsoft.EntityFrameworkCore.Design;

namespace AutoService.Data;

public class AppDbContextFactory : IDesignTimeDbContextFactory<AppDbContext>
{
    public AppDbContext CreateDbContext(string[] args) => Db.CreateContext();
}