using Common.Shared.Dtos;
using Common.Shared.Dtos.PaymentsDto;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Primitives;

namespace Payment.API.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class PaymentProccesController : ControllerBase
    {
        [HttpPost]
        public IActionResult Create(PaymentCreateRequestDto request)
        {
             if (HttpContext.Request.Headers.TryGetValue("traceparent", out StringValues values))
                Console.WriteLine($"traceparent: {values.First()}");

            const decimal balance = 1000;

            if (request.TotalPrice > balance)
                return BadRequest(ResponseDto<PaymentCreateResponseDto>.Fail(400, "balansda kifayet qeder vasait yoxdur"));

            return Ok(ResponseDto<PaymentCreateResponseDto>.Success
                (200,new PaymentCreateResponseDto() { Description="kart prosesi ugurla heyata kecmisdir"}));
        }
    }
}
