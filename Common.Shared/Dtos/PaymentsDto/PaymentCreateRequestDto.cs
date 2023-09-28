namespace Common.Shared.Dtos.PaymentsDto
{
    public record PaymentCreateRequestDto
    {
        public string OrderCode { get; set; } = null!;
        public decimal TotalPrice { get; set; }
    }
}
