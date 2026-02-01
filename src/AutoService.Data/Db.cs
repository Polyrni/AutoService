using Microsoft.EntityFrameworkCore;

namespace AutoService.Data;

public static class Db
{
    public const string ConnectionString = "Data Source=autoservice.db";

    public static DbContextOptions<AppDbContext> CreateOptions()
        => new DbContextOptionsBuilder<AppDbContext>()
            .UseSqlite(ConnectionString)
            .Options;

    public static AppDbContext CreateContext()
        => new AppDbContext(CreateOptions());
}