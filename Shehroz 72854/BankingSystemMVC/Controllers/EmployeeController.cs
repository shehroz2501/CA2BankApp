using BankingSystemMVC.Data;
using BankingSystemMVC.Models;
using BankingSystemMVC.Services;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;

namespace BankingSystemMVC.Controllers
{
    public class EmployeeController : Controller
    {
        private readonly ILogger<EmployeeController> _logger;
        private readonly ApplicationDbContext _context;
        private readonly AccountService _accountServices;

        public EmployeeController(ILogger<EmployeeController> logger, ApplicationDbContext context, AccountService accountService)
        {
            _logger = logger;
            _context = context;
            _accountServices = accountService;
        }

       public IActionResult Login()
        {
            return View();
        }
        // Action to handle login form submission
        [HttpPost]
        public IActionResult Login(string pin)
        {

            if (pin == "A1234")
            {
                return RedirectToAction("Index");
            }
            else
            {
                var errorViewModel = new ErrorViewModel
                {
                    ErrorMessage = "Login failed due to incorrect PIN.",
                    Path = HttpContext.Request.Path,
                    StackTrace = "Incorrect PIN."
                };

                return View("Error", errorViewModel);
            }
        }

        // Index action for Employee dashboard
        public IActionResult Index()
        {
            return View();
        }
        [HttpGet]
        public IActionResult CreateCustomer()
        {
            return View();
        }

        // Action to create a new customer
        [HttpPost]
        public IActionResult CreateCustomer(Customer customer)
        {
            
                // Generate account number and PIN
                customer.AccountNumber = _accountServices.GenerateAccountNumber(customer);
                customer.PIN = _accountServices.GeneratePIN(customer);

                // Add customer to database
                _context.Customers.Add(customer);
                _context.SaveChanges();

                // Redirect to the Employee dashboard or another appropriate page
                return RedirectToAction("Index");
            

        }
        [HttpGet]
        public IActionResult DeleteCustomer()
        {
            return View();
        }

        // Action to delete a customer
        [HttpPost]
        public IActionResult DeleteCustomer(string accNumber)
        {
            // Retrieve customer from database
            var customer = _context.Customers.FirstOrDefault(c => c.AccountNumber == accNumber);

            if (customer != null && customer.SavingsBalance == 0 && customer.CurrentBalance == 0)
            {
                // Perform deletion logic if customer is found
                _context.Customers.Remove(customer);
                _context.SaveChanges();
            }
            else
            {
                var errorViewModel = new ErrorViewModel
                {
                    ErrorMessage = "Customer deletion failed due to non-zero balances.",
                    Path = HttpContext.Request.Path,
                    StackTrace = "Customer balances are not zero."
                };

                return View("Error", errorViewModel);
            }

            // Redirect to the Employee dashboard or another appropriate page
            return RedirectToAction("Index");
        }

        [HttpGet]
        public IActionResult CreateTransaction()
        {
            return View();
        }
        // Action to create a transaction
        [HttpPost]
        public IActionResult CreateTransaction(Transaction transaction,string accType)
        {
            var customer = _context.Customers.FirstOrDefault(c => c.AccountNumber == transaction.AccountNumber);

            // Add transaction to database
            if (accType == "savings")
            {         
                if(transaction.Action == "deposit")
                {
                    customer.SavingsBalance += transaction.Amount;
                }
                else if(transaction.Action == "withdraw")
                {
                    if (customer.SavingsBalance >=transaction.Amount)
                    {
                        customer.SavingsBalance -= transaction.Amount;
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
                transaction.FinalBalance = customer.SavingsBalance;
            }
            else if (accType == "current")
            {
                if (transaction.Action == "deposit")
                {
                    customer.CurrentBalance += transaction.Amount;
                }
                else if (transaction.Action == "withdraw")
                {
                    if (customer.CurrentBalance >= transaction.Amount)
                    {
                        customer.CurrentBalance -= transaction.Amount;
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
                transaction.FinalBalance = customer.CurrentBalance;
            }
            transaction.Date = DateTime.Now;
            _context.Transactions.Add(transaction);
            _context.SaveChanges();
            // Redirect to the Employee dashboard or another appropriate page
            return RedirectToAction("Index");
          
        }

        // Action to list all customers
        public IActionResult ListCustomers()
        {
            // Retrieve all customers from the database
            var customers = _context.Customers.ToList();

            // Pass the list of customers to the view for display
            return View(customers);
        }
    }
}
