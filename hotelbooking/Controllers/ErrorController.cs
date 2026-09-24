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
                return NotFound(exception.Message);
            }
            if(exception is InvalidCustomerException)
            {
                return BadRequest(exception.Message);
            }
            if(exception is RoomNotFoundException)
            {
                return NotFound(exception.Message);
            }
            if(exception is InvalidRoomException)
            {
                return BadRequest(exception.Message);
            }
            if (exception is InvalidBookingException)
            {
                return BadRequest(exception.Message);
            }
            if (exception is RoomAlreadyBookedException)
            {
                return BadRequest(exception.Message);
            }
            return Problem();
          
        }
    }
}
