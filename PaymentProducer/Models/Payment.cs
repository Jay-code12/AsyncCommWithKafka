namespace PaymentProducer.Models
{
    public class Payment
    {
        public Guid Id { get; set; }
        public bool IsSuccess { get; set; } = false;
        public int Amount { get; set; }
        public string Type { get; set; }
    }
}
