using hotelbooking.Exceptions;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;


namespace hotelbooking.Controllers
{
    [ApiController]
    public class ErrorController : ControllerBase
    {
        [Route("/error")]
        public IActionResult HandleError()
        {
            Exception? exception = HttpContext.Features.Get<IExceptionHandlerFeature>()?.Error;

            if (exception is CustomerNotFoundException)
            {
                return NotFound();
            }
            if(exception is InvalidCustomerException)
            {
                return BadRequest(exception.Message);
            }
            return Problem();
          
        }
    }
}
