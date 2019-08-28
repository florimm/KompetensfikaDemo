using FutreTechAPI.BL.NotificationV4B;
using FutreTechAPI.BL.NotificationV4B.Queries;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FutreTechAPI.Controllers
{
    public class NotficationV4BController : ControllerBase
    {
        private readonly Mediator mediator;

        public NotficationV4BController(Mediator mediator)
        {
            this.mediator = mediator;
        }


        [HttpPost("send/email/{from}/{to}")]
        public async Task<ActionResult> SendEmail(string @from, string @to)
        {
            await mediator.Dispatch(new SendEmailCommand() { From = @from, To = @to });
            return Ok();
        }

        [HttpPost("send/sms/{nr}")]
        public async Task<ActionResult<bool>> SendSMS(string nr)
        {
            try
            {
                var result = await mediator.Dispatch(new SendSMSCommand() { Nr = nr });
                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        [HttpGet("city/cityId")]
        public async Task<ActionResult<bool>> GetCityById(int cityId)
        {
            try
            {
                var result = await mediator.Dispatch(new GetCitiyById { Id = cityId });
                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }


    }
}
