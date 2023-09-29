using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Common.Shared.Dtos.PaymentsDto
{
    public record PaymentCreateResponseDto
    {
        public string? Description { get; set; }
    }
}
