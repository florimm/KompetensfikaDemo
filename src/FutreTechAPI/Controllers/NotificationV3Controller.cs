using FutreTechAPI.BL;
using FutreTechAPI.BL.NotificationV3.Contracts;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace FutreTechAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class NotificationV3Controller : ControllerBase
    {
        private readonly INotificationServiceV3 notificationService;

        public NotificationV3Controller(INotificationServiceV3 notificationService)
        {
            this.notificationService = notificationService;
        }


        [HttpPost("send/email/{from}/{to}")]
        public async Task<ActionResult> SendEmail(string @from, string @to)
        {
            await notificationService.SendEmail(new SendEmailRequest(){ FromEmail = @from, ToEmail = @to });
            return Ok();
        }

        [HttpPost("send/sms/{nr}")]
        public async Task<ActionResult<bool>> SendSMS(string nr)
        {
            var result = await notificationService.SendSMS(new SendSMSRequest(){Nr = nr });
            return Ok(result);
        }


    }
}
