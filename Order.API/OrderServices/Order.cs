﻿using Microsoft.EntityFrameworkCore;

namespace Order.API.OrderServices
{
    public class Order
    {
        public int Id { get; set; }
        public string OrderCode { get; set; } = null!;
        public DateTime Created { get; set; }
        public int UserId { get; set; }
        public OrderStatus OrderStatus { get; set; }
        public List<OrderItem> OrderItems { get; set; } = null!;
    }

    public enum OrderStatus
    {
        Success = 1,
        Fail = 0
    }

    public class OrderItem
    {
        public int Id { get; set; }
        public int ProductId { get; set; }
        public int Count { get; set; }

        [Precision(10,2)]
        public decimal UnitPrice { get; set; }
        public int OrderId { get; set; }
        public Order Order { get; set; } = null!;
    }
}
