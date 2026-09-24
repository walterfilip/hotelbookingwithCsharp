using hotelbooking.Models;
using hotelbooking.Exceptions;
using hotelbooking.Data;

namespace hotelbooking.Services
{
    public class CustomerService
    {
      
        private readonly AppDbContext _context;

        private readonly ILogger<CustomerService> _logger;

        public CustomerService(AppDbContext context, ILogger<CustomerService> logger)
        {
            _context = context;
            _logger = logger;
        }

        public List<Customer> GetAllCustomers()
        {
            return _context.Customers.ToList();
        }

        public Customer GetCustomer(int id)
        {
            Customer? customer = _context.Customers.FirstOrDefault(c => c.Id == id);
            if(customer == null)
            {
                _logger.LogWarning("Kunden med id {Id} hittades inte", id);
                throw new CustomerNotFoundException($"Kund med id {id} finns ej");
            }
            return customer;
        }
        public Customer CreateCustomer(Customer customer)
        {
            if (string.IsNullOrWhiteSpace(customer.FirstName))
            {
                _logger.LogWarning("Försök att skapa kund utan förnamn");
                throw new InvalidCustomerException("förnamn måste anges");
            }
            if (string.IsNullOrWhiteSpace(customer.LastName))
            {
                _logger.LogWarning("Försök att skapa kund utan efternamn");
                throw new InvalidCustomerException("efternamn måste anges");
            }

            try
            {
                _logger.LogInformation("Skapar kund: {FirstName} {Email}", customer.FirstName, customer.Email);
                            _context.Customers.Add(customer);
                            _context.SaveChanges();

                            return customer;
             }catch (Exception ex)
            {
                _logger.LogError(ex, "Ett fel uppstod när kunden skulle sparas");
                throw;
            }
            }
            

        public Customer UpdateCustomer(int id, Customer customer)
        {
            Customer? existingCustomer = _context.Customers.FirstOrDefault(c => c.Id == id);

            if(existingCustomer == null)
            {
                throw new CustomerNotFoundException("Kunden finns ej");
            }
            if (string.IsNullOrWhiteSpace(customer.FirstName))
            {
                throw new InvalidCustomerException("Förnamn måste anges");
            }
            if (string.IsNullOrWhiteSpace(customer.LastName))
            {
                throw new InvalidCustomerException("Efternamn måste anges");
            }

            existingCustomer.FirstName = customer.FirstName;
            existingCustomer.LastName = customer.LastName;
            existingCustomer.Email = customer.Email;

            _context.SaveChanges();

            return existingCustomer;
        }

        public bool DeleteCustomer(int id)
        {
            Customer? customer = _context.Customers.FirstOrDefault(c => c.Id == id);

            if(customer == null)
            {
                return false;
            }

            _context.Customers.Remove(customer);
            _context.SaveChanges();

            return true;
        }
    }
}
