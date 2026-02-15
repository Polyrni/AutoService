using AutoService.Data;
using AutoService.Data.Models.Orders;
using Microsoft.EntityFrameworkCore;

namespace AutoService.Managers
{
    public class OrdersManager
    {
        private readonly AppDbContext _db;

        public OrdersManager(AppDbContext db)
        {
            _db = db;
        }

        public Order CreateOrder(Order order, IEnumerable<OrderService> services, IEnumerable<int> employeeIds)
        {
            using var transaction = _db.Database.BeginTransaction();

            try
            {
                // Сохраняем заказ
                _db.Orders.Add(order);
                _db.SaveChanges();

                // Добавляем услуги
                foreach (var service in services)
                {
                    service.OrderId = order.Id;
                    _db.OrderServices.Add(service);
                }

                // Добавляем сотрудников
                foreach (var employeeId in employeeIds)
                {
                    _db.OrderEmployees.Add(new OrderEmployee
                    {
                        OrderId = order.Id,
                        EmployeeId = employeeId
                    });
                }

                _db.SaveChanges();
                transaction.Commit();

                return order;
            }
            catch
            {
                transaction.Rollback();
                throw;
            }
        }

        public Order UpdateOrder(Order order, IEnumerable<OrderService> services, IEnumerable<int> employeeIds)
        {
            using var transaction = _db.Database.BeginTransaction();

            try
            {
                // Обновляем основные данные заказа
                _db.Orders.Update(order);

                // Удаляем старые связи
                var oldServices = _db.OrderServices
                    .Where(os => os.OrderId == order.Id)
                    .ToList();
                _db.OrderServices.RemoveRange(oldServices);

                var oldEmployees = _db.OrderEmployees
                    .Where(oe => oe.OrderId == order.Id)
                    .ToList();
                _db.OrderEmployees.RemoveRange(oldEmployees);

                _db.SaveChanges();

                // Добавляем новые
                foreach (var service in services)
                {
                    service.OrderId = order.Id;
                    service.Id = 0; // Сбрасываем ID для новой записи
                    _db.OrderServices.Add(service);
                }

                foreach (var employeeId in employeeIds)
                {
                    _db.OrderEmployees.Add(new OrderEmployee
                    {
                        OrderId = order.Id,
                        EmployeeId = employeeId
                    });
                }

                _db.SaveChanges();
                transaction.Commit();

                return order;
            }
            catch
            {
                transaction.Rollback();
                throw;
            }
        }

        public bool DeleteOrder(int orderId)
        {
            var order = _db.Orders.Find(orderId);
            if (order == null)
                return false;

            _db.Orders.Remove(order);
            _db.SaveChanges();
            return true;
        }

        public string GenerateOrderNumber()
        {
            var date = DateTime.Now.ToString("yyyyMMdd");
            var lastOrderToday = _db.Orders
                .Where(o => o.CreatedAt.Date == DateTime.Now.Date)
                .Count();

            return $"ORD-{date}-{(lastOrderToday + 1):D3}";
        }

        public Order? GetOrderWithDetails(int orderId)
        {
            return _db.Orders
                .Include(o => o.Customer)
                .Include(o => o.OrderServices)
                    .ThenInclude(os => os.Service)
                .Include(o => o.OrderEmployees)
                    .ThenInclude(oe => oe.Employee)
                .FirstOrDefault(o => o.Id == orderId);
        }

        public List<Order> GetAllOrdersWithDetails()
        {
            return _db.Orders
                .Include(o => o.Customer)
                .Include(o => o.OrderServices)
                    .ThenInclude(os => os.Service)
                .Include(o => o.OrderEmployees)
                    .ThenInclude(oe => oe.Employee)
                .OrderByDescending(o => o.CreatedAt)
                .ToList();
        }

        public decimal GetServicePrice(int serviceId)
        {
            var service = _db.Services.Find(serviceId);
            return service != null ? 1000 : 0;
        }
    }
}