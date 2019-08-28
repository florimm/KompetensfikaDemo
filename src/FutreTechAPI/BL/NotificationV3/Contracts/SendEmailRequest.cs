using FluentValidation;

namespace FutreTechAPI.BL.NotificationV3.Contracts
{
    public class SendEmailRequest
    {
        public string FromEmail { get; set; }
        public string ToEmail { get; set; }
    }

    public class SendEmailRequestValidator : AbstractValidator<SendEmailRequest>
    {
        public SendEmailRequestValidator()
        {
            this.RuleFor(t => t.FromEmail).EmailAddress();
            this.RuleFor(t => t.ToEmail).EmailAddress();
        }
    }
}
