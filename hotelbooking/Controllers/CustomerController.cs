using hotelbooking.Services;
using hotelbooking.Models;
using hotelbooking.DTOs;

using Microsoft.AspNetCore.Mvc;

namespace hotelbooking.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class CustomerController : ControllerBase
    {
        private readonly CustomerService _customerService;

        public CustomerController(CustomerService customerService)
        {
            _customerService = customerService;
        }

        [HttpGet]
        public ActionResult<List<CustomerResponseDto>> GetAllCustomers()
        {
            var customers = _customerService.GetAllCustomers();

            return Ok(customers.Select(ToDto).ToList());
        }
        [HttpGet("{id}")]       
        public ActionResult<CustomerResponseDto> GetCustomer(int id)
        {
            var customer = _customerService.GetCustomer(id);
            
            return Ok(ToDto(customer));
        }

        [HttpPost]
        public ActionResult<CustomerResponseDto> CreateCustomer(Customer customer)
        {
            var created = _customerService.CreateCustomer(customer);
            var dto = ToDto(created);

            return CreatedAtAction(nameof(GetCustomer), new { id = dto.Id }, dto);
        }

        [HttpPut("{id}")]
        public ActionResult<CustomerResponseDto> UpdateCustomer(int id, Customer customer)
        {
            var updateCustomer = _customerService.UpdateCustomer(id, customer);

            return Ok(ToDto(updateCustomer));
        }

        [HttpDelete("{id}")]
        public IActionResult DeleteCustomer(int id)
        {
            _customerService.DeleteCustomer(id);

            return NoContent();
        }

        private CustomerResponseDto ToDto(Customer customer)
        {
            return new CustomerResponseDto
            {
                Id = customer.Id,
                FirstName = customer.FirstName,
                LastName = customer.LastName,
                Email = customer.Email
            };
        }
    }
}
