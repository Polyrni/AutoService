namespace AutoService.Data.Models.Orders
{
    public class Order
    {
        public int Id { get; set; }

        public string OrderNumber { get; set; } // Автогенерируемый номер заказа

        public int CustomerId { get; set; }
        public Customer Customer { get; set; }

        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }

        public decimal TotalAmount { get; set; }

        public string? Note { get; set; }

        public ICollection<OrderService> OrderServices { get; set; }
        public ICollection<OrderEmployee> OrderEmployees { get; set; }
    }
}
