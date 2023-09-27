namespace Common.Shared.Dtos
{
    public record StockCheckAndPaymentProcessRequestDto
    {
        public string OrderCode { get; set; } = null!;
        public List<OrderItemDto> OrderItems { get; set; } = null!;
    }
}
