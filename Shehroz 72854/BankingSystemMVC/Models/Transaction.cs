namespace BankingSystemMVC.Models
{
    public class Transaction
    {
        public int Id { get; set; }
        public DateTime Date { get; set; }
        public string Action { get; set; }
        public double Amount { get; set; }
        public double FinalBalance { get; set; }
        public string AccountNumber { get; set; }
    }
}
