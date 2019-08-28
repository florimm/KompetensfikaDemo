using FutreTechAPI.BL.NotificationV3.Contracts;
using System;
using System.Threading.Tasks;

namespace FutreTechAPI.BL
{
    public interface INotificationServiceV3
    {
        Task SendEmail(SendEmailRequest request);
        Task<bool> SendSMS(SendSMSRequest request);

    }

    public class NotificationServiceV3 : INotificationServiceV3
    {
        private readonly IEmailService emailService;
        private readonly ISMSService smsService;
        private readonly IObjectValidator validator;

        public NotificationServiceV3(
            IObjectValidator validator,
            IEmailService emailService,
            ISMSService smsService)
        {
            this.emailService = emailService;
            this.smsService = smsService;
            this.validator = validator;
        }

        public async Task SendEmail(SendEmailRequest request)
        {
            var result = validator.Validate(request);
            if (!result)
            {
                throw new ArgumentException(nameof(request));
            }
            await emailService.Send(request.FromEmail, request.ToEmail);
        }

        public Task<bool> SendSMS(SendSMSRequest request)
        {
            var result = validator.Validate(request);
            if (!result)
            {
                throw new ArgumentException(nameof(request));
            }

            return smsService.Send(request.Nr);
        }
    }
}
