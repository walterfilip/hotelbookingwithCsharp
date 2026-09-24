using hotelbooking.Data;
using hotelbooking.Models;
using hotelbooking.Services;
using hotelbooking.Exceptions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;


namespace hotelbooking.Tests
{
    public class CustomerServiceTests
    {
        private readonly AppDbContext _context;
        private readonly CustomerService _service;
        public CustomerServiceTests()
        {
            var options = new DbContextOptionsBuilder<AppDbContext>()
                 .UseInMemoryDatabase(Guid.NewGuid().ToString())
                 .Options;

            _context = new AppDbContext(options);

            using var loggerFactory = LoggerFactory.Create(builder => { });
            var logger = loggerFactory.CreateLogger<CustomerService>();

            _service = new CustomerService(_context, logger);
        }

        [Fact]
        public void GetCustomerShouldReturnCustomer()
        {
            var customer = new Customer
            {
                FirstName = "John",
                LastName = "Doe",
                Email = "john.doe@example.com"
            };

            _context.Customers.Add(customer);
            _context.SaveChanges();


            var result = _service.GetCustomer(1);

            Assert.NotNull(result);
            Assert.Equal("John", result.FirstName);
            Assert.Equal("Doe", result.LastName);

        }
        [Fact]
        public void GetCustomerShouldThrowExceptionWhenCustomerDoesNotExist()
        {
            var exception = Assert.Throws<CustomerNotFoundException>(() => _service.GetCustomer(999)); // Assuming 999 is an ID that does not exist
            Assert.Equal($"Kund med id 999 finns ej", exception.Message);

        }
        [Fact]
        public void CreateCustomerShouldReturnCustomer()
        {
            var customer = new Customer
            {
                FirstName = "Jane",
                LastName = "Smith",
                Email = "jane.smith@example.com"
            };

            var result = _service.CreateCustomer(customer);

            Assert.NotNull(result);
            Assert.Equal(customer.FirstName, result.FirstName);
            Assert.Equal(customer.LastName, result.LastName);
            Assert.Equal(customer.Email, result.Email);

            var savedCustomer = _context.Customers.FirstOrDefault(c => c.Email == customer.Email);
            Assert.NotNull(savedCustomer);
        }

        [Fact]
        public void CreateCustomerShouldThrowExceptionWhenFirstNameIsMissing()
        {
            var customer = new Customer
            {
                FirstName = "",
                LastName = "Smith",
                Email = "jane.smith@example.com"
            };

            var exception = Assert.Throws<InvalidCustomerException>(
                () => _service.CreateCustomer(customer));
            Assert.Equal("förnamn måste anges", exception.Message);
        }
        [Fact]
        public void CreateCustomerShouldThrowExceptionWhenLastNameIsMissing()
        {
            var customer = new Customer
            {
                FirstName = "Jane",
                LastName = "",
                Email = "jane.smith@example.com"
            };

            var exception = Assert.Throws<InvalidCustomerException>(
                () => _service.CreateCustomer(customer));
            Assert.Equal("efternamn måste anges", exception.Message);
        }
        [Fact]
        public void UpdateCustomerShouldUpdateCustomer()
        {
            Customer customer = new Customer
            {
                FirstName = "Janne",
                LastName = "Svensson",
                Email = "janne.svensson@example.com"
            };

            var createdCustomer = _service.CreateCustomer(customer);

            _service.UpdateCustomer(createdCustomer.Id, new Customer
            {
                FirstName = "Jens",
                LastName = "Sandström",
                Email = "jens.sandstrom@example.com"
            });

            var updatedCustomer = _service.GetCustomer(createdCustomer.Id);

            Assert.NotNull(updatedCustomer);
            Assert.Equal(createdCustomer.Id, updatedCustomer.Id);
            Assert.Equal("Jens", updatedCustomer.FirstName);
            Assert.Equal("Sandström", updatedCustomer.LastName);
            Assert.Equal("jens.sandstrom@example.com", updatedCustomer.Email);

        }

        [Fact]
        public void UpdateCustomerShouldThrowExceptionWhenCustomerDoesNotExist()
        {

            var exception = Assert.Throws<CustomerNotFoundException>(() => _service.UpdateCustomer(999, new Customer
            {
                FirstName = "Jens",
                LastName = "Sandström",
                Email = "jens.sandstrom@example.com"

            }));
            Assert.Equal("Kunden finns ej", exception.Message);
        }
        [Fact]
        public void UpdateCustomerShouldThrowExceptionWhenFirstNameIsMissing()
        {
            Customer customer = new Customer
            {
                FirstName = "Janne",
                LastName = "Svensson",
                Email = "janne.svensson@example.com"

            };
            _service.CreateCustomer(customer);

            var exception = Assert.Throws<InvalidCustomerException>(() => _service.UpdateCustomer(customer.Id, new Customer
            {
                FirstName = "",
                LastName = "Sandström",
                Email = "jens.sandstrom@example.com"

            }));
            Assert.Equal("Förnamn måste anges", exception.Message);
        }
        [Fact]
        public void UpdateCustomerShouldThrowExceptionWhenLastNameIsMissing()
        {
            Customer customer = new Customer
            {
                FirstName = "Janne",
                LastName = "Svensson",
                Email = "janne.svensson@example.com"
            };

            var createdCustomer = _service.CreateCustomer(customer);

            var exception = Assert.Throws<InvalidCustomerException>(() => _service.UpdateCustomer(createdCustomer.Id, new Customer
            {
                FirstName = "Jens",
                LastName = "",
                Email = "jens.sandstrom@example.com"

            }));
            Assert.Equal("Efternamn måste anges", exception.Message);
        }
        [Fact]
        public void DeleteCustomerShouldDeleteCustomer()
        {
            Customer customer = new Customer
            {
                FirstName = "Janne",
                LastName = "Svensson",
                Email = "janne.svensson@example.com"
            };

            var createdCustomer = _service.CreateCustomer(customer);

            var deleted = _service.DeleteCustomer(createdCustomer.Id);
            Assert.True(deleted);

            var deletedCustomer = Assert.Throws<CustomerNotFoundException>(() => _service.GetCustomer(createdCustomer.Id));
            Assert.Equal($"Kund med id {createdCustomer.Id} finns ej", deletedCustomer.Message);
        }

        [Fact]
        public void DeleteCustomerShouldReturnFalseWhenCustomerDoesNotExist()
        {
            var result = _service.DeleteCustomer(999);

            Assert.False(result);
        }
        [Fact]
        public void GetAllCustomersShouldReturnAllCustomers()
        {
            _service.CreateCustomer(new Customer
            {
                FirstName = "Janne",
                LastName = "Svensson",
                Email = "janne@example.com"
            });

            _service.CreateCustomer(new Customer
            {
                FirstName = "Anna",
                LastName = "Andersson",
                Email = "anna@example.com"
            });

            var result = _service.GetAllCustomers();

            Assert.Equal(2, result.Count);
        }
        [Fact]
        public void GetAllCustomersShouldReturnEmptyListWhenNoCustomersExist()
        {
            var result = _service.GetAllCustomers();

            Assert.Empty(result);
        }
    }
}
