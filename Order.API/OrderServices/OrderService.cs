﻿using Common.Shared.Dtos;
using Common.Shared.Events;
using MassTransit;
using OpenTelemetry.Shared;
using Order.API.Models;
using Order.API.RedisServices;
using Order.API.StockServices;
using System.Diagnostics;
using System.Net;

namespace Order.API.OrderServices
{
    public class OrderService
    {
        private readonly AppDbContext _appDbContext;
        private readonly StockService _stockServices;
        private readonly RedisService _redisService;
        private readonly ILogger<OrderService> _logger;

        public OrderService(AppDbContext appDbContext, StockService stockServices, RedisService redisService, ILogger<OrderService> logger)
        {
            _appDbContext = appDbContext;
            _stockServices = stockServices;
            _redisService = redisService;
            _logger = logger;
        }

        public async Task<ResponseDto<OrderCreateResponseDto>> CreateAsync(OrderCreateRequestDto request)
        {
            using (var redisActivity = ActivitySourceProvider.Source.StartActivity("Redis Request"))
            {
                // OpenTelemetry.Instrumentation.Redis paketini yuklediyimiz zaman
                //aftomatik redise yazilan datalari activity edecek. Eger istisek ozumuzde yarada bilerik.
                // using blogu ile. ama using yazmasax bele yeni activity yaratmasaq bele paket sahesinde 
                //ozu activity edecek.
                await _redisService.GetDatabase(3).StringSetAsync("userId",request.UserId);
                //var redisUserId=_redisService.GetDatabase(3).StringGetAsync("userId");
            }


            Activity.Current?.SetTag("Asp .Net Core (Instrumentation) tag 1", "Asp .Net Core (Instrumentation) tag value");

            using var activity = ActivitySourceProvider.Source.StartActivity();
            activity?.AddEvent(new("Sifaris prosesi basladi"));

            _logger.LogInformation("Sifaris database save edildi. {@userId}",request.UserId);

            //microservicler arasi data oturmek ucun lazim olur. requestin header hissesinin Tracestate alaninda dasiyacax. 
            activity?.SetBaggage("userId", request.UserId.ToString());

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
