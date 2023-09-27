using Common.Shared.Dtos;
using OpenTelemetry.Shared;
using Order.API.Models;
using Order.API.StockServices;
using System.Diagnostics;
using System.Net;

namespace Order.API.OrderServices
{
    public class OrderService
    {
        private readonly AppDbContext _appDbContext;
        private readonly StockService _stockServices;

        public OrderService(AppDbContext appDbContext, StockService stockServices)
        {
            _appDbContext = appDbContext;
            _stockServices = stockServices;
        }

        public async Task<ResponseDto<OrderCreateResponseDto>> CreateAsync(OrderCreateRequestDto request)
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

            StockCheckAndPaymentProcessRequestDto stockRequest = new();

            stockRequest.OrderCode = newOrder.OrderCode;
            stockRequest.OrderItems = request.Items!;

            var (isSuccess, failMessage) = await _stockServices.ChecStockAndPaymentStart(stockRequest);

            if (!isSuccess)
                return ResponseDto<OrderCreateResponseDto>.Fail(HttpStatusCode.InternalServerError.GetHashCode(), failMessage!);

            activity?.AddEvent(new("Sifaris prosesi bitdi"));

            return ResponseDto<OrderCreateResponseDto>.Success
                (HttpStatusCode.OK.GetHashCode(), new OrderCreateResponseDto() { Id = newOrder.Id });
        }
    }
}
