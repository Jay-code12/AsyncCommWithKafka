namespace TransHistoryConsumer.Models
{
    public record HistoryDto
    {
        public string Id { get; set; }
        public bool IsSuccess { get; set; } = false;
        public int Amount { get; set; }
        public string Type { get; set; }
    }
}
