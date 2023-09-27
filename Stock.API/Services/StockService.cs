using Common.Shared.Dtos;
using System.Net;

namespace Stock.API.Services
{
    public class StockService
    {
        private Dictionary<int, int> GetProductStockList()
        {
            Dictionary<int, int> productStockList = new();
            productStockList.Add(1, 10);
            productStockList.Add(2, 20);
            productStockList.Add(3, 30);

            return productStockList;
        }

        public  ResponseDto<StockCheckAndPaymentProcessResponseDto> CheckAndPaymentProcess(StockCheckAndPaymentProcessRequestDto request)
        {
            var productStockList = GetProductStockList();
            var stockStatus = new List<(int productId, bool hasStockExsist)>();

            foreach (var orderItem in request.OrderItems)
            {
                var hasExsistStock = productStockList.Any
                    (x => x.Key == orderItem.ProductId && x.Value >= orderItem.Count);

                stockStatus.Add((orderItem.ProductId, hasExsistStock));
            }

            if (stockStatus.Any(x => x.hasStockExsist == false))
                return ResponseDto<StockCheckAndPaymentProcessResponseDto>.Fail
                    (HttpStatusCode.BadRequest.GetHashCode(), "stock yetersiz");

            //payment prosesi basyacaq.

            return ResponseDto<StockCheckAndPaymentProcessResponseDto>.Success
                  (HttpStatusCode.OK.GetHashCode(),new StockCheckAndPaymentProcessResponseDto() { Description= "stock ayrildi"});
        }
    }
}
