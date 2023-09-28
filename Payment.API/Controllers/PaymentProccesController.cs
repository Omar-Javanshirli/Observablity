using Common.Shared.Dtos;
using Common.Shared.Dtos.PaymentsDto;
using Microsoft.AspNetCore.Mvc;

namespace Payment.API.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class PaymentProccesController : ControllerBase
    {
        [HttpPost]
        public IActionResult Create(PaymentCreateRequestDto request)
        {
            const decimal balance = 1000;

            if (request.TotalPrice > balance)
                return BadRequest(ResponseDto<string>.Fail(400, "balansda kifayet qeder vasait yoxdur"));

            return Ok(ResponseDto<string>.Success(200,"kart prosesi ugurla heyata kecmisdir"));
        }
    }
}
