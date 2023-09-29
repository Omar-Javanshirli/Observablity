namespace Common.Shared.Events
{
    public record OrderCreatedEvent
    {
        //public Dictionary<string,string> Headers { get; set; }
        public string OrderCode { get; set; } = null!;
    }
}
