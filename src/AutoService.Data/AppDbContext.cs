using AutoService.Data.Models;
using AutoService.Data.Models.Orders;
using Microsoft.EntityFrameworkCore;

namespace AutoService.Data
{
    public class AppDbContext : DbContext
    {
        public DbSet<Customer> Customers => Set<Customer>();
        public DbSet<Employe> Employees => Set<Employe>();
        public DbSet<Service> Services => Set<Service>(); 
        public DbSet<Order> Orders => Set<Order>();
        public DbSet<OrderService> OrderServices => Set<OrderService>();
        public DbSet<OrderEmployee> OrderEmployees => Set<OrderEmployee>();

        public AppDbContext(DbContextOptions<AppDbContext> options) 
            : base(options) { }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Настройка для Order
            modelBuilder.Entity<Order>()
                .HasIndex(o => o.OrderNumber)
                .IsUnique();

            modelBuilder.Entity<Order>()
                .Property(o => o.OrderNumber)
                .HasMaxLength(20)
                .IsRequired();

            modelBuilder.Entity<Order>()
                .Property(o => o.CreatedAt)
                .HasDefaultValueSql("CURRENT_TIMESTAMP");

            // Связь Order с Customer
            modelBuilder.Entity<Order>()
                .HasOne(o => o.Customer)
                .WithMany()
                .HasForeignKey(o => o.CustomerId)
                .OnDelete(DeleteBehavior.Restrict); // Запрещаем удаление клиента, если есть заказы

            // Настройка для OrderService (связь многие-ко-многим Order-Service)
            modelBuilder.Entity<OrderService>()
                .HasKey(os => os.Id);

            modelBuilder.Entity<OrderService>()
                .HasOne(os => os.Order)
                .WithMany(o => o.OrderServices)
                .HasForeignKey(os => os.OrderId)
                .OnDelete(DeleteBehavior.Cascade); // При удалении заказа удаляются и связи

            modelBuilder.Entity<OrderService>()
                .HasOne(os => os.Service)
                .WithMany()
                .HasForeignKey(os => os.ServiceId)
                .OnDelete(DeleteBehavior.Restrict); // Запрещаем удаление услуги, если она используется в заказах

            modelBuilder.Entity<OrderService>()
                .Property(os => os.Price)
                .HasPrecision(18, 2); // Точность для денежных значений

            // Настройка для OrderEmployee (связь многие-ко-многим Order-Employee)
            modelBuilder.Entity<OrderEmployee>()
                .HasKey(oe => oe.Id);

            modelBuilder.Entity<OrderEmployee>()
                .HasOne(oe => oe.Order)
                .WithMany(o => o.OrderEmployees)
                .HasForeignKey(oe => oe.OrderId)
                .OnDelete(DeleteBehavior.Cascade); // При удалении заказа удаляются и связи

            modelBuilder.Entity<OrderEmployee>()
                .HasOne(oe => oe.Employee)
                .WithMany()
                .HasForeignKey(oe => oe.EmployeeId)
                .OnDelete(DeleteBehavior.Restrict); // Запрещаем удаление сотрудника, если он назначен на заказы

            // Индексы для улучшения производительности
            modelBuilder.Entity<Order>()
                .HasIndex(o => o.CreatedAt);

            modelBuilder.Entity<Order>()
                .HasIndex(o => o.CustomerId);
        }
    }
}
