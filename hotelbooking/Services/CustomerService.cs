using hotelbooking.Models;
using hotelbooking.Exceptions;
using hotelbooking.Data;

namespace hotelbooking.Services
{
    public class CustomerService
    {
        private readonly List<Customer> _customers =
        [
            new Customer
            {
                Id = 1,
                FirstName = "Jens",
                Lastname ="Sond",
                Email = "jesse@test.se"
            }
            ];
        private readonly AppDbContext _context;

        public CustomerService(AppDbContext context)
        {
            _context = context;
        }

        public List<Customer> GetAllCustomers()
        {
            return _context.Customers.ToList();
        }

        public Customer ? GetCustomer(int id)
        {
            return _context.Customers.FirstOrDefault(c => c.Id == id);
        }
        public Customer CreateCustomer(Customer customer)
        {
            if (string.IsNullOrWhiteSpace(customer.FirstName))
            {
                throw new InvalidCustomerException("förnamn måste anges");
            }
            if (string.IsNullOrWhiteSpace(customer.Lastname))
            {
                throw new InvalidCustomerException("efternamn måste anges");
            }
            _context.Customers.Add(customer);
            _context.SaveChanges();

            return customer;
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
            if (string.IsNullOrWhiteSpace(customer.Lastname))
            {
                throw new InvalidCustomerException("Efternamn måste anges");
            }

            existingCustomer.FirstName = customer.FirstName;
            existingCustomer.Lastname = customer.Lastname;
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
