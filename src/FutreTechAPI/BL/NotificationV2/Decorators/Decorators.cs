using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FutreTechAPI.BL.NotificationV2.Decorators
{
    public class NotificationServiceV2LoogerDecorator : INotificationServiceV2
    {
        private readonly INotificationServiceV2 notificationServiceV2;
        private readonly ILogger logger;

        public NotificationServiceV2LoogerDecorator(INotificationServiceV2 notificationServiceV2, ILogger logger)
        {
            this.notificationServiceV2 = notificationServiceV2;
            this.logger = logger;
        }

        public async Task SendEmail(string fromEmail, string toEmail)
        {
            logger.Write($"Sending {fromEmail} to {toEmail}");
            try
            {
                await notificationServiceV2.SendEmail(fromEmail, toEmail);
                logger.Write($"Email {fromEmail} to {toEmail}");
            }
            catch (Exception ex)
            {
                logger.Write($"Failed to send email! {ex.Message}");
                throw ex;
            }
        }

        public async Task<bool> SendSMS(string nr)
        {
            logger.Write($"Sending SMS to {nr}");
            var result = await notificationServiceV2.SendSMS(nr);
            logger.Write($"SMS to {nr} sent with result {result}");
            return result;
        }
    }

    public class NotificationServiceV2CacheDecorator : INotificationServiceV2
    {
        private readonly INotificationServiceV2 notificationServiceV2;
        private readonly ICache cache;

        public NotificationServiceV2CacheDecorator(INotificationServiceV2 notificationServiceV2, ICache cache)
        {
            this.notificationServiceV2 = notificationServiceV2;
            this.cache = cache;
        }

        public async Task SendEmail(string fromEmail, string toEmail)
        {
            await notificationServiceV2.SendEmail(fromEmail, toEmail);
        }

        public async Task<bool> SendSMS(string nr)
        {
            var result = cache.Get<string>("phoneNr");
            if (result != null && result == nr)
            {
                return true;
            }
            var sendResult = await notificationServiceV2.SendSMS(nr);
            cache.Add("phoneNr", nr);
            return sendResult;
        }
    }
}
