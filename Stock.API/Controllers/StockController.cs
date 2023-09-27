using Common.Shared.Dtos;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Stock.API.Services;

namespace Stock.API.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class StockController : ControllerBase
    {
        private readonly StockService _stockService;

        public StockController(StockService stockService)
        {
            _stockService = stockService;
        }

        [HttpPost]
        public IActionResult CheckAndPaymentStart(StockCheckAndPaymentProcessRequestDto request)
        {
            var result = _stockService.CheckAndPaymentProcess(request);
            return new ObjectResult(result) { StatusCode = result.StatusCode };
        }
    }
}
