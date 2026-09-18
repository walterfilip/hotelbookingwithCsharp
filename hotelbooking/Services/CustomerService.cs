using hotelbooking.Models;
using hotelbooking.Exceptions;

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

        public List<Customer> GetAllCustomers()
        {
            return _customers;
        }

        public Customer GetCustomer(int id)
        {
            return _customers.FirstOrDefault(c => c.Id == id);
        }
        public Customer CreateCustomer(Customer customer)
        {
            customer.Id = _customers.Count + 1;
            _customers.Add(customer);
            Console.WriteLine($"antal kunder: {_customers.Count}");
            //Console.WriteLine($"{customer.Id}");
            // _logger.LogInformation($"antal kunder: {_customers.Count}");


            return customer;
        }

        public Customer UpdateCustomer(int id, Customer customer)
        {
            Customer ? existingCustomer = _customers.FirstOrDefault(c => c.Id == id);

            if(existingCustomer == null)
            {
                throw new CustomerNotFoundException("Kunden finns ej");
            }

            existingCustomer.FirstName = customer.FirstName;
            existingCustomer.Lastname = customer.Lastname;
            existingCustomer.Email = customer.Email;

            return existingCustomer;
        }

        public bool DeleteCustomer(int id)
        {
            Customer customer = _customers.FirstOrDefault(c => c.Id == id);

            if(customer == null)
            {
                return false;
            }

            _customers.Remove(customer);
            return true;
        }
    }
}
