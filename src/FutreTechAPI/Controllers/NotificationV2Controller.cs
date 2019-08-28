using FutreTechAPI.BL.NotificationV2;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace FutreTechAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class NotificationV2Controller : ControllerBase
    {
        private readonly INotificationServiceV2 notificationService;

        public NotificationV2Controller(INotificationServiceV2 notificationService)
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
