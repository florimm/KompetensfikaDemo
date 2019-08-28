using FutreTechAPI.BL;
using FutreTechAPI.BL.NotificationV4;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace FutreTechAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class NotificationV4Controller : ControllerBase
    {
        private readonly IMediator mediator;

        public NotificationV4Controller(IMediator mediator)
        {
            this.mediator = mediator;
        }


        [HttpPost("send/email/{from}/{to}")]
        public async Task<ActionResult> SendEmail(string @from, string @to)
        {
            try
            {
                await mediator.Send(new SendEmail() { From = @from, To = @to });
                return Ok();
            }
            catch (System.Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        [HttpPost("send/sms/{nr}")]
        public async Task<ActionResult<bool>> SendSMS(string nr)
        {
            var result = await mediator.Send(new SendSMS() { Nr = nr });
            return Ok(result);
        }


    }
}
