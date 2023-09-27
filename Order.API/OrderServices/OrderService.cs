using OpenTelemetry.Shared;
using Order.API.Models;
using System.Diagnostics;

namespace Order.API.OrderServices
{
    public class OrderService
    {
        private readonly AppDbContext _appDbContext;

        public OrderService(AppDbContext appDbContext)
        {
            _appDbContext = appDbContext;
        }

        public async Task<OrderCreateResponseDto> CreateAsync(OrderCreateRequestDto request)
        {
            Activity.Current?.SetTag("Asp .Net Core (Instrumentation) tag 1", "Asp .Net Core (Instrumentation) tag value");

            using var activity = ActivitySourceProvider.Source.StartActivity();
            activity?.AddEvent(new("Sifaris prosesi basladi"));

            var newOrder = new Order()
            {
                Created = DateTime.Now,
                OrderCode = Guid.NewGuid().ToString(),
                OrderStatus = OrderStatus.Success,
                UserId = request.UserId,
                OrderItems = request.Items!.Select(x => new OrderItem()
                {
                    Count = x.Count,
                    ProductId = x.ProductId,
                    UnitPrice = x.UnitPrice
                }).ToList()
            };

            _appDbContext.Orders.Add(newOrder);
            await _appDbContext.SaveChangesAsync();


            activity?.SetTag("order user id: ", request.UserId);
            activity?.AddEvent(new("Sifaris prosesi bitdi"));

            return new OrderCreateResponseDto() { Id = newOrder.Id };
        }
    }
}
