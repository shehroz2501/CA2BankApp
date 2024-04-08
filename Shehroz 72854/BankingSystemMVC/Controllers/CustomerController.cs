using Microsoft.AspNetCore.Mvc;
using BankingSystemMVC.Models;
using System.Linq;
using BankingSystemMVC.Data;

namespace BankingSystemMVC.Controllers
{
    public class CustomerController : Controller
    {
        private readonly ApplicationDbContext _context;

        public CustomerController(ApplicationDbContext context)
        {
            _context = context;
        }

        // Action to display login page
        public IActionResult Login()
        {
            return View();
        }

        // Action to handle login form submission
        [HttpPost]
        public IActionResult Login(string firstName, string lastName, string accountNumber, string pin)
        {
            // Check if the provided credentials are valid
            var customer = _context.Customers.FirstOrDefault(c =>
                c.FirstName == firstName && c.LastName == lastName &&
                c.AccountNumber == accountNumber && c.PIN == pin);

            if (customer != null)
            {
                HttpContext.Session.SetInt32("CustomerId",customer.Id);
                HttpContext.Session.SetString("AccNumber",customer.AccountNumber);
                // If credentials are valid, redirect to the Customer dashboard or another appropriate page
                return RedirectToAction("Index");
            }
            else
            {
                var errorViewModel = new ErrorViewModel
                {
                    ErrorMessage = "Login failed due to invalid Credentials.",
                    Path = HttpContext.Request.Path,
                    StackTrace = "Invalid Credentials."
                };

                return View("Error", errorViewModel);
            }
        }

        // Action to display customer dashboard
        public IActionResult Index()
        {
            return View();
        }

        // Action to retrieve transaction history
        public IActionResult History()
        {
            var accNumber = HttpContext.Session.GetString("AccNumber");
            // Retrieve transactions for the specified account number
            var transactions = _context.Transactions
                .Where(t => t.AccountNumber == accNumber)
                .ToList();

            // Pass the list of transactions to the view for display
            return View(transactions);
        }
        [HttpGet]
        public IActionResult CreateTransaction()
        {
            return View();
        }

        // Action to create a new transaction
        [HttpPost]
        public IActionResult AddMoney(double amount, string accountType)
        {
            var id = HttpContext.Session.GetInt32("CustomerId");
            var customer = _context.Customers.FirstOrDefault(c => c.Id == id);
            var transaction = _context.Transactions.FirstOrDefault();
            transaction.Id = 0;
            transaction.Action = "deposit";
            transaction.AccountNumber = customer.AccountNumber;
            transaction.Date = DateTime.Now;
            transaction.Amount = amount;

            if (customer != null)
            {
                if (accountType == "savings")
                {
                    customer.SavingsBalance += amount;
                    transaction.FinalBalance = customer.SavingsBalance;
                }
                else if(accountType == "current")
                {
                    customer.CurrentBalance += amount;
                    transaction.FinalBalance = customer.CurrentBalance;
                }
            }
            _context.Transactions.Add(transaction);
            _context.SaveChanges();
            return RedirectToAction("Index"); // Redirect to home page or another appropriate page
        }

        [HttpPost]
        public IActionResult SubtractMoney(double amount, string accountType)
        {
            // Retrieve customer from session or database
            var id = HttpContext.Session.GetInt32("CustomerId");
            var customer = _context.Customers.FirstOrDefault(c => c.Id == id);
            var transaction = _context.Transactions.FirstOrDefault();
            transaction.Id = 0;
            transaction.Action = "withdraw";
            transaction.AccountNumber = customer.AccountNumber;
            transaction.Date = DateTime.Now;
            transaction.Amount = amount;

            if (customer != null)
            {
                if (accountType == "savings")
                {
                    if (customer.SavingsBalance >= amount)
                    {
                        customer.SavingsBalance -= amount;
                        transaction.FinalBalance = customer.SavingsBalance;
                    }
                    else
                    {
                        var errorViewModel = new ErrorViewModel
                        {
                            ErrorMessage = "Amount Withdrawl failed due to invalid balances.",
                            Path = HttpContext.Request.Path,
                            StackTrace = "Customer balances are insufficient."
                        };

                        return View("Error", errorViewModel);
                    }
                }
                else if (accountType == "current")
                {
                    if(customer.CurrentBalance >= amount)
                    { 
                    customer.CurrentBalance -= amount;
                        transaction.FinalBalance = customer.CurrentBalance;
                    }
                    else
                    {
                        var errorViewModel = new ErrorViewModel
                        {
                            ErrorMessage = "Amount Withdrawl failed due to invalid balances.",
                            Path = HttpContext.Request.Path,
                            StackTrace = "Customer balances are insufficient."
                        };

                        return View("Error", errorViewModel);
                    }
                }
            }
            _context.Transactions.Add(transaction);
            _context.SaveChanges();
            return RedirectToAction("Index"); // Redirect to home page or another appropriate page
        }

    }
}
