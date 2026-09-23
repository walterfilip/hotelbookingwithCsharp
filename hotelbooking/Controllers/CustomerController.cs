using hotelbooking.Services;
using hotelbooking.Models;

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
        public List<Customer> GetAllCustomers()
        {
            return _customerService.GetAllCustomers();
        }
        [HttpGet("{id}")]       
        public ActionResult<Customer> GetCustomer(int id)
        {
            Customer customer = _customerService.GetCustomer(id);
            
            return Ok(customer);
        }

        [HttpPost]
        public ActionResult<Customer> CreateCustomer(Customer customer)
        {
           
            Customer createCustomer = _customerService.CreateCustomer(customer);

            return CreatedAtAction(nameof(GetCustomer), new { id = createCustomer.Id }, createCustomer);
        }

        [HttpPut("{id}")]
        public ActionResult<Customer> UpdateCustomer(int id, Customer customer)
        {
            Customer updateCustomer = _customerService.UpdateCustomer(id, customer);

            return Ok(updateCustomer);

        }

        [HttpDelete("{id}")]
        public IActionResult DeleteCustomer(int id)
        {
            bool deleted = _customerService.DeleteCustomer(id);

            if (!deleted)
            {
                return NotFound();
            }
            return NoContent();
        }
    }
}
