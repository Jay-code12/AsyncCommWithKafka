namespace TransactionHistory.Models
{
    public class History
    {
        public Guid Id { get; set; }
        public Guid PaymentId { get; set; }
        public bool IsSuccess { get; set; } = false;
        public int Amount { get; set; }
        public string Type { get; set; }
    }
}
