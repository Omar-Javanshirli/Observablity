using Common.Shared.Dtos;

namespace Order.API.StockServices
{
    public class StockService
    {
        private readonly HttpClient _httpClient;

        public StockService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<(bool isSuccess, string? failMessage)> ChecStockAndPaymentStart
            (StockCheckAndPaymentProcessRequestDto request)
        {
            var response = await _httpClient.PostAsJsonAsync<StockCheckAndPaymentProcessRequestDto>("api/Stock/CheckAndPaymentStart", request);

            var responseContent = await response.Content.ReadFromJsonAsync<ResponseDto<StockCheckAndPaymentProcessResponseDto>>();

            return response.IsSuccessStatusCode ? (true, null) : (false, responseContent!.Errors!.First());
        }
    }
}
