using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FutreTechAPI.BL;
using FutreTechAPI.BL.NotificationV1;
using Microsoft.AspNetCore.Mvc;

namespace FutreTechAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class NotificationController : ControllerBase
    {
        private readonly INotificationService notificationService;

        public NotificationController(INotificationService notificationService)
        {
            this.notificationService = notificationService;
        }


        [HttpPost("send/email/{from}/{to}")]
        public async Task<ActionResult> SendEmail(string @from, string @to)
        {
            await notificationService.SendEmail(@from, @to);
            return Ok();
        }

        [HttpPost("send/sms/{nr}")]
        public async Task<ActionResult<bool>> SendSMS(string @nr)
        {
            var result = await notificationService.SendSMS(@nr);
            return Ok(result);
        }

 
    }
}
