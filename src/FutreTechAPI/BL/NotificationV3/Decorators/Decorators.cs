using FutreTechAPI.BL.NotificationV3.Contracts;
using System;
using System.Threading.Tasks;

namespace FutreTechAPI.BL.NotificationV3.Decorators
{
    #region decorators
    public class NotificationServiceV3LoogerDecorator : INotificationServiceV3
    {
        private readonly INotificationServiceV3 notificationServiceV3;
        private readonly ILogger logger;

        public NotificationServiceV3LoogerDecorator(INotificationServiceV3 notificationServiceV3, ILogger logger)
        {
            this.notificationServiceV3 = notificationServiceV3;
            this.logger = logger;
        }

        public async Task SendEmail(SendEmailRequest request)
        {
            logger.Write($"Sending {request.FromEmail} to {request.ToEmail}");
            try
            {
                await notificationServiceV3.SendEmail(request);
                logger.Write($"Email {request.FromEmail} to {request.ToEmail}");
            }
            catch (Exception ex)
            {
                logger.Write($"Failed to send email! {ex.Message}");
                throw ex;
            }
        }

        public async Task<bool> SendSMS(SendSMSRequest request)
        {
            logger.Write($"Sending SMS to {request.Nr}");
            var result = await notificationServiceV3.SendSMS(request);
            logger.Write($"SMS to {request.Nr} sent with result {result}");
            return result;
        }
    }

    public class NotificationServiceV3RequestValidatorDecorator : INotificationServiceV3
    {
        private readonly INotificationServiceV3 notificationServiceV3;
        private readonly IObjectValidator validator;

        public NotificationServiceV3RequestValidatorDecorator(INotificationServiceV3 notificationServiceV3, IObjectValidator validator)
        {
            this.notificationServiceV3 = notificationServiceV3;
            this.validator = validator;
        }

        public async Task SendEmail(SendEmailRequest request)
        {
            var result = validator.Validate(request);
            if (!result)
            {
                throw new ArgumentException(nameof(request));
            }
            await notificationServiceV3.SendEmail(request);
        }

        public Task<bool> SendSMS(SendSMSRequest request)
        {
            var result = validator.Validate(request);
            if (!result)
            {
                throw new ArgumentException(nameof(request));
            }
            return notificationServiceV3.SendSMS(request);
        }
    }

    public class NotificationServiceV3RequestCacheDecorator : INotificationServiceV3
    {
        private readonly INotificationServiceV3 notificationServiceV3;
        private readonly ICache cache;

        public NotificationServiceV3RequestCacheDecorator(INotificationServiceV3 notificationServiceV3, ICache cache)
        {
            this.notificationServiceV3 = notificationServiceV3;
            this.cache = cache;
        }

        public async Task SendEmail(SendEmailRequest request)
        {
            await notificationServiceV3.SendEmail(request);
        }

        public async Task<bool> SendSMS(SendSMSRequest request)
        {
            var result = cache.Get<string>("phoneNr");
            if (result != null && result == request.Nr)
            {
                return true;
            }
            var sendResult = await notificationServiceV3.SendSMS(request);
            cache.Add("phoneNr", request.Nr);
            return sendResult;
        }
    }

    #endregion
}
