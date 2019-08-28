using System;
using System.Threading.Tasks;

namespace FutreTechAPI.BL.NotificationV2
{
    public interface INotificationServiceV2
    {
        Task SendEmail(string email, string to);
        Task<bool> SendSMS(string nr);
    }

    public class NotificationServiceV2 : INotificationServiceV2
    {
        private readonly IEmailService emailService;
        private readonly ISMSService smsService;

        public NotificationServiceV2(
            IEmailService emailService,
            ISMSService smsService)
        {
            this.emailService = emailService;
            this.smsService = smsService;
        }

        public async Task SendEmail(string fromEmail, string toEmail)
        {
            if (String.IsNullOrEmpty(fromEmail) || !fromEmail.Contains("@"))
            {
                throw new ArgumentException("Not valid email!");
            }

            if (String.IsNullOrEmpty(toEmail) || !toEmail.Contains("@"))
            {
                throw new ArgumentException("Not valid email!");
            }
            await emailService.Send(fromEmail, toEmail);
        }

        public async Task<bool> SendSMS(string nr)
        {
            if (String.IsNullOrEmpty(nr) || !nr.StartsWith("07"))
            {
                throw new ArgumentException("Not valid phone nr!");
            }
            var result = await smsService.Send(nr);
            return result;
        }
    }

}
