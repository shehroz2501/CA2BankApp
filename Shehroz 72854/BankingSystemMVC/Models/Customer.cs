namespace BankingSystemMVC.Models
{
    public class Customer
    {
        public int Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string AccountNumber { get; set; }
        public string Email { get; set; }
        public string PIN { get; set; }
        public double SavingsBalance { get; set; }
        public double CurrentBalance { get; set; }
    }
}
