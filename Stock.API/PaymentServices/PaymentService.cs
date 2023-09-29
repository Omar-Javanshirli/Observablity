using Common.Shared.Dtos;
using Common.Shared.Dtos.PaymentsDto;

namespace Stock.API.PaymentServices
{
    public class PaymentService
    {

        private readonly HttpClient _httpClient;

        public PaymentService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<(bool isSuccess, string? failMessage)> CreatePaymentProcess
            (PaymentCreateRequestDto request)
        {
            var response = await _httpClient.PostAsJsonAsync<PaymentCreateRequestDto>("api/PaymentProcces/Create", request);

            var responseContent = await response.Content.ReadFromJsonAsync<ResponseDto<PaymentCreateResponseDto>>();

            return response.IsSuccessStatusCode ? (true, null) : (false, responseContent!.Errors!.First());
        }
    }
}
