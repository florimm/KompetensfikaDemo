using System;
using System.Threading.Tasks;

namespace FutreTechAPI.BL.NotificationV1
{
    public interface INotificationService
    {
        Task SendEmail(string fromEmail, string toEmail);

        Task<bool> SendSMS(string nr);
    }

    public class NotificationService : INotificationService
    {
        private readonly ILogger logger;
        private readonly IEmailService emailService;
        private readonly ISMSService smsService;
        private readonly ICache cache;

        public NotificationService(
            ILogger logger,
            IEmailService emailService,
            ISMSService smsService,
            ICache cache)
        {
            this.logger = logger;
            this.emailService = emailService;
            this.smsService = smsService;
            this.cache = cache;
        }

        public async Task SendEmail(string fromEmail, string toEmail)
        {
            logger.Write($"Request {fromEmail} to {toEmail}");
            if (String.IsNullOrEmpty(fromEmail) || !fromEmail.Contains("@"))
            {
                throw new ArgumentException("Not valid email!");
            }

            if (String.IsNullOrEmpty(toEmail) || !toEmail.Contains("@"))
            {
                throw new ArgumentException("Not valid email!");
            }
            try
            {
                await emailService.Send(fromEmail, toEmail);
                logger.Write($"Email {fromEmail} to {toEmail}");
            }
            catch (Exception ex)
            {
                logger.Write($"Failed to send email! {ex.Message}");
            }
        }

        public async Task<bool> SendSMS(string nr)
        {
            logger.Write($"Request to send SMS to {nr}");
            if (String.IsNullOrEmpty(nr) || !nr.StartsWith("07"))
            {
                throw new ArgumentException("Not valid phone nr!");
            }
            var existInCache = cache.Get<string>(nr);
            if(existInCache != null)
            {
                var result = await smsService.Send(nr);
                logger.Write($"SMS to {nr} sent result {result}");
                return result;
            }
            logger.Write($"Already send sms to {nr}");
            return true;
        }

        
    }
}
