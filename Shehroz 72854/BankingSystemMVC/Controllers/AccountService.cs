using BankingSystemMVC.Data;
using BankingSystemMVC.Models;
using System;
using System.Linq;
using BankingSystemMVC.Models;

namespace BankingSystemMVC.Services
{
    public class AccountService
    {
        private readonly ApplicationDbContext _context;

        public AccountService(ApplicationDbContext context)
        {
            _context = context;
        }

        public string GenerateAccountNumber(Customer customer)
        {
            // Calculate the initials and their alphabetical positions
            string initials = $"{customer.FirstName[0]}{customer.LastName[0]}".ToLower();
            int length = customer.FirstName.Length + customer.LastName.Length;
            int firstInitialPosition = Char.ToLower( customer.FirstName[0]) - 'a' + 1;
            int secondInitialPosition =Char.ToLower( customer.LastName[0]) - 'a' + 1;

            // Construct the account number
            return $"{initials}-{length}-{firstInitialPosition}-{secondInitialPosition}";
        }

        public string GeneratePIN(Customer customer)
        {
            // Calculate the PIN based on the alphabetical positions
            int firstInitialPosition = Char.ToLower(customer.FirstName[0]) - 'a' + 1;
            int secondInitialPosition = Char.ToLower(customer.LastName[0]) - 'a' + 1;

            // Combine the positions into a PIN
            return $"{firstInitialPosition:00}{secondInitialPosition:00}";
        }
    }
}
